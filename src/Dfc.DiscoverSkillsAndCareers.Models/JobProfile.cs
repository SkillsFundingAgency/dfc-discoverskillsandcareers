using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class JobProfile
    {
        public string SocCode { get; set; }

        public string Title { get; set; }

        public string Overview { get; set; }

        public decimal SalaryStarter { get; set; }

        public decimal SalaryExperienced { get; set; }

        public string UrlName { get; set; }

        public string WYDDayToDayTasks { get; set; }

        public string CareerPathAndProgression { get; set; }

        public string[] JobProfileCategories { get; set; }

        public int MinimumHours { get; set; }

        public int MaximumHours { get; set; }

        public string TypicalHours => $"{MinimumHours} to {MaximumHours}";

        public string[] WorkingHoursDetails { get; set; }

        public string[] WorkingPattern { get; set; }

        public string[] WorkingPatternDetails { get; set; }
    }
}
        