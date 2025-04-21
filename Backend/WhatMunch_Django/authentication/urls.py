from django.urls import path
from .views import RegisterView, TestProtectedView, LoginRedirectView

urlpatterns = [
    path('register/', RegisterView.as_view(), name='register'),
    path('protected/', TestProtectedView.as_view(), name='protected'),
    path("login-redirect/", LoginRedirectView.as_view(), name="login-redirect"),
]