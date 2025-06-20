from rest_framework.response import Response
from rest_framework.views import APIView
from rest_framework.permissions import AllowAny, IsAuthenticated
from django.contrib.auth.models import User
from .serializers import UserSerializer
from rest_framework import generics
from rest_framework_simplejwt.tokens import RefreshToken
from django.utils.http import urlencode
from django.http import HttpResponseRedirect
from rest_framework.response import Response
from django.contrib.auth import logout
from rest_framework import status

class LogoutView(APIView):
    permission_classes = [IsAuthenticated]
    def post(self, request):
        logout(request)
        return Response({"message": "Logged out successfully."})

class RegisterView(generics.CreateAPIView):
    queryset = User.objects.all()
    serializer_class = UserSerializer
    permission_classes = [AllowAny] 

class TestProtectedView(APIView):
    permission_classes = [IsAuthenticated]
    
    def get(self, request):
        return Response({'message': 'This is a protected endpoint!'})
    
class LoginRedirectView(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request):
        user = request.user
        refresh = RefreshToken.for_user(user)
        access_token = str(refresh.access_token)
        refresh_token = str(refresh)

        query_params = urlencode({
            'access_token': access_token,
            'refresh_token': refresh_token,
            'email': user.email
        })

        return HttpResponseRedirect(f"whatmunch://oauth-redirect?{query_params}")
    
class DeleteUserView(APIView):
    permission_classes = [IsAuthenticated]

    def post(self, request):
        user = request.user
        logout(request)
        user.delete()
        return Response(status = status.HTTP_204_NO_CONTENT)