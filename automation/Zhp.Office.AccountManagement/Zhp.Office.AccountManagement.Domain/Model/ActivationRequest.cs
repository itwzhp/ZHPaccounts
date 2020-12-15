namespace Zhp.Office.AccountManagement.Model
{
    public class ActivationRequest
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string MembershipNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// Full name - "Hufiec Sopot", "Chorągiew Stołeczna" or "Główna Kwatera ZHP"
        /// </summary>
        public string FirstLevelUnit { get; set; } = string.Empty;

        /// <summary>
        /// Full name - "Chorągiew Stołeczna" or "Główna Kwatera ZHP"
        /// </summary>
        public string SecondLevelUnit { get; set; } = string.Empty;
    }
}
