FROM python:3.13-alpine

# Install runtime and build dependencies
RUN apk add --no-cache \
        postgresql-client && \
    apk add --no-cache --virtual .build-deps \
        build-base \
        postgresql-dev
    
# Install pip dependencies
COPY requirements.txt .
RUN pip install --upgrade pip && \
    pip install --no-cache-dir -r requirements.txt && \
    apk del .build-deps

# Create and set working directory
WORKDIR /app 

# Set environment variables
ENV PYTHONDONTWRITEBYTECODE=1 \
    PYTHONUNBUFFERED=1

# Copy the Django project
COPY . .

# Expose the port the app runs on
EXPOSE 8080

# Default command
ENTRYPOINT ["sh", "-c", "\
    python manage.py migrate && \
    python manage.py collectstatic --noinput && \
    python manage.py createsu && \
    uvicorn WhatMunch_Django.asgi:application --host 0.0.0.0 --port 8080"]