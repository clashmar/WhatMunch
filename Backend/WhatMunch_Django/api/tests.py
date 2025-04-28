import pytest
from django.urls import reverse
from rest_framework import status
from django.contrib.auth import get_user_model

@pytest.mark.django_db
def test_get_places_key_authenticated(client):
    user = get_user_model().objects.create_user(
        username='testuser',
        password='strongpassword123'
    )
    client.login(username='testuser', password='strongpassword123')
    url = reverse('get-places-key')
    response = client.get(url)
    assert response.status_code == status.HTTP_200_OK
    assert 'googleMapsApiKey' in response.data

@pytest.mark.django_db
def test_get_places_key_unauthenticated(client):
    url = reverse('get-places-key')
    response = client.get(url)
    assert response.status_code == status.HTTP_401_UNAUTHORIZED
