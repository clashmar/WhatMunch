FROM python:3.13-alpine

# Install build dependencies
RUN apk update && \
    apk add --no-cache build-base && \
    rm -rf /var/cache/apk/*
    
# Install pip dependencies
COPY requirements.txt ./
RUN pip install --upgrade pip && \
    pip install --no-cache-dir -r requirements.txt

# Create and set working directory
WORKDIR /app

# Set environment variables
ENV PYTHONDONTWRITEBYTECODE=1 \
    PYTHONUNBUFFERED=1

# Copy the Django project
COPY . .

# Run Django collectstatic
RUN python manage.py collectstatic --noinput

# Expose the port the app runs on
EXPOSE 8080

# Default command
CMD ["sh", "-c", "python manage.py migrate && uvicorn WhatMunch_Django.asgi:application --bind 0.0.0.0:8080"]