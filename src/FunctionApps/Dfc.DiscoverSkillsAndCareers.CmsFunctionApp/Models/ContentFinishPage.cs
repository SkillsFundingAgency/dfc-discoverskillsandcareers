using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ContentFinishPage : IContentPage
    {
        public string Title { get; set; } 
        public string BreadcrumbText { get; set; } 
        public string Content { get; set; } 
        public string ContentTitle { get; set; } 
        public string Headline { get; set; } 
        public string ViewResultsButtonText { get; set; } 
        public string HeadlineStrap { get; set; } 
        [JsonProperty("LastModified")]
        public DateTime LastUpdated { get; set; }
    }
}
