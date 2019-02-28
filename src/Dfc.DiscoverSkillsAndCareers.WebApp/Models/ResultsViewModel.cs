using System.Collections.Generic;
using Dfc.DiscoverSkillsAndCareers.Models;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public class ResultsViewModel
    {
        public string SessionId { get; set; }
        public JobFamilyResult[] JobFamilies { get; set; }
        public string[] Traits { get; set; }
        public int JobFamilyCount { get; set; }
        public int JobFamilyMoreCount { get; set; }
        public string AssessmentType { get; set; }
        public string Title { get; set; } = "National Careers Service - Results";
        public string TraitTitle { get; set; } = "What you told us";
        public string TraitSummaryText { get; set; } = "Because of your answers, we have provided the following job types. You can choose to view more results at the bottom of the page.";
        public string Headline { get; set; } = "Job categories you might be suited to";
        public string SaveProgressLinkText { get; set; } = "Save my progress";
        public string SeeLinkButtonText { get; set; } = "See job category";
        public string SaveProgressTitle { get; set; } = "Return to this later";
    }
}
