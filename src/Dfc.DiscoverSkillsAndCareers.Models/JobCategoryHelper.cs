namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public static class JobCategoryHelper
    {
        public static string GetCode(string input)
        {
            string code = "";
            var words = input.Split(" ");
            if (words.Length > 1)
            {
                for (var i = 0; i < words.Length; i++)
                {
                    code += words[i].Substring(0, 1).ToUpper();
                }
            }
            else
            {
                code = input.Substring(0, 5).ToUpper();
            }
            return code;
        }
    }
}