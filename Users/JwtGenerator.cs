using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Users
{
    internal interface ITokenGenerator
    {
        string Generate();
    }

    public class JwtGenerator : ITokenGenerator
    {
        private readonly User user;
        private readonly string secret;

        public JwtGenerator(string secret, User user)
        {
            this.secret = secret;
            this.user = user;
        }

        private SecurityTokenDescriptor CreateDescriptor(byte[] key) => 
            new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new List<Claim>() { new Claim(ClaimTypes.Email, user.EmailAddress), new Claim("sub", user.UserId.Value.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

        public string Generate()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(secret);
            var descriptor = CreateDescriptor(key);
            return tokenHandler.CreateEncodedJwt(descriptor);
        }
    }
}