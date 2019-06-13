using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetShortQuestionData : IGetShortQuestionData
    {
        readonly ISiteFinityHttpService _httpService;

        public GetShortQuestionData(ISiteFinityHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<List<SiteFinityShortQuestion>> GetData(string questionSetId)
        {
            var data = await _httpService.GetAll<SiteFinityShortQuestion>($"shortquestionsets({questionSetId})/Questions");
            foreach (var question in data)
            {
                var trait = await _httpService.Get<SiteFinityTrait>($"shortquestions({question.Id})/Trait");
                question.Trait = trait.Name;
            }
            return data;
        }
    }
}
