using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class DfcJobProfile
    {
        [JsonProperty("SocCode")]
        public string SocCode { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("Overview")]
        public string Overview { get; set; }
        [JsonProperty("SalaryStarter")]
        public decimal SalaryStarter { get; set; }
        [JsonProperty("SalaryExperienced")]
        public decimal SalaryExperienced { get; set; }
        [JsonProperty("UrlName")]
        public string UrlName { get; set; }
        [JsonProperty("WYDDayToDayTasks")]
        public string WYDDayToDayTasks { get; set; }
        [JsonProperty("CareerPathAndProgression")]
        public string CareerPathAndProgression { get; set; }
        [JsonProperty("typicalHours")]
        public string TypicalHours { get; set; }
        [JsonProperty("shiftPattern")]
        public string ShiftPattern { get; set; }
        [JsonProperty("JobProfileCategories")]
        public string[] JobProfileCategories { get; set; }
    }
}
