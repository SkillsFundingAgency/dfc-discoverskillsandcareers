using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetShortQuestionData : IGetShortQuestionData
    {
        readonly IHttpService HttpService;

        public GetShortQuestionData(IHttpService httpService)
        {
            HttpService = httpService;
        }

        public async Task<List<ShortQuestion>> GetData(string siteFinityApiUrlbase, string questionSetId)
        {
            string getQuestionsUrl = $"{siteFinityApiUrlbase}/api/default/shortquestionsets({questionSetId})/Questions";
            string json = await HttpService.GetString(getQuestionsUrl);
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<ShortQuestion>>>(json);
            foreach (var question in data.Value)
            {
                string getShortTraitUrl = $"{siteFinityApiUrlbase}/api/default/shortquestions({question.Id})/Trait";
                json = await HttpService.GetString(getShortTraitUrl);
                var trait = JsonConvert.DeserializeObject<ShortTrait>(json);
                question.Trait = trait.Name;
            }
            return data.Value;
        }
    }
}
