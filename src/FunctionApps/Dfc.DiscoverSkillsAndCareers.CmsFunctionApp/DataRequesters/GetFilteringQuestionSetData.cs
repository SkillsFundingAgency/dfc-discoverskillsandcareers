using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetFilteringQuestionSetData : IGetFilteringQuestionSetData
    {
        public Task<List<FilteringQuestionSet>> GetData(string siteFinityApiUrlbase)
        {
            var questionSets = new List<FilteringQuestionSet>()
            {
                new FilteringQuestionSet()
                {
                    Id = "AC1",
                    Title = "Animal Care",
                    LastUpdated = new System.DateTime(2019, 1, 1)
                },
                new FilteringQuestionSet()
                {
                    Id = "SC1",
                    Title = "Social Care",
                    LastUpdated = new System.DateTime(2019, 1, 1)
                },
                new FilteringQuestionSet()
                {
                    Id = "SL1",
                    Title = "Sports and leisure",
                    LastUpdated = new System.DateTime(2019, 1, 1)
                }
            };
            return Task.FromResult(questionSets);
        }
    }
}
