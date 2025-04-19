from rest_framework.response import Response
from rest_framework.views import APIView
from rest_framework.permissions import AllowAny, IsAuthenticated
from django.contrib.auth.models import User
from .serializers import UserSerializer
from rest_framework import generics
from rest_framework_simplejwt.tokens import RefreshToken
from django.http import JsonResponse
from allauth.socialaccount.providers.google.views import GoogleOAuth2Adapter
from allauth.socialaccount.providers.oauth2.client import OAuth2Client
from dj_rest_auth.registration.views import SocialLoginView
from django.views.decorators.csrf import csrf_exempt
from django.utils.decorators import method_decorator
from django.contrib.auth.decorators import login_required
from django.utils.http import urlencode
from django.http import HttpResponseRedirect

class RegisterView(generics.CreateAPIView):
    queryset = User.objects.all()
    serializer_class = UserSerializer
    permission_classes = [AllowAny] 

class TestProtectedView(APIView):
    permission_classes = [IsAuthenticated]
    
    def get(self, request):
        return Response({'message': 'This is a protected endpoint!'})
    
class OAuthRedirectView(APIView):
    def get(self, request):
        user = request.user
        refresh = RefreshToken.for_user(user)
        return JsonResponse({
            'access': str(refresh.access_token),
            'refresh': str(refresh),
        })

@method_decorator(csrf_exempt, name='dispatch')
class GoogleLogin(SocialLoginView):
    adapter_class = GoogleOAuth2Adapter
    callback_url = "https://aae9-217-123-90-227.ngrok-free.app/accounts/google/login/callback/"
    client_class = OAuth2Client

class GoogleRedirectView(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request):
        user = request.user
        refresh = RefreshToken.for_user(user)
        access_token = str(refresh.access_token)
        refresh_token = str(refresh)

        query_params = urlencode({
            'access_token': access_token,
            'refresh_token': refresh_token
        })
        HttpResponseRedirect.allowed_schemes.append('whatmunch')
        return HttpResponseRedirect(f"whatmunch://oauth-redirect?{query_params}")