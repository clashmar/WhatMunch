from django.urls import path
from .views import RegisterView, TestProtectedView, GoogleRedirectView

urlpatterns = [
    path('register/', RegisterView.as_view(), name='register'),
    path('protected/', TestProtectedView.as_view(), name='protected'),
    path("google-redirect/", GoogleRedirectView.as_view(), name="google-login-redirect"),
]