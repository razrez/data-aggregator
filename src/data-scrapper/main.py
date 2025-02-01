from dotenv import load_dotenv
from time import sleep
from os import getenv
import requests
import logging
import json

load_dotenv()

logging.getLogger().setLevel(logging.INFO)

access_token = getenv("VK_ACCESS_TOKEN")
vk_api_version = getenv("VK_API_VERSION")
posts_count = getenv("POSTS_COUNT")
posts_offset = getenv("POSTS_OFFSET")
infinispan_host = getenv("INFINISPAN_HOST")
infinispan_port = getenv("INFINISPAN_PORT")
infinispan_user = getenv("INFINISPAN_USER")
infinispan_pass = getenv("INFINISPAN_PASS")
infinispan_cache_name = getenv("INFINISPAN_CACHE_NAME")

public_domains = [
    "vk",
    "vkazani",
    "po.recepty",
    "leninka_ru",
    "bookstagram_eng",
    "znanierussia",
    "jumoreski",
    "horror_memoirs",
]

urls_gen = (
    f"https://api.vk.com/method/wall.get?domain={domain}&access_token={access_token}&v={vk_api_version}&count={posts_count}&offset={posts_offset}&extended=1"
    for domain in public_domains
)


def make_request(url):
    response = requests.get(url)
    logging.info(f"response, {response.status_code}, {url}")
    return response.json()


def extract_data(data: dict):
    if data.get("response") is None:
        logging.info(f"Нет данных для извлечения, {data}")
        return

    posts = data["response"]["items"]
    for idx, post in enumerate(posts):
        post_data = {}
        post_data["id"] = post["id"]
        post_data["date"] = post["date"]
        post_data["text"] = post["text"]
        post_data["likes"] = post["likes"]["count"]
        post_data["views"] = post["views"]["count"]
        post_data['public_name'] = data["response"]["groups"][0]["name"]
        logging.info(f'extracted {idx + 1} post from {post_data["id"]}')
        yield post_data


def put_key(key, value):
    logging.debug(f"Сохранение ключа {key} со значением {value} в Infinispan")
    try:
        url = f"http://{infinispan_host}:{infinispan_port}/rest/v2/caches/{infinispan_cache_name}/{key}"

        response = requests.post(
            url,
            data=json.dumps(value),
            auth=(infinispan_user, infinispan_pass),
            headers={"Content-Type": "application/json"},
        )

        if response.status_code == 204:
            logging.info(f"Ключ '{key}' успешно сохранен.")
        elif response.status_code == 409:
            logging.info(
                f'Ключ "{key}" уже существует. Выполняется обновление значения.'
            )

            response = requests.put(
                url,
                data=json.dumps(value),
                auth=(infinispan_user, infinispan_pass),
                headers={"Content-Type": "application/json"},
            )

            if response.status_code == 204:
                logging.info(f"Ключ '{key}' успешно обновлен.")
            else:
                logging.error(
                    f"Ошибка при обновлении ключа '{key}': {response.status_code}, {response.text}"
                )

        else:
            logging.error(
                f"Ошибка при сохранении ключа '{key}': {response.status_code}, {response.text}"
            )
    except Exception as e:
        logging.error(f"Ошибка при сохранении ключа '{key}': {e}")


def create_cache():
    logging.info(f"Создание кэша {infinispan_cache_name} в Infinispan")

    try:
        url = f"http://{infinispan_host}:{infinispan_port}/rest/v2/caches/{infinispan_cache_name}"
        response = requests.post(
            url,
            auth=(infinispan_user, infinispan_pass),
            headers={"Content-Type": "application/json"},
        )

        if response.status_code == 204:
            logging.info(f"Кэш '{infinispan_cache_name}' успешно создан.")
        elif response.status_code == 400:
            logging.info(f'Кэш "{infinispan_cache_name}" уже существует.')
        else:
            logging.error(
                f"Ошибка при создании кэша '{infinispan_cache_name}': {response.status_code}, {response.text}"
            )
    except Exception as e:
        logging.error(f"Ошибка при создании кэша '{infinispan_cache_name}': {e}")
        exit()


if __name__ == "__main__":
    logging.info("Запуск скрипта...")
    logging.info("Инициализация кэша...")
    sleep(10)

    create_cache()
    for url in urls_gen:
        data = make_request(url)
        for post_data in extract_data(data):
            put_key(post_data["id"], post_data)
        sleep(2)

    logging.info("Успешное завершение")
