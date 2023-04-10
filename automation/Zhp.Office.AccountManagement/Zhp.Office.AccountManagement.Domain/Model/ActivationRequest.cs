using System.Text.RegularExpressions;

namespace Zhp.Office.AccountManagement.Model
{
    public class ActivationRequest
    {
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string membershipNumber = string.Empty;

        public string Id { get; set; } = string.Empty;
        public string FirstName { get => firstName; set => firstName = CleanName(value); }
        public string LastName { get => lastName; set => lastName = CleanName(value); }
        public string MembershipNumber { get => membershipNumber; set => membershipNumber = Regex.Replace(value, @"\W", string.Empty).ToUpper(); }
        /// <summary>
        /// Full name - "Hufiec Sopot", "Chorągiew Stołeczna" or "Główna Kwatera ZHP"
        /// </summary>
        public string FirstLevelUnit { get; set; } = string.Empty;

        /// <summary>
        /// Full name - "Chorągiew Stołeczna" or "Główna Kwatera ZHP"
        /// </summary>
        public string SecondLevelUnit { get; set; } = string.Empty;

        private static string CleanName(string input)
        {
            input = input.Trim();

            input = Regex.Replace(input, @"\s{1,}", " ");

            input = Regex.Replace(input, @"\w{1,}", m => m.Value[0..1].ToUpper() + m.Value[1..].ToLower());

            return input;
        }
    }
}
