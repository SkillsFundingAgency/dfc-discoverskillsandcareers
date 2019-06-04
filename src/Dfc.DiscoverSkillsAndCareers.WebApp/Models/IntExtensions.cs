namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public static class IntExtensions
    {
        public static string ToQuestionPageNumber(this int questionNumber)
        {
            if (questionNumber < 10) return $"0{questionNumber.ToString()}";
            return questionNumber.ToString();
        }
    }
}