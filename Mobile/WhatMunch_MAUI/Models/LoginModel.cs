using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Username is required.")]
        [MinLength(3, ErrorMessage = "Must be at least 3 characters.")]
        [MaxLength(150, ErrorMessage = "Must be less than 150 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9@.+\-_]+$", ErrorMessage = "Letters, digits and @/./+/-/_ only.")]
        public string _username = "";

        partial void OnUsernameChanged(string value)
        {
            ValidateProperty(value, nameof(Username));
            UsernameError = GetErrors(nameof(Username)).Select(e => e.ErrorMessage).FirstOrDefault() ?? string.Empty;
        }

        [ObservableProperty]
        public string _usernameError = "";

        [ObservableProperty]
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessage = "Must contain at least one lowercase, uppercase and numeric character.")]
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
