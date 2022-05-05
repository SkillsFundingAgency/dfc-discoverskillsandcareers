using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace ConsoleApp1
{
    class Program
    {
        async static Task Main(string[] args)
        {
            var options = new SiteFinitySettings
            {
                SiteFinityApiUrlBase = "https://dev-service.nationalcareersservice.org.uk",
                SiteFinityApiWebService = "dsac",
                SiteFinityRequiresAuthentication = true,
                SiteFinityApiAuthenicationEndpoint = "SiteFinity/Authenticate/openid/connect/token",
                SiteFinityScope = "offline_access openid",
                SiteFinityUsername = "A",
                SiteFinityPassword = "B",
                SiteFinityClientId = "C",
                SiteFinityClientSecret = "2D",
            };

            ISiteFinityHttpService _sitefinity = new SiteFinityHttpService(
                new NullLoggerFactory(),
                new OptionsWrapper<SiteFinitySettings>(options));
            
            var jobProfiles = 
                await _sitefinity.GetAll<SiteFinityJobProfile>("jobprofiles?$select=Id,Title,JobProfileCategories&$expand=RelatedSkills&$orderby=Title");

            var jobCategories = 
                await _sitefinity.GetTaxonomyInstances("Job Profile Category");
            
            var categorySkillMappings = JobCategorySkillMapper.Map(jobProfiles, jobCategories,
                0.75,
                0.75);
        }
    }
}