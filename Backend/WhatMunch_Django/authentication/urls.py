from django.urls import path
from .views import RegisterView, TestProtectedView, OAuthRedirectView, GoogleLogin, GoogleRedirectView

urlpatterns = [
    path('register/', RegisterView.as_view(), name='register'),
    path('protected/', TestProtectedView.as_view(), name='protected'),
    path('oauth-redirect-jwt/', OAuthRedirectView.as_view(), name='oauth-redirect-jwt'),
    path('api/auth/google/', GoogleLogin.as_view(), name='google_login'),
    path("google-redirect/", GoogleRedirectView.as_view(), name="google-login-redirect"),
]