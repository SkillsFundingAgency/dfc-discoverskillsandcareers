﻿using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ContentQuestionPage : IContentPage
    {
        public string Title { get; set; } = "National Careers Service - Statement";
        public string SaveProgressLinkText { get; set; } = "Save my progress";
        public string StronglyDisagree { get; set; } = "Strongly disagree";
        public string StronglyAgree { get; set; } = "Strongly agree";
        public string SaveProgressTitle { get; set; } = "Return to this later";
        public string Neutral { get; set; } = "This doesn't apply to me";
        public string Agree { get; set; } = "Agree";
        public string NextButtonText { get; set; } = "Next";
        public string Disagree { get; set; } = "Disagree";
        [JsonProperty("LastModified")]
        public DateTime LastUpdated { get; set; }
    }
}
