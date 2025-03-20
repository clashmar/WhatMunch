using System.ComponentModel.DataAnnotations;
using WhatMunch_MAUI.Resources.Localization;

namespace WhatMunch_MAUI.Models
{
    public partial class RegistrationModel : ObservableValidator
    {
        public RegistrationModel()
        {
            ValidateProperty(Email, nameof(Email));
            EmailError = GetErrors(nameof(Email)).Select(e => e.ErrorMessage).FirstOrDefault() ?? string.Empty;

            ValidateProperty(Username, nameof(Username));
            EmailError = GetErrors(nameof(Email)).Select(e => e.ErrorMessage).FirstOrDefault() ?? string.Empty;

            ValidateProperty(Password, nameof(Password));
            EmailError = GetErrors(nameof(Email)).Select(e => e.ErrorMessage).FirstOrDefault() ?? string.Empty;

            ValidateProperty(ConfirmPassword, nameof(ConfirmPassword));
            EmailError = GetErrors(nameof(Email)).Select(e => e.ErrorMessage).FirstOrDefault() ?? string.Empty;
        }

        [ObservableProperty]
        [Required(ErrorMessageResourceName = "EmailRequiredError", ErrorMessageResourceType = typeof(AppResources))]
        [RegularExpression(@"^(?![_.-])[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", ErrorMessageResourceName = "EmailFormatError", ErrorMessageResourceType = typeof(AppResources))]
        public string _email = "";
        
        partial void OnEmailChanged(string value)
        {
            ValidateProperty(value, nameof(Email));
            EmailError = GetErrors(nameof(Email)).Select(e => e.ErrorMessage).FirstOrDefault() ?? string.Empty;
        }

        [ObservableProperty]
        public string _emailError = "";

        [ObservableProperty]
        [Required(ErrorMessageResourceName = "UsernameRequiredError", ErrorMessageResourceType = typeof(AppResources))]
        [MinLength(3, ErrorMessageResourceName = "UsernameMinLengthError", ErrorMessageResourceType = typeof(AppResources))]
        [MaxLength(150, ErrorMessageResourceName = "UsernameMaxLengthError", ErrorMessageResourceType = typeof(AppResources))]
        [RegularExpression(@"^[a-zA-Z0-9@.+\-_]+$", ErrorMessageResourceName = "UsernameFormatError", ErrorMessageResourceType = typeof(AppResources))]
        public string _username = "";

        partial void OnUsernameChanged(string value)
        {
            ValidateProperty(value, nameof(Username));
            UsernameError = GetErrors(nameof(Username)).Select(e => e.ErrorMessage).FirstOrDefault() ?? string.Empty;
        }

        [ObservableProperty]
        public string _usernameError = "";

        [ObservableProperty]
        [Required(ErrorMessageResourceName = "PasswordRequiredError", ErrorMessageResourceType = typeof(AppResources))]
        [MinLength(8, ErrorMessageResourceName = "PasswordMinLengthError", ErrorMessageResourceType = typeof(AppResources))]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessageResourceName = "PasswordFormatError", ErrorMessageResourceType = typeof(AppResources))]
        public string _password = "";

        partial void OnPasswordChanged(string value)
        {
            ValidateProperty(value, nameof(Password));
            PasswordError = GetErrors(nameof(Password)).Select(e => e.ErrorMessage).FirstOrDefault() ?? string.Empty;
        }

        [ObservableProperty]
        public string _passwordError = "";

        [ObservableProperty]
        [Required(ErrorMessageResourceName = "PasswordRequiredError", ErrorMessageResourceType = typeof(AppResources))]
        [MinLength(8, ErrorMessageResourceName = "PasswordMinLengthError", ErrorMessageResourceType = typeof(AppResources))]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessageResourceName = "PasswordFormatError", ErrorMessageResourceType = typeof(AppResources))]
        [CustomValidation(typeof(RegistrationModel), nameof(ValidatePasswordsMatch))]
        public string _confirmPassword = "";

        partial void OnConfirmPasswordChanged(string value)
        {
            ValidateProperty(value, nameof(ConfirmPassword));
            ConfirmPasswordError = GetErrors(nameof(ConfirmPassword)).Select(e => e.ErrorMessage).FirstOrDefault() ?? string.Empty;
        }

        [ObservableProperty]
        public string _confirmPasswordError = "";

        public bool IsValid()
        {
            ValidateAllProperties();
            return !HasErrors;        
        }

        public static ValidationResult ValidatePasswordsMatch(string? confirmPassword, ValidationContext context)
        {
            if(context.ObjectInstance is RegistrationModel registrationModel)
            {
                if (!string.IsNullOrEmpty(confirmPassword) && registrationModel.Password != confirmPassword)
                {
                    return new(AppResources.PasswordMatchError);
                }
            }
            return ValidationResult.Success!;
        }
    }
}
