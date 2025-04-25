from django.urls import path
from .views import GetPlacesKeyView

urlpatterns = [
    path('get-places-key/', GetPlacesKeyView.as_view(), name='get-places-key'),
]