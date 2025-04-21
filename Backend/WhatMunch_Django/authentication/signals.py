# from allauth.account.signals import user_signed_up
# from django.dispatch import receiver

# @receiver(user_signed_up)
# def populate_user_email(request, user, sociallogin=None, **kwargs):
#     if sociallogin:
#         user_email = sociallogin.account.extra_data.get('email')
#         if user_email:
#             user.email = user_email
#             user.save()