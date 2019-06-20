using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public class ShortTraitDataProcessor : IContentTypeProcessor<ShortTraitDataProcessor>
    {
        private readonly ISiteFinityHttpService _sitefinity;
        private readonly IShortTraitRepository _shortTraitRepository;

        public ShortTraitDataProcessor(ISiteFinityHttpService sitefinity, IShortTraitRepository shortTraitRepository)
        {
            _sitefinity = sitefinity;
            _shortTraitRepository = shortTraitRepository;
        }

        public async Task RunOnce(ILogger logger)
        {
            logger.LogInformation("Begin poll for ShortTraits");

            var data = await _sitefinity.GetAll<SiteFinityTrait>("traits");

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
                await _shortTraitRepository.CreateTrait(trait);
            }

            logger.LogInformation("End poll for ShortTraits");
        }
    }
}
