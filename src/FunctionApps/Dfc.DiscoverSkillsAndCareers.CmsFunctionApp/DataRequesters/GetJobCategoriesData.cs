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
        readonly ISiteFinityHttpService _siteFinityService;

        public GetJobCategoriesData(ISiteFinityHttpService siteFinityService)
        {
            _siteFinityService = siteFinityService;
        }

        public async Task<List<SiteFinityJobCategory>> GetData(string taxonomyName = "Job Profile Category")
        {
            var traits = await _siteFinityService.GetAll<SiteFinityTrait>("traits");

            var taxonomies = await _siteFinityService.GetTaxonomyInstances(taxonomyName);
            
            var jobCategories = taxonomies
                .Select(x => new SiteFinityJobCategory
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
                    if (trait.JobProfileCategories.Contains(jobCategory.Id))
                    {
                        jobCategory.Traits.Add(trait.Code);
                    }
                }
            }
            return jobCategories.Where(jc => jc.Traits.Count > 0).ToList();
        }
    }
}
