import os
from rest_framework.response import Response
from rest_framework.views import APIView
from rest_framework.permissions import IsAuthenticated
import logging

logger = logging.getLogger(__name__)

class GetPlacesKeyView(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request):
        api_key = os.getenv('GOOGLE_MAPS_API_KEY', default=None)
        if not api_key:
            logger.error("GOOGLE_MAPS_API_KEY is missing in environment")
            return Response({"error": "API key not configured."}, status=500)
        
        logger.info(f"API key found: {api_key[:6]}****")
        return Response({'googleMapsApiKey': api_key})

