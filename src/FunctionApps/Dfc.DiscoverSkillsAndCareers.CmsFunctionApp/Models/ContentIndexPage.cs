using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ContentIndexPage : IContentPage
    {
        public string Title { get; set; } = "Discover your skills and careers | National Careers Service";
        public string Headline { get; set; } = "Discover your skills and careers";
        public string TextBlock1 { get; set; } = "Take this assessment to find out what types of jobs might suit you, for example \"retail and sales\".";
        public string TextBlock2 { get; set; } = "Answer a few more questions to find out what specific job roles might suit you, for example \"florist\".";
        public string TextHighlight { get; set; } = "This could take 5 to 10 minutes. It will take longer if you’re using assistive technologies, for example a screen reader or screen magnifier.";
        public string TextBlock3 { get; set; } = "You can save your assessment if you want to complete it later, or look at your results again another time.";
        public string ResumeTitle { get; set; } = "Return to an assessment";
        public string ResumeFieldTitle { get; set; } = "Enter your reference";
        public string ResumeButtonText { get; set; } = "Continue";
        public string ResumeErrorMessage { get; set; } = "The code could not be found";
        public string MissingCodeErrorMessage { get; set; } = "Please enter your reference";
        [JsonProperty("LastModified")]
        public DateTimeOffset LastUpdated { get; set; }
    }
}
