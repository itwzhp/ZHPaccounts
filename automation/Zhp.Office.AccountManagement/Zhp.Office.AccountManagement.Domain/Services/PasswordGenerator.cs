using System;
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
        private readonly RandomNumberGenerator generator;

        public PasswordGenerator(RandomNumberGenerator generator)
        {
            this.generator = generator;
        }

        // based on https://stackoverflow.com/a/38997554
        public string GeneratePassword()
        {
            const int length = 8;
            const string punctuations = "!@#$%^&*()_-+=[{]}:|./?";

            var byteBuffer = new byte[length];

            generator.GetBytes(byteBuffer);

            bool hasLowerCase = false;
            bool hasUpperCase = false;
            bool hasDigit = false;

            var password = new StringBuilder(length + 3);

            for (var i = 0; i < length; i++)
            {
                var randomValue = byteBuffer[i] % (10 + 26 + 26 + punctuations.Length);

                if (randomValue < 10)
                {
                    password.Append((char)('0' + randomValue));
                    hasDigit = true;
                }
                else if (randomValue < 36)
                {
                    password.Append((char)('A' + randomValue - 10));
                    hasUpperCase = true;
                }
                else if (randomValue < 62)
                {
                    password.Append((char)('a' + randomValue - 36));
                    hasLowerCase = true;
                }
                else
                    password.Append(punctuations[randomValue - 62]);
            }

            var random = new Random();

            if (!hasDigit)
                password.Insert(random.Next(password.Length), '0' + random.Next(10));

            if (!hasLowerCase)
                password.Insert(random.Next(password.Length), 'a' + random.Next(26));

            if (!hasUpperCase)
                password.Insert(random.Next(password.Length), 'A' + random.Next(26));

            return password.ToString();
        }
    }
}
