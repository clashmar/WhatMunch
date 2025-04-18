from django.urls import path
from .views import RegisterView, TestProtectedView, OAuthRedirectView

urlpatterns = [
    path('register/', RegisterView.as_view(), name='register'),
    path('protected/', TestProtectedView.as_view(), name='protected'),
    path('oauth-redirect-jwt/', OAuthRedirectView.as_view(), name='oauth-redirect-jwt'),
]