using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ContentFinishPage : IContentPage
    {
        public string Title { get; set; } = "National Careers Service - Finished";
        public string Content { get; set; } = "On the next page you will see your results, which are based on the statements you have just responded to.";
        public string ContentTitle { get; set; } = "What to do next";
        public string Headline { get; set; } = "Assessment complete";
        public string ViewResultsButtonText { get; set; } = "View results";
        public string HeadlineStrap { get; set; } = "Thank you";
        [JsonProperty("LastModified")]
        public DateTime LastUpdated { get; set; }
    }
}
