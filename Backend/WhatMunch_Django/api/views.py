from decouple import config 
from rest_framework.response import Response
from rest_framework.views import APIView
from rest_framework.permissions import IsAuthenticated

class GetPlacesKeyView(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request):
        api_key = config('GOOGLE_MAPS_API_KEY', default=None)
        if not api_key:
            return Response({"error": "API key not configured."}, status=500)
        return Response({'googleMapsApiKey': api_key})

