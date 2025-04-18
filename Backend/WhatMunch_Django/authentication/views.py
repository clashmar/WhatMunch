from rest_framework.response import Response
from rest_framework.views import APIView
from rest_framework.permissions import AllowAny, IsAuthenticated
from django.contrib.auth.models import User
from .serializers import UserSerializer
from rest_framework import generics
from rest_framework_simplejwt.tokens import RefreshToken
from django.http import JsonResponse

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
    

