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

        public async Task<List<ShortQuestion>> GetData(string siteFinityApiUrlbase, string siteFinityService, string questionSetId)
        {
            string getQuestionsUrl = $"{siteFinityApiUrlbase}/api/{siteFinityService}/shortquestionsets({questionSetId})/Questions";
            string json = await _httpService.GetString(getQuestionsUrl);
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<ShortQuestion>>>(json, JsonSettings.Instance);
            foreach (var question in data.Value)
            {
                string getShortTraitUrl = $"{siteFinityApiUrlbase}/api/{siteFinityService}/shortquestions({question.Id})/Trait";
                json = await _httpService.GetString(getShortTraitUrl);
                var trait = JsonConvert.DeserializeObject<ShortTrait>(json, JsonSettings.Instance);
                question.Trait = trait.Name;
            }
            return data.Value;
        }
    }
}
