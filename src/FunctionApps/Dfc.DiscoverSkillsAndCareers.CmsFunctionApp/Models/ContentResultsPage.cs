using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ContentResultsPage : IContentPage
    {
        public string Title { get; set; } 
        public string TraitTitle { get; set; } 
        public string TraitSummaryText { get; set; } 
        public string Headline { get; set; } 
        public string SaveProgressText { get; set; } 
        public string SeeLinkButtonText { get; set; } 
        public string SaveProgressTitle { get; set; } 
        [JsonProperty("LastModified")]
        public DateTime LastUpdated { get; set; }
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
        public string YouCouldWorkText { get; set; } 
    }
}
