using EasyIdentity.Core;
using System;
using Xunit;
using static LanguageExt.Prelude;

namespace UserManagement.Test
{
    public class UserTest
    {
        [Fact]
        public void SignUpSuccess()
        {
            var userEither = User.TrySignUp("severin.nauer@gmail.com", "Test1234!", "testname");
            Assert.True(userEither.IsRight);
            userEither.IfRight(user =>
            {
                Assert.Equal("severin.nauer@gmail.com", user.Email);
                Assert.Equal("Test1234!", user.Password);
                user.Username.IfSome(username =>
                {
                    Assert.Equal("testname", username);
                });
            });
        }

        [Fact]
        public void SignupNoUsername()
        {
            var userEither = User.TrySignUp("severin.nauer@gmail.com", "Test1234!", None);
            Assert.True(userEither.IsRight);
            userEither.IfRight(user =>
            {
                Assert.True(user.Username.IsNone);
            });
        }

        [Fact]
        public void ChangePassword()
        {
            var userEither = User.TrySignUp("severin.nauer@gmail.com", "Test1234!", "testname");
            userEither.IfRight(user =>
            {
                Assert.Equal("Test1234!", user.Password);
                var password = Password.TryCreate("!1234Test");
                password.IfRight(pw =>
                {
                    var newUser = user.ChangePassword(pw);
                    Assert.Equal("!1234Test", newUser.Password);
                });
            });
        }

        [Fact]
        public void CreateUserInvalidEmail()
        {
            var userEither = User.TrySignUp("", "Test1234!", "testname");
            Assert.True(userEither.IsLeft);
            userEither.IfLeft(error =>
            {
                Assert.True(error is EmailAddress.NoEmptyEmail);
            });

            var userEither2 = User.TrySignUp("test.test.com", "Test12345!", None);
            Assert.True(userEither2.IsLeft);
            userEither2.IfLeft(error =>
            {
                Assert.True(error is EmailAddress.NoAtProvided);
            });
        }

        [Fact]
        public void CreateUserInvalidUsername()
        {
            var userEither = User.TrySignUp("severin.nauer@gmail.com", "Test1234!", "");
            Assert.True(userEither.IsLeft);
            userEither.IfLeft(error =>
            {
                Assert.True(error is Username.UsernameTooShort);
            });
        }

        [Fact]
        public void VerifyEmail()
        {
            var userMaybe = User.TrySignUp("test@test.com", "Test1234!", None);
            userMaybe.IfRight(user =>
            {
                var verifiedUser = user.VerifyEmail();
                Assert.False(user.EmailIsVerified);
                Assert.True(verifiedUser.EmailIsVerified);
            });
        }
    }
}