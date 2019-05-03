using System;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetJobCategoriesData : IGetJobCategoriesData
    {
        readonly ISiteFinityHttpService HttpService;

        public GetJobCategoriesData(ISiteFinityHttpService httpService)
        {
            HttpService = httpService;
        }

        public async Task<List<JobCategory>> GetData(string sitefinityBaseUrl, string sitefinityWebService, string taxonomyName = "Job Profile Category")
        {
            var traits = new List<ShortTrait>();
            var traitsUrl = $"{sitefinityBaseUrl}/api/{sitefinityWebService}/traits";
            
           
            
            string traitsJson = await HttpService.GetString(traitsUrl);
            
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<ShortTrait>>>(traitsJson);
            foreach(var item in data.Value)
            {
                var traitJson = await HttpService.GetString($"{traitsUrl}({item.Id})");
                var trait = JsonConvert.DeserializeObject<ShortTrait>(traitJson);
                trait.Code = trait.Name.ToUpper();
                traits.Add(trait);
            }
            
            var taxonomyJson = await HttpService.GetString($"{sitefinityBaseUrl}/api/{sitefinityWebService}/taxonomies");
            var taxaId =
                JsonConvert.DeserializeObject<SiteFinityDataFeed<List<JObject>>>(taxonomyJson)
                    .Value
                    .Single(r => String.Equals(r.Value<string>("TaxonName"), taxonomyName, StringComparison.InvariantCultureIgnoreCase))
                    .Value<string>("Id");

            string json = await HttpService.GetString($"{sitefinityBaseUrl}/api/{sitefinityWebService}/hierarchy-taxa");
            
            var jobCategories = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<TaxonomyHierarchy>>>(json).Value
                .Where(x => String.Equals(x.TaxonomyId, taxaId, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => new JobCategory
                {
                    Id = x.Id,
                    TaxonomyId = x.TaxonomyId,
                    Title = x.Title,
                    Description = x.Description,
                    UrlName = x.UrlName,
                    Traits = new List<string>()
                }).ToList();
            
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
            return jobCategories.Where(jc => jc.Traits.Count > 0).ToList();
        }
    }
}
