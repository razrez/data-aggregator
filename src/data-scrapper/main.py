from dotenv import load_dotenv
from os import getenv
import requests

load_dotenv()

access_token = getenv("VK_ACCESS_TOKEN")
vk_api_version = getenv("VK_API_VERSION")
posts_count = getenv("POSTS_COUNT")

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
    f"https://api.vk.com/method/wall.get?domain={domain}&access_token={access_token}&v={vk_api_version}&count={posts_count}"
    for domain in public_domains
)


def make_request(url):
    response = requests.get(url)
    print("response", response.status_code, url)
    return response.json()


def extract_data(data: dict):
    posts = data["response"]["items"]
    for post in posts:
        post_data = {}
        post_data["id"] = post["id"]
        post_data["date"] = post["date"]
        post_data["text"] = post["text"]
        post_data["likes"] = post["likes"]["count"]
        post_data["views"] = post["views"]["count"]
        yield post_data


if __name__ == "__main__":
    for url in urls_gen:
        data = make_request(url)
        for post_data in extract_data(data):
            print(post_data)
