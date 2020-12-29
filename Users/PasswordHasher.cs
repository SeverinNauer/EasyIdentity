using System;

namespace Users
{
    public interface IPasswordHasher
    {
        string Hash(string cleartextPassword);

        bool Verify(string clearTextPassword, string encryptedPassword);
    }

    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string cleartextPassword) 
            => BCrypt.Net.BCrypt.HashPassword(cleartextPassword);

        public bool Verify(string clearTextPassword, string encryptedPassword) 
            => BCrypt.Net.BCrypt.Verify(clearTextPassword, encryptedPassword);
    }
}