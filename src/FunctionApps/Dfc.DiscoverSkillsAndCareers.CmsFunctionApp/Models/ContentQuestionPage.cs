using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ContentQuestionPage : IContentPage
    {
        public string Title { get; set; } = "National Careers Service - Assessment Statement";
        public string Breadcrumb { get; set; } = "Assessment";
        public string SaveProgressText { get; set; } = "Save my progress";
        public string StronglyDisagreeText { get; set; } = "Strongly disagree";
        public string StronglyAgreeText { get; set; } = "Strongly agree";
        public string SaveProgressTitle { get; set; } = "Return to this later";
        public string NeutralText { get; set; } = "It depends";
        public string AgreeText { get; set; } = "Agree";
        public string NextButtonText { get; set; } = "Next";
        public string DisagreeText { get; set; } = "Disagree";
        public string YesText { get; set; } = "Yes";
        public string NoText { get; set; } = "No";
        public string NoAnswerErrorMessage { get; set; } = "Please select an option below to continue";

        [JsonProperty("LastModified")]
        public DateTime LastUpdated { get; set; }
    }
}
