using System.Collections.Generic;
using Dfc.DiscoverSkillsAndCareers.Models;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public class ResultsViewModel
    {
        public string SessionId { get; set; }
        public string Code { get; set; }
        public JobFamilyResult[] JobFamilies { get; set; }
        public string[] Traits { get; set; }
        public int JobFamilyCount { get; set; }
        public int JobFamilyMoreCount { get; set; }
        public string AssessmentType { get; set; }
        public string Title { get; set; } 
        public string TraitTitle { get; set; } 
        public string TraitSummaryText { get; set; } 
        public string Headline { get; set; } 
        public string SaveProgressText { get; set; } 
        public string SeeLinkButtonText { get; set; } 
        public string SaveProgressTitle { get; set; } 
        public bool UseFilteringQuestions { get; set; }
        public JobProfileResult[] JobProfiles { get; set; }
        public string[] WhatYouToldUs { get; set; }
        public string StartFilterQuestionsButtonText { get; set; } 
        public string ViewResultsButtonText { get; set; } 
        public string WhatYouToldUsTitle { get; set; } 
        public string JobProfilesTitle { get; set; } 
        public string JobProfileTextSingle { get; set; } 
        public string JobProfileTextPlural { get; set; } 
        public string ThatMightSuitText { get; set; } 
        public string AverageSalaryText { get; set; } 
        public string StarterText { get; set; } 
        public string ExperiencedText { get; set; } 
        public string TypicalHoursText { get; set; } 
        public string HoursDetailsText { get; set; }
        public string YouCouldWorkText { get; set; } 
        public string ResultsAnswerMoreText { get; set; } 

        public string ExploreCareersBaseUrl { get; set; } 
        
    }
}
