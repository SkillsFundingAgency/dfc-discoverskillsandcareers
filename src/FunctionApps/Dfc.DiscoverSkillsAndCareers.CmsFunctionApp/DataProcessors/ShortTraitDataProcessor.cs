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
        readonly ISiteFinityHttpService HttpService;
        readonly IGetShortTraitData GetShortTraitData;
        readonly AppSettings AppSettings;
        readonly IShortTraitRepository ShortTraitRepository;

        public ShortTraitDataProcessor(
            ISiteFinityHttpService httpService,
            IGetShortTraitData getShortTraitData,
            IOptions<AppSettings> appSettings,
            IShortTraitRepository shortTraitRepository)
        {
            HttpService = httpService;
            GetShortTraitData = getShortTraitData;
            AppSettings = appSettings.Value;
            ShortTraitRepository = shortTraitRepository;
        }

        public async Task RunOnce(ILogger logger)
        {
            logger.LogInformation("Begin poll for ShortTraits");

            var data = await GetShortTraitData.GetData(AppSettings.SiteFinityApiUrlbase, AppSettings.SiteFinityApiWebService);

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

            logger.LogInformation("End poll for ShortTraits");
        }
    }
}
