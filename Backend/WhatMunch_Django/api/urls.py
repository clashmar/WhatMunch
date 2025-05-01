from django.urls import path
from .views import GetPlacesKeyView
from .health import health_check

urlpatterns = [
    path('health/', health_check, name='health-check'),
    path('get-places-key/', GetPlacesKeyView.as_view(), name='get-places-key'),
]