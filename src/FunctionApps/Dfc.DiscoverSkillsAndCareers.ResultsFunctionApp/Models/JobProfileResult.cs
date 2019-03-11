using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.ResultsFunctionApp.Models
{
    public class JobProfileResult
    {
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
    }
}
