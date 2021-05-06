using System;
using System.Text.RegularExpressions;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public class SendSmsRequest
    {
        private const string pattern = @"^(07\d{8,12}|447\d{7,11})$";
        private const RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
        private static readonly Regex phoneNumberRegex = new Regex(pattern, options);
        
        public string MobileNumber { get; set; }
        
        public bool ValidMobileNumber => !String.IsNullOrWhiteSpace(MobileNumber) && phoneNumberRegex.IsMatch(MobileNumber);
    }
}
