using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ContentInformationSourcesPage : IContentPage
    {
        public string Title { get; set; } 
        public string InformationSource1Heading { get; set; } 
        public string InformationSource1Description { get; set; } 
        public string Feedback1 { get; set; } 
        public string FeedbackFormLink { get; set; } 
        public string FeedbackFormLinkText { get; set; } 
        public string Feedback2 { get; set; } 

        [JsonProperty("LastModified")]
        public DateTime LastUpdated { get; set; }
    }
}
