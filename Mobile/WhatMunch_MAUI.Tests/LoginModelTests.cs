using WhatMunch_MAUI.Models;

namespace WhatMunch_MAUI.Tests
{
    public class LoginModelTests
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
            var model = new LoginModel() { Username = username, Password = "Password1" };

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
            var model = new LoginModel() { Username = username, Password = "password" };

            // Act
            bool isValid = model.IsValid();

            // Assert
            Assert.False(isValid);
        }

        [Theory]
        [InlineData("Password97")]
        [InlineData("PASSWORd97")]
        [InlineData("1Pp2assword")]
        [InlineData("passworD97!")]
        [InlineData("pP1@/.,-)(*&^%$£\"!")]
        public void IsValid_ValidPasswords_ReturnTrue(string password)
        {
            // Arrange
            var model = new LoginModel() { Username = "username", Password = password };

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
            var model = new LoginModel() { Username = "username", Password = password };

            // Act
            bool isValid = model.IsValid();

            // Assert
            Assert.False(isValid);
        }
    }
}
