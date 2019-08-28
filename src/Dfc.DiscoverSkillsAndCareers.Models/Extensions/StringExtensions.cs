using System;

namespace Dfc.DiscoverSkillsAndCareers.Models.Extensions
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string source, string target) =>
            source?.Equals(target, StringComparison.InvariantCultureIgnoreCase) ?? false;
        
        public static string FormatReferenceCode(this string code)
        {
            string result = "";
            int i = 0;
            foreach(var c in code.ToUpper().ToCharArray())
            {
                i++;
                if (i % 4 == 1 && i > 1)
                {
                    result += " ";
                }
                result += c.ToString();
            }
            return result;
        }
    }
}