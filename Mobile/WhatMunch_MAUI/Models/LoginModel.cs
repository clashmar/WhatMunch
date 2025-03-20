using System.ComponentModel.DataAnnotations;
using WhatMunch_MAUI.Resources.Localization;

namespace WhatMunch_MAUI.Models
{
    public partial class LoginModel : ObservableValidator
    {
        public LoginModel()
        {
            ValidateProperty(Username, nameof(Username));
            UsernameError = GetErrors(nameof(Username)).Select(e => e.ErrorMessage).FirstOrDefault() ?? string.Empty;

            ValidateProperty(Password, nameof(Password));
            PasswordError = GetErrors(nameof(Password)).Select(e => e.ErrorMessage).FirstOrDefault() ?? string.Empty;
        }

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

        public bool IsValid()
        {
            ValidateAllProperties();
            return !HasErrors;
        }
    }
}
