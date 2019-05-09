using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ContentResultsPage : IContentPage
    {
        public string Title { get; set; } = "National Careers Service - Results";
        public string TraitTitle { get; set; } = "What you told us";
        public string TraitSummaryText { get; set; } = "Because of your answers, we have provided the following job categories. You can choose to view more results at the bottom of the page.";
        public string TraitSummaryTextNoResults { get; set; } = "Because of your answers, we could not recommend any job categories. You might want to go through the assessment again to check that your responses were correct.";
        public string Headline { get; set; } = "Your results";
        public string SaveProgressText { get; set; } = "Save my progress";
        public string SeeLinkButtonText { get; set; } = "See job category";
        public string SaveProgressTitle { get; set; } = "Return to this later";
        [JsonProperty("LastModified")]
        public DateTimeOffset LastUpdated { get; set; }
        public string StartFilterQuestionsButtonText { get; set; } = "Answer [questions] more question";
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
        public string HoursDetailsText { get; set; }
        public string YouCouldWorkText { get; set; } = "You could work";
        public string ResultsAnswerMoreText { get; set; } = "To find out job roles you might be suited to in [jobcategory], answer more questions.";
        public string JobCategoryTextSingle { get; set; } = "job category";
        public string JobCategoryTextPlural { get; set; } = "job categories";
        public string ExploreCareersBaseUrl { get; set; } = "https://nationalcareers.service.gov.uk";
    }
}
