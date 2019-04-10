using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public class ShortTraitDataProcessor : IShortTraitDataProcessor
    {
        readonly ILogger<ShortQuestionSetDataProcessor> Logger;
        readonly ISiteFinityHttpService HttpService;
        readonly IGetShortTraitData GetShortTraitData;
        readonly AppSettings AppSettings;
        readonly IShortTraitRepository ShortTraitRepository;

        public ShortTraitDataProcessor(
            ILogger<ShortQuestionSetDataProcessor> logger,
            ISiteFinityHttpService httpService,
            IGetShortTraitData getShortTraitData,
            IOptions<AppSettings> appSettings,
            IShortTraitRepository shortTraitRepository)
        {
            Logger = logger;
            HttpService = httpService;
            GetShortTraitData = getShortTraitData;
            AppSettings = appSettings.Value;
            ShortTraitRepository = shortTraitRepository;
        }

        public async Task RunOnce()
        {
            Logger.LogInformation("Begin poll for ShortTraits");

            string siteFinityApiUrlbase = AppSettings.SiteFinityApiUrlbase;
            string siteFinityService = AppSettings.SiteFinityApiWebService;

            string url = $"{siteFinityApiUrlbase}/api/{siteFinityService}/traits";
            var data = await GetShortTraitData.GetData(url);

            foreach(var traitData in data)
            {
                if (string.IsNullOrEmpty(traitData.Name))
                {
                    continue;
                }
                var trait = new Trait()
                {
                    TraitName = traitData.Name,
                    TraitCode = traitData.Name.ToUpper(),
                    Texts = new[]
                    { new TraitText()
                        {
                            LanguageCode = "en",
                            Text = traitData.ResultDisplayText
                        }
                    }
                };
                await ShortTraitRepository.CreateTrait(trait, "shorttrait-cms");
            }

            Logger.LogInformation("End poll for ShortTraits");
        }
    }
}
