using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ContentQuestionPage : IContentPage
    {
        public string Title { get; set; }
        public string Breadcrumb { get; set; }
        public string SaveProgressText { get; set; }
        public string StronglyDisagreeText { get; set; }
        public string StronglyAgreeText { get; set; } 
        public string SaveProgressTitle { get; set; }
        public string NeutralText { get; set; }
        public string AgreeText { get; set; }
        public string NextButtonText { get; set; }
        public string DisagreeText { get; set; }
        public string YesText { get; set; }
        public string NoText { get; set; }
        public string NoAnswerErrorMessage { get; set; } 

        [JsonProperty("LastModified")]
        public DateTime LastUpdated { get; set; }
    }
}
