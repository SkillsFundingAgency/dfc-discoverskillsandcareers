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
        public string Title { get; set; } = "National Careers Service - Results";
        public string TraitTitle { get; set; } = "What you told us";
        public string TraitSummaryText { get; set; } = "Because of your answers, we have provided the following job types. You can choose to view more results at the bottom of the page.";
        public string Headline { get; set; } = "Job categories you might be suited to";
        public string SaveProgressLinkText { get; set; } = "Save my progress";
        public string SeeLinkButtonText { get; set; } = "See job category";
        public string SaveProgressTitle { get; set; } = "Return to this later";
        public bool UseFilteringQuestions { get; set; }
        public JobProfileResult[] JobProfiles { get; set; }
        public string[] WhatYouToldUs { get; set; }
        public string StartFilterQuestionsButtonText { get; set; } = "Answer more questions";
        public string ViewResultsButtonText { get; set; } = "View results";
        public string WhatYouToldUsTitle { get; set; } = "What you told us";
        public string JobProfilesTitle { get; set; } = "Job roles that might suit you";
        public string JobProfileTextSingle { get; set; } = "role";
        public string JobProfileTextPlural { get; set; } = "roles";
        public string ThatMightSuitText { get; set; } = "that might suit you";
        public string AverageSalaryText { get; set; } = "Average salary (a year)";
        public string StarterText { get; set; } = "Starter";
        public string ExperiencedText { get; set; } = "Experienced";
        public string TypicalHoursText { get; set; } = "Typical hours (a week)";
        public string YouCouldWorkText { get; set; } = "You could work";
        public string ResultsAnswerMoreText { get; set; } = "To find out job roles you might be suited to in [jobcategory], answer more questions.";
    }
}
