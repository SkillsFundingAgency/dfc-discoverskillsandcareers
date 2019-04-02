using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public class JobProfileResult
    {
        [JsonProperty("jobCategory")]
        public string JobCategory { get; set; }
        [JsonProperty("socCode")]
        public string SocCode { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("overview")]
        public string Overview { get; set; }
        [JsonProperty("salaryStarter")]
        public decimal SalaryStarter { get; set; }
        [JsonProperty("salaryExperienced")]
        public decimal SalaryExperienced { get; set; }
        [JsonProperty("urlName")]
        public string UrlName { get; set; }
        [JsonProperty("wydDayToDayTasks")]
        public string WYDDayToDayTasks { get; set; }
        [JsonProperty("careerPathAndProgression")]
        public string CareerPathAndProgression { get; set; }
        [JsonProperty("typicalHours")]
        public string TypicalHours { get; set; }
        [JsonProperty("shiftPattern")]
        public string ShiftPattern { get; set; }
    }
}
