from django.apps import AppConfig
from django.http import HttpResponseRedirect

class AuthenticationConfig(AppConfig):
    default_auto_field = 'django.db.models.BigAutoField'
    name = 'authentication'

    def ready(self):
        custom_scheme = 'whatmunch'
        if custom_scheme not in HttpResponseRedirect.allowed_schemes:
            HttpResponseRedirect.allowed_schemes.append(custom_scheme)