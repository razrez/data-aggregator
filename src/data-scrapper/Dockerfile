# Используем "slim" версию образа Python для меньшего размера
FROM python:3.9-slim

# Отключаем создание .pyc-файлов и буферизацию вывода
ENV PYTHONDONTWRITEBYTECODE=1
ENV PYTHONUNBUFFERED=1

# Создаём рабочую директорию в контейнере
WORKDIR /app

# Сначала копируем requirements.txt,
# чтобы Docker кешировал установку зависимостей
COPY requirements.txt /app/

# Устанавливаем зависимости
RUN pip install --no-cache-dir -r requirements.txt

# Копируем оставшиеся файлы скрипта
COPY . /app

# Запускаем основной Python-скрипт (например, main.py)
CMD ["python", "main.py"]
