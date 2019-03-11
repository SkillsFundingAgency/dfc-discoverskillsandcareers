using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ContentIndexPage : IContentPage
    {
        public string Title { get; set; } = "National Careers Service";
        public string Headline { get; set; } = "Discover your skills and careers";
        public string Subheading { get; set; } = "Take a quick assessment to see job categories you might be suited to. Or take a longer assessment to see specific job roles.";
        public string Assessment1Title { get; set; } = "See job categories";
        public string Assessment1Subtitle { get; set; } = "5 minutes";
        public string Assessment2Title { get; set; } = "See specific job roles";
        public string Assessment2Subtitle { get; set; } = "10 to 15 minutes";
        public string ResumeTitle { get; set; } = "Resume your progress";
        public string ResumeFieldTitle { get; set; } = "Reference number";
        public string ResumeButtonText { get; set; } = "Resume progress";
        public string ResumeErrorMessage { get; set; } = "The code could not be found";
        [JsonProperty("LastModified")]
        public DateTime LastUpdated { get; set; }
    }
}
