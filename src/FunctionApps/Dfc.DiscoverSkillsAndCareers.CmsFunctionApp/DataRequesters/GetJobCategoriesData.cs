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
        readonly ISiteFinityHttpService _httpService;

        public GetJobCategoriesData(ISiteFinityHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<List<JobCategory>> GetData(string taxonomyName = "Job Profile Category")
        {
            var traits = await _httpService.GetAll<ShortTrait>("traits");

            var taxonomies = await _httpService.GetTaxonomyInstances(taxonomyName);
            
            var jobCategories = taxonomies
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
                    if (trait.JobProfileCategories.Contains(new Guid(jobCategory.Id)))
                    {
                        jobCategory.Traits.Add(trait.Code);
                    }
                }
            }
            return jobCategories.Where(jc => jc.Traits.Count > 0).ToList();
        }
    }
}
