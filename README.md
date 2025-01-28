# data-aggregator
Service which collects posts from VK (using VK API), caches recent data with InfiniSpan, provides analytical insights  

## Общая информация

Репозиторий организован по принципу «монорепозитория» и содержит три основных микросервиса, а также общий Docker Compose для их одновременного запуска.  
Основные сервисы:
1. **Infinispan** — кеш-хранилище (непосредственно указано в `docker-compose.yml`).
2. **data-scrapper** — Python-скрипт для сбора/обработки данных.
3. **server** — C# ASP.NET Core-приложение (Web API) для выдачи/агрегации данных.
4. **ui** — фронтенд-приложение (React или Vue).

## Корневые файлы

- **`docker-compose.yml`**  
  - Главный файл для развёртывания всех сервисов:
    - Поднимает контейнеры `infinispan`, `data-scrapper`, `server` и `ui`.
    - Настраивает порты и основные переменные окружения для каждого сервиса.

- **`.gitignore` / `.dockerignore`**  
  - Шаблоны, позволяющие исключать временные файлы из репозитория и сборки Docker-образов.

## Каталоги проекта

### `data-scrapper`
- **Назначение**: Python-скрипт для периодического сбора/обработки данных с записью в кеш (Infinispan).  
- **Содержимое**:
  - `Dockerfile`: описывает базу (Python), установку зависимостей из `requirements.txt` и запуск `main.py`.
  - `requirements.txt`: список Python-зависимостей.
  - `main.py` или другой основной скрипт: входная точка (точка запуска).

При запуске `docker-compose up --build` в корне проекта создаётся Docker-образ сервиса `data-scrapper`, который использует переменные окружения из `docker-compose.yml` для доступа к Infinispan.

### `server`
- **Назначение**: C# ASP.NET Core-приложение, обрабатывающее данные из кеша. Предоставляет Web API для `ui` и других потенциальных клиентов.  
- **Содержимое**:
  - `.sln`-файл (решение) и проекты:
    - **`.Web`** (или аналог) — ASP.NET Core Web API.
    - **`.Core`** — бизнес-логика, если применимо.
    - **`.Infrastructure`** — работа с внешними источниками (Infinispan, базы данных и т. п.).
  - `Dockerfile`: инструкции по сборке .NET-приложения (включая `dotnet restore`, `dotnet build`, `dotnet publish`) и проброс порта (обычно `80` внутри контейнера, снаружи маппится на `8080`).

### `ui`
- **Назначение**: Фронтенд-приложение, написанное на React/Vue.  
- **Содержимое**:
  - `package.json` / `package-lock.json` (или `yarn.lock`): описывают зависимости.
  - Исходники в `src/`, статические или публичные файлы в `public/` (React) или `dist/` (Vue, после сборки).
  - `Dockerfile`: может быть в двух вариантах:
    1. Dev-сервер (Node.js) с Hot Reload для разработки.  
    2. Production-сборка (мульти-стейдж: сборка → копирование статических файлов в Nginx).

## Как запустить проект

1. **Установите** Docker и Docker Compose (убедитесь, что они работают корректно).
2. **Клонируйте** репозиторий:
   ```bash
   git clone https://github.com/razrez/data-aggregator.git
   cd data-aggregator/src

2. **Соберите и запустите** все сервисы командой:
    ```bash
    docker-compose up --build
- Будут собраны образы всех сервисов.
- Запустятся контейнеры infinispan, data-scrapper, server и ui.

4. **Проверьте**:
    - Infinispan по порту 11222.
    - server по адресу http://localhost:8080 (Web API).
    - ui по адресу http://localhost:3000 (React/Vue-приложение).
