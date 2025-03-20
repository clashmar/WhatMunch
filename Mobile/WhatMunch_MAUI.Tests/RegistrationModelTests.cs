using WhatMunch_MAUI.Models;

namespace WhatMunch_MAUI.Tests
{
    public class RegistrationModelTests
    {
        [Theory]
        [InlineData("username")]
        [InlineData("username1997")]
        [InlineData("username.user")]
        [InlineData("username@user")]
        [InlineData("username_user")]
        [InlineData("username-user")]
        [InlineData("username+user")]
        public void IsValid_ValidUsernames_ReturnTrue(string username)
        {
            // Arrange
            var model = new RegistrationModel() { Email = "user@email.com", Username = username, Password = "Password1", ConfirmPassword = "Password1" };

            // Act
            bool isValid = model.IsValid();

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("97")]
        [InlineData("usernameusernameusernameusernameusernameusernameusernameusernameusernameusernameusernameusernameusernameusernameusernameusernameusernameusernameusernameusernameusername")]
        [InlineData("username>user")]
        [InlineData("username?user")]
        [InlineData("username#user")]
        [InlineData("username/user")]
        [InlineData("username$user")]
        [InlineData("username=user")]
        [InlineData("\"usernameuser\"")]
        [InlineData("u!s£e$r%n^a%m*")]
        public void IsValid_InvalidUsernames_ReturnFalse(string username)
        {
            // Arrange
            var model = new RegistrationModel() { Email = "user@email.com", Username = username, Password = "Password1", ConfirmPassword = "Password1" };

            // Act
            bool isValid = model.IsValid();

            // Assert
            Assert.False(isValid);
        }

        [Theory]
        [InlineData("user@email.com")]
        [InlineData("user@email.co.uk")]
        [InlineData("first.last@email.com")]
        [InlineData("user1234@email.biz")]
        [InlineData("user_name@email.com")]
        public void IsValid_ValidEmails_ReturnTrue(string email)
        {
            // Arrange
            var model = new RegistrationModel() { Email = email, Username = "username", Password = "Password1", ConfirmPassword = "Password1" };

            // Act
            bool isValid = model.IsValid();

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("user@email@com")]
        [InlineData("@email")]
        [InlineData("user@.com")]
        [InlineData("user@com")]
        [InlineData("user@domain,com")]
        public void IsValid_InvalidEmails_ReturnFalse(string email)
        {
            // Arrange
            var model = new RegistrationModel() { Email = email, Username = "username", Password = "Password1", ConfirmPassword = "Password1" };

            // Act
            bool isValid = model.IsValid();

            // Assert
            Assert.False(isValid);
        }

        [Theory]
        [InlineData("Password1")]
        [InlineData("Password97")]
        [InlineData("PASSWORd97")]
        [InlineData("1Pp2assword")]
        [InlineData("passworD97!")]
        [InlineData("pP1@/.,-)(*&^%$£\"!")]
        public void IsValid_ValidPassword_ReturnTrue(string password)
        {
            // Arrange
            var model = new RegistrationModel() { Email = "user@email.com", Username = "username", Password = password, ConfirmPassword = password };

            // Act
            bool isValid = model.IsValid();

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("pa")]
        [InlineData("12345678")]
        [InlineData("!!!!!!!!")]
        [InlineData("password")]
        [InlineData("password97")]
        [InlineData("password97!")]
        [InlineData("PASSWORD97!")]
        public void IsValid_InvalidPasswords_ReturnFalse(string password)
        {
            // Arrange
            var model = new RegistrationModel() { Email = "user@email.com", Username = "username", Password = password, ConfirmPassword = password };

            // Act
            bool isValid = model.IsValid();

            // Assert
            Assert.False(isValid);
        }

        [Theory]
        [InlineData("password1")]
        [InlineData("Password97")]
        [InlineData("PASSWORd97")]
        [InlineData("1Pp2assword")]
        [InlineData("passworD97!")]
        [InlineData("pP1@/.,-)(*&^%$£\"!")]
        public void IsValid_NonMatchingPasswords_ReturnFalse(string password)
        {
            // Arrange
            var model = new RegistrationModel() { Email = "user@email.com", Username = "username", Password = password, ConfirmPassword = "Password1" };

            // Act
            bool isValid = model.IsValid();

            // Assert
            Assert.False(isValid);
        }
    }
}
