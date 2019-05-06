using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ContentInformationSourcesPage : IContentPage
    {
        public string Title { get; set; } = "Information Sources | National Careers Service";
        public string InformationSource1Heading { get; set; } = "Information sources";
        public string InformationSource1Description { get; set; } = "Our occupational psychology subject matter experts";
        public string Feedback1 { get; set; } = "We worked with subject matter experts, SHL Group Limited, to create the 'Discover your skills and careers' service.";
        public string FeedbackFormLink { get; set; } = "https://www.smartsurvey.co.uk/s/discover-skills-careers/";
        public string FeedbackFormLinkText { get; set; } = "this form";
        public string Feedback2 { get; set; } = "We're constantly working to improve our information and your feedback is important to us. Please use";

        [JsonProperty("LastModified")]
        public DateTimeOffset LastUpdated { get; set; }
    }
}
