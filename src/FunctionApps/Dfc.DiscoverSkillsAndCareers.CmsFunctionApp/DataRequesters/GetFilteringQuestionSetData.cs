using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetFilteringQuestionSetData : IGetFilteringQuestionSetData
    {
        private readonly ISiteFinityHttpService _sitefinity;
        private readonly IGetFilteringQuestionData _getFilteringQuestionData;

        public GetFilteringQuestionSetData(ISiteFinityHttpService sitefinity,
            IGetFilteringQuestionData getFilteringQuestionData)
        {
            _sitefinity = sitefinity;
            _getFilteringQuestionData = getFilteringQuestionData;
        }

        public async Task<List<FilteringQuestionSet>> GetData()
        {
            var data = await _sitefinity.GetAll<FilteringQuestionSet>("filteringquestionsets?$expand=JobProfileTaxonomy");
            foreach (var fqs in data)
            {
                fqs.Questions = await _getFilteringQuestionData.GetData(fqs.Id);
            }
            return data;
        }
    }
}
