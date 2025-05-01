import logging
from django.http import HttpRequest, HttpResponse

logger = logging.getLogger(__name__)
handler = logging.StreamHandler()
formatter = logging.Formatter(fmt="%(asctime)s %(levelname)s; %(message)s")
handler.formatter = formatter
logger.addHandler(handler)
logger.setLevel(logging.INFO)

class LoggingMiddleware:
    def __init__(self, get_response):
        self.get_response = get_response

    def __call__(self, request: HttpRequest):
        request_data = {
            'method': request.method,
            'ip_address': request.META.get('REMOTE_ADDR'),
            'path': request.path,
            'body': request.body    
        }
        logger.info(request_data)

        response: HttpResponse = self.get_response(request)

        #logic executed after the View is called:

        response_dict = {
            'status_code': response.status_code,
            'headers': response.headers,
        }
        logger.info(response_dict)

        return response