import pytest
from django.urls import reverse
from django.contrib.auth.models import User
from rest_framework import status

@pytest.mark.django_db
def test_register_user(client):
    url = reverse('register')
    data = {'username': 'testuser', 'email': 'test@example.com', 'password': 'strongpassword123'}
    response = client.post(url, data)
    assert response.status_code == status.HTTP_201_CREATED
    assert User.objects.count() == 1
    assert User.objects.first().email == 'test@example.com'

@pytest.mark.django_db
def test_login_redirect_authenticated(client):
    user = User.objects.create_user(username='testuser', email='test@example.com', password='strongpassword123')
    client.login(username='testuser', password='strongpassword123')
    url = reverse('login-redirect')
    response = client.get(url)
    assert response.status_code == status.HTTP_302_FOUND
    assert 'access_token' in response.url

@pytest.mark.django_db
def test_delete_account(client):
    user = User.objects.create_user(username='testuser', email='test@example.com', password='strongpassword123')
    client.login(username='testuser', password='strongpassword123')
    url = reverse('delete-account')
    response = client.post(url)
    assert response.status_code == status.HTTP_204_NO_CONTENT
    assert User.objects.count() == 0