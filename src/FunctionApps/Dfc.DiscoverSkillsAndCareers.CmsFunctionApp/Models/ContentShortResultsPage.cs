using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ContentShortResultsPage : IContentPage
    {
        public string Title { get; set; } = "National Careers Service - Results";
        [JsonProperty("SummaryTitleText")]
        public string TraitTitle { get; set; } = "What you told us";
        [JsonProperty("SummaryText")]
        public string TraitSummaryText { get; set; } = "Because of your answers, we have provided the following job types. You can choose to view more results at the bottom of the page.";
        public string Headline { get; set; } = "Job categories you might be suited to";
        public string SaveProgressLinkText { get; set; } = "Save my progress";
        public string SeeLinkButtonText { get; set; } = "See job category";
        public string SaveProgressTitle { get; set; } = "Return to this later";
        [JsonProperty("LastModified")]
        public DateTime LastUpdated { get; set; }
    }
}
