from dotenv import load_dotenv
from os import getenv
import requests

load_dotenv()

access_token = getenv("VK_ACCESS_TOKEN")

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
    f"https://api.vk.com/method/wall.get?domain={domain}&access_token={access_token}&v=5.199&count=1"
    for domain in public_domains
)


def make_request(url):
    response = requests.get(url)
    return response.json()


print(make_request(next(urls_gen)))
