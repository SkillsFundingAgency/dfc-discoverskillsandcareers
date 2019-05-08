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
        readonly ISiteFinityHttpService _httpService;
        readonly IGetShortTraitData _getShortTraitData;
        readonly AppSettings _appSettings;
        readonly IShortTraitRepository _shortTraitRepository;

        public ShortTraitDataProcessor(
            ISiteFinityHttpService httpService,
            IGetShortTraitData getShortTraitData,
            IOptions<AppSettings> appSettings,
            IShortTraitRepository shortTraitRepository)
        {
            _httpService = httpService;
            _getShortTraitData = getShortTraitData;
            _appSettings = appSettings.Value;
            _shortTraitRepository = shortTraitRepository;
        }

        public async Task RunOnce(ILogger logger)
        {
            logger.LogInformation("Begin poll for ShortTraits");

            var data = await _getShortTraitData.GetData(_appSettings.SiteFinityApiUrlbase, _appSettings.SiteFinityApiWebService);

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
                await _shortTraitRepository.CreateTrait(trait, "shorttrait-cms");
            }

            logger.LogInformation("End poll for ShortTraits");
        }
    }
}
