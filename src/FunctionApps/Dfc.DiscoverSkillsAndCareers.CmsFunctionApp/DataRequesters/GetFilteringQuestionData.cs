using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetFilteringQuestionData : IGetFilteringQuestionData
    {
        readonly ISiteFinityHttpService _sitefinity;

        public GetFilteringQuestionData(ISiteFinityHttpService sitefinity)
        {
            _sitefinity = sitefinity;
        }

        public async Task<List<FilteringQuestion>> GetData(string questionSetId)
        {
            var questions = await _sitefinity.GetAll<FilteringQuestion>($"filteringquestionsets({questionSetId})/Questions");
            
            foreach (var question in questions)
            {
                var profiles = await _sitefinity.GetAll<JobProfile>($"filteringquestions({question.Id})/ExcludedJobProfiles");
                question.ExcludesJobProfiles = profiles.Select(jp => jp.Title).ToList();
            }
            return questions;
        }
    }
}