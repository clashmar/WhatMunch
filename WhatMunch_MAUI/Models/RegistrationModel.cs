using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string _email = "";
        
        partial void OnEmailChanged(string value)
        {
            ValidateProperty(value, nameof(Email));
            EmailError = GetErrors(nameof(Email)).Select(e => e.ErrorMessage).FirstOrDefault() ?? string.Empty;
        }

        [ObservableProperty]
        public string _emailError = "";

        [ObservableProperty]
        [Required(ErrorMessage = "Username is required.")]
        [MinLength(3, ErrorMessage = "Must be at least 3 characters.")]
        [MaxLength(150, ErrorMessage = "Must be less than 150 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9@.+-_]+$", ErrorMessage = "Letters, digits and @/./+/-/_ only.")]
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
        public string _password = "";

        partial void OnPasswordChanged(string value)
        {
            ValidateProperty(value, nameof(Password));
            PasswordError = GetErrors(nameof(Password)).Select(e => e.ErrorMessage).FirstOrDefault() ?? string.Empty;
        }

        [ObservableProperty]
        public string _passwordError = "";

        [ObservableProperty]
        [Required(ErrorMessage = "Password must be confirmed.")]
        [MinLength(8, ErrorMessage = "Must be at least 8 characters.")]
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
                    return new("Passwords do not match.");
                }
            }
            return ValidationResult.Success!;
        }
    }
}
