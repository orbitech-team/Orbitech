using System.Security.Cryptography;
using System.Text;

namespace OrbitechWeb.Utils
{
    public static class PasswordHelper
    {
        public static string HashPassword(string plainPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(plainPassword);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static bool VerifyPassword(string plainPassword, string storedHash)
        {
            return HashPassword(plainPassword) == storedHash;
        }
    }
}
