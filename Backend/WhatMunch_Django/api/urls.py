from django.urls import path
from .views import GetPlacesKeyView
from .health import health_check

urlpatterns = [
    path('get-places-key/', health_check, name='health-check'),
    path('health/', GetPlacesKeyView.as_view(), name='get-places-key'),
]