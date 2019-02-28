using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ContentStartPage : IContentPage
    {
        public string Title { get; set; } 
        public string Headline { get; set; } 
        public string Subheading { get; set; } 
        public string StartNowButtontext { get; set; } 
        public string HighlightText { get; set; } 
        public string Content { get; set; }
        [JsonProperty("LastModified")]
        public DateTime LastUpdated { get; set; }
    }
}
