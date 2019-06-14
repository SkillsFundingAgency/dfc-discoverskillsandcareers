using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class JobCategoryQuestion
    {
        [JsonProperty("Id")]
        public Guid Id { get; set; }
        
        [JsonProperty("Title")]
        public string Title { get; set; }
        
        [JsonProperty("QuestionText")]
        public string QuestionText { get; set; }
        
        [JsonProperty("Description")]
        public string Description { get; set; }
      
        
        [JsonProperty("RelatedSkill")]
        public SiteFinityFilteringQuestionSkill RelatedSkill { get; set; }
        
        [JsonProperty("LastModified")]
        public DateTimeOffset LastUpdated { get; set; }
        
        public List<SiteFinityFilteringQuestionJobProfileMapping> JobProfiles { get; set; } = new List<SiteFinityFilteringQuestionJobProfileMapping>();

    }
    
    public class JobCategoryQuestionsResult
    {
        
        private readonly List<(LogLevel, string)> _message = new List<(LogLevel, string)>();
        
        public string JobCategory { get; set; }
        
        public List<JobCategoryQuestion> Questions { get; set; } = new List<JobCategoryQuestion>();

        public bool HasMessages => _message.Count > 0;

        public IEnumerable<(LogLevel, string)> Messages => _message.AsEnumerable();

        public void AddMessage(LogLevel level, string error)
        {
            _message.Add((level, $"Job category: {JobCategory} - {error}"));
        }
    }
}