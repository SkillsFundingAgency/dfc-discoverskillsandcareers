using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ContentFinishPage : IContentPage
    {
        public string Title { get; set; } = "Assessment complete | National Careers Service";
        public string BreadcrumbText { get; set; } = "Assessment";
        public string Content { get; set; } = "See the types of jobs that might suit you. Your results are based on your responses.";
        public string ContentTitle { get; set; } = "What to do next";
        public string Headline { get; set; } = "Assessment complete";
        public string ViewResultsButtonText { get; set; } = "See results";
        public string HeadlineStrap { get; set; } = "Thank you";
        [JsonProperty("LastModified")]
        public DateTime LastUpdated { get; set; }
    }
}
