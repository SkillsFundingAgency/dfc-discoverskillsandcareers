using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetFilteringQuestionSetData : IGetFilteringQuestionSetData
    {
        readonly IHttpService HttpService;
        readonly IGetFilteringQuestionData GetFilteringQuestionData;

        public GetFilteringQuestionSetData(IHttpService httpService,
            IGetFilteringQuestionData getFilteringQuestionData)
        {
            HttpService = httpService;
            GetFilteringQuestionData = getFilteringQuestionData;
        }

        public async Task<List<FilteringQuestionSet>> GetData(string siteFinityApiUrlbase, string siteFinityService, bool useLocalFile)
        {
            if (useLocalFile)
            {
                var runPath = System.Environment.CurrentDirectory;
                var fileJson = await System.IO.File.ReadAllTextAsync(System.IO.Path.Combine(runPath, "filtering_questions.json"));
                return JsonConvert.DeserializeObject<List<FilteringQuestionSet>>(fileJson);
            }
            string url = $"{siteFinityApiUrlbase}/api/{siteFinityService}/filteringquestionsets";
            string json = await HttpService.GetString(url);
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<FilteringQuestionSet>>>(json);
            foreach (var fqs in data.Value)
            {
                fqs.Questions = await GetFilteringQuestionData.GetData(siteFinityApiUrlbase, siteFinityService, fqs.Id);

            }
            var localPath = @"C:\ncs\dfc-discoverskillsandcareers-dev\src\FunctionApps\Dfc.DiscoverSkillsAndCareers.CmsFunctionApp"; //TODO: temp issue as no CMS
            var saveFilename = System.IO.Path.Combine(localPath, "filtering_questions.json");
            await System.IO.File.WriteAllTextAsync(saveFilename, JsonConvert.SerializeObject(data.Value));
            return data.Value;
        }
    }
}
