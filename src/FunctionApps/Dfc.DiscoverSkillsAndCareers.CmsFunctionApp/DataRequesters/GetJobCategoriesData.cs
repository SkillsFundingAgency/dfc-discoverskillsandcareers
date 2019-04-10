using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetJobCategoriesData : IGetJobCategoriesData
    {
        readonly ISiteFinityHttpService HttpService;

        public GetJobCategoriesData(ISiteFinityHttpService httpService)
        {
            HttpService = httpService;
        }

        public async Task<List<JobCategory>> GetData(string url, string traitsUrl, string taxonomyId = "3b635a67-db48-43d2-b94b-332304775d37")
        {
            var traits = new List<ShortTrait>();
            string traitsJson = await HttpService.GetString(traitsUrl);
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<ShortTrait>>>(traitsJson);
            foreach(var item in data.Value)
            {
                var traitJson = await HttpService.GetString($"{traitsUrl}({item.Id})");
                var trait = JsonConvert.DeserializeObject<ShortTrait>(traitJson);
                trait.Code = trait.Name.ToUpper();
                traits.Add(trait);
            }

            string json = await HttpService.GetString(url);
            var jobCategories = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<TaxonomyHierarchy>>>(json).Value
                .Where(x => x.TaxonomyId == taxonomyId)
                .Select(x => new JobCategory
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    UrlName = x.UrlName,
                    Traits = new List<string>()
                })
                .ToList();
            foreach (var jobCategory in jobCategories)
            {
                foreach (var trait in traits)
                {
                    if (trait.JobProfileCategories.Contains(new System.Guid(jobCategory.Id)))
                    {
                        jobCategory.Traits.Add(trait.Code);
                    }
                }
            }
            return jobCategories;
        }
    }
}
