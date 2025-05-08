#!/usr/bin/env python3

import os
import json
import notify_discord

COMMITS = json.loads(os.environ["COMMITS"])
PR_NOTIFY_WEBHOOK = os.environ["PR_NOTIFY_WEBHOOK"]

# божи упаси(дебаг строка)
def main():
    content = ""

    content_dict: dict[str, list[str]] = {

    }

    peoples = ['Fanolli', 'mosleyos']

    for commit in COMMITS:
        author = commit["author"]["username"]
        if (author not in peoples):
            print(f"{author} не был найден среди участников вайтлист организаций: {peoples}!")
            continue

        message = commit['message']

        body = notify_discord.format_body(message)

        if (body == "" or body.isspace()):
            continue

        if content_dict.get(author) == None:
            content_dict[author] = [body]  
        else:
            content_dict[author].append(body)

    for author, messages in content_dict.items():
        message = str.join("\n", messages)
        message += f"Автор изменений: {author}\n"

        content += message + "\n"

    if content == "" or content.isspace():
        print("В тексте не найдено :cl: тега и элементов add/remove... и т.д.")
        print("Ошибка при отправке оповещения. Оно было пустым!")
        return

    notify_discord.send_discord(content, PR_NOTIFY_WEBHOOK)

if __name__ == '__main__':
    main()
