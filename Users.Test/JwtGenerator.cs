using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Users.Test.Unit
{
    public class JwtGenerator
    {
        private const string TestSecret = "This_Is_A_Secret_For_Testing";

        private SymmetricSecurityKey Key => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TestSecret));

        private User CreateUser()
        {
            return new UserModel
            {
                EmailAddress = "test@test.ch",
                EmailVerificationState = EmailVerificationState.Unverified,
                Password = "Password123",
                ProjectId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Username = "testUsername"
            }.ToDomain().Match(user => user, _ => throw new Exception());
        }

        private string GenerateToken(User user)
        {
            var generator = new Users.JwtGenerator(TestSecret, user);
            return generator.Generate();
        }

        [Fact]
        public void TokenIsValid()
        {
            var jwtToken = GenerateToken(CreateUser());
            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TestSecret));
            handler.ValidateToken(jwtToken, new TokenValidationParameters()
            {
                IssuerSigningKey = key,
                ValidateAudience = false,
                ValidateIssuerSigningKey = false,
                ValidateIssuer = false
                //TODO better validate
            }, out var securityToken);

            Assert.NotNull(securityToken);
        }

        private Claim? GetClaim(JwtSecurityToken token, string claimType)
            => token.Claims.FirstOrDefault(claim => claim.Type == claimType);

        [Fact]
        public void ClaimsAreValid()
        {
            var user = CreateUser();
            var jwtToken = GenerateToken(user);
            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.ReadJwtToken(jwtToken);

            Assert.Equal(user.EmailAddress, GetClaim(securityToken, "email")?.Value);
            Assert.Equal(user.UserId.Value.ToString(), securityToken.Subject);
        }
    }
}