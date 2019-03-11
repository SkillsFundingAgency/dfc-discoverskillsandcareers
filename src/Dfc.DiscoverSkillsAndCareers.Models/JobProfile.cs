using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class JobProfile
    {
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }
        [JsonProperty("id")]
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
        [JsonProperty("jobProfileCategories")]
        public string[] JobProfileCategories { get; set; }
    }
}
