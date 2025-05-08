#!/usr/bin/env python3

import subprocess
import requests
import os
import hashlib

##GITHUB_TOKEN = os.environ["GITHUB_TOKEN"]
#PUBLISH_TOKEN = os.environ["PUBLISH_TOKEN"]
##GITHUB_REPOSITORY = os.environ["GITHUB_REPOSITORY"]
#VERSION = os.environ['GITHUB_SHA']
#FORK_ID = os.environ['FORK_ID']

LOCALE_PUBLISH_TOKEN = "83ffff70-2a27-42a4-84a2-68c29d140545"
LOCALE_USER_IP = "192.168.0.8"
LOCALE_USER_LISTENER_PORT = 443

ROBUST_CDN_URL = "https://cdn.station14.ru/"

build_name = "build.zip"

def main():
    random_bytes = os.urandom(64)
    token = hashlib.sha256(random_bytes).hexdigest()

    print("Содержимое рабочего каталога пакуется в архив.")
    path_to_archive = create_archive()

    url = f"http://{LOCALE_USER_IP}:{LOCALE_USER_LISTENER_PORT}/publish/work/{token}"

    archive = "/zip"
    upload = "/upload"

    print("Начинается выгрузка архива на удалённую машину.")
    send_archive(url + upload, path_to_archive)

    '''
    print("Начинается отправка на cdn.")
    publish_data = {
        "version": VERSION,
        "engineVersion": get_engine_version(),
        "archive": url + archive
    }
    publish_headers = {
        "Authorization": f"Bearer {PUBLISH_TOKEN}",
        "Content-Type": "application/json"
    }
    publish_resp = requests.post(f"{ROBUST_CDN_URL}fork/{FORK_ID}/publish", json=publish_data, headers=publish_headers)
    publish_resp.raise_for_status()
    print("Отправка на cdn успешна!")
    '''
    print("Начинается удаление выгруженного архива.")
    delete_head = {
        "Authorization": f"Bearer {LOCALE_PUBLISH_TOKEN}",
    }
    delete_resp = requests.delete(url, headers=delete_head)
    delete_resp.raise_for_status()
    print("Удаление выгруженного архива прошло успешно!")


def get_engine_version() -> str:
    return "229.1.2"

def create_archive() -> str:
    cur_dir = os.getcwd()
    parent_dir = os.path.dirname(cur_dir)

    subprocess.run(
        args=['zip', '-5', '-v', '-r', build_name, '.'], 
        cwd=parent_dir, 
        check=True, 
        stderr=subprocess.PIPE, 
        stdin=subprocess.PIPE, 
        stdout=subprocess.PIPE)
    
    return parent_dir + "/" + build_name

def send_archive(url: str, path_to_file: str):
    with open(path_to_file, 'rb') as file:
        files = {'file': file}
        upload_head = {
            "Authorization": f"Bearer {LOCALE_PUBLISH_TOKEN}",
        }
        response = requests.post(url, files=files, headers=upload_head)
        response.raise_for_status()
        print("Отправка прошла успешно!")

if __name__ == '__main__':
    main()
