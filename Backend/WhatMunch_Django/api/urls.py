from django.urls import path
from .views import RestaurantListView

urlpatterns = [
    path('restaurants/', RestaurantListView.as_view(), name='restaurants'),
]