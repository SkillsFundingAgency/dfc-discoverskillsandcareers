using System;
using System.Collections.Generic;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ShortQuestionSet
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<ShortQuestion> Questions { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
