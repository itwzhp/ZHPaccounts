using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Zhp.Office.AccountManagement.Domain.Services
{
    public interface IPasswordGenerator
    {
        string GeneratePassword();
    }

    public class PasswordGenerator : IPasswordGenerator
    {
        private const int length = 8;
        private readonly RandomNumberGenerator generator;

        public PasswordGenerator(RandomNumberGenerator generator)
        {
            this.generator = generator;
        }

        public string GeneratePassword()
        {
            string password;

            do
            {
                password = GeneratePasswordPropose();
            }
            while (!IsPasswordSafe(password));

            return password;
        }

        // based on https://stackoverflow.com/a/38997554
        private string GeneratePasswordPropose()
        {
            var password = new StringBuilder(length);

            byte[] byteBuffer = new byte[length];
            generator.GetBytes(byteBuffer);

            for (int i = 0; i < length; i++)
            {
                int randomValue = byteBuffer[i] % (10 + 26 + 26);

                if (randomValue < 10)
                    password.Append((char)('0' + randomValue));
                else if (randomValue < 36)
                    password.Append((char)('A' + randomValue - 10));
                else
                    password.Append((char)('a' + randomValue - 36));
            }
            return password.ToString();
        }

        private bool IsPasswordSafe(string password) => password.Any(char.IsDigit) && password.Any(char.IsLower) && password.Any(char.IsUpper);
    }
}
