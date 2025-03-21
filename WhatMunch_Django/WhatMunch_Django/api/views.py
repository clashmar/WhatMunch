from rest_framework.views import APIView
from rest_framework.response import Response
from rest_framework import status
from rest_framework.permissions import IsAuthenticated
from .data.dummy_data import RESTAURANTS

class RestaurantListView(APIView):
    permission_classes = [IsAuthenticated]
    
    def get(self, request):
        return Response(RESTAURANTS, status=status.HTTP_200_OK)
