using Dfc.DiscoverSkillsAndCareers.Models;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public class ResultsViewModel : BaseViewModel
    {
        public string SessionId { get; set; }
        public string Code { get; set; }
        public JobCategoryResult[] JobCategories { get; set; }
        public string[] Traits { get; set; }
        public int JobFamilyCount { get; set; }
        public int JobFamilyMoreCount { get; set; }
        public string AssessmentType { get; set; }
        public bool UseFilteringQuestions { get; set; }
        public JobProfileResult[] JobProfiles { get; set; }
        public string[] WhatYouToldUs { get; set; }

        public string GetJobCategoryForNumberText(int number)
        {
            if (number == 1) return $"{number} job category";
            return $"{number} job categories";
        }

        public string ExploreCareersBaseUrl { get; set; } = "https://nationalcareers.service.gov.uk";
    }
}