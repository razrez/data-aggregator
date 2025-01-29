FROM python:3.10

ENV PYTHONDONTWRITEBYTECODE=1
ENV PYTHONUNBUFFERED=1
ENV PYTHONPATH=/usr/bin/python3

RUN apt-get update && apt-get install -y cron && apt-get install -y python3-pip

WORKDIR /app

COPY cronjob /etc/cron.d/crontab

RUN chmod 0644 /etc/cron.d/crontab

RUN crontab /etc/cron.d/crontab

COPY . /app

RUN /usr/bin/python3 -m pip install --no-cache-dir --break-system-packages -r requirements.txt

CMD ["/bin/bash", "-c", "printenv > /etc/environment && cron -f"]
