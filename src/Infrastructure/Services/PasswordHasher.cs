using Domain.Interfaces;
using BCrypt.Net;
namespace Infrastructure.Services
{
    public sealed class PasswordHasher : IPasswordHasher
    {

        public string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("The password cannot be blank", nameof(password));

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }

}
