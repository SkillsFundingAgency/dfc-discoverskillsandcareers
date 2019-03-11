using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public class ShortTraitDataProcessor : IShortTraitDataProcessor
    {
        readonly ILogger<ShortQuestionSetDataProcessor> Logger;
        readonly IHttpService HttpService;
        readonly IGetShortTraitData GetShortTraitData;
        readonly AppSettings AppSettings;

        public ShortTraitDataProcessor(
            ILogger<ShortQuestionSetDataProcessor> logger,
            IHttpService httpService,
            IGetShortTraitData getShortTraitData,
            IOptions<AppSettings> appSettings)
        {
            Logger = logger;
            HttpService = httpService;
            GetShortTraitData = getShortTraitData;
            AppSettings = appSettings.Value;
        }

        public async Task RunOnce()
        {
            Logger.LogInformation("Begin poll for ShortTraits");

            string siteFinityApiUrlbase = AppSettings.SiteFinityApiUrlbase;

            string url = $"{siteFinityApiUrlbase}/api/default/shorttraits";
            var data = await GetShortTraitData.GetData(url);

            // TODO: save some data

            Logger.LogInformation("End poll for ShortTraits");
        }
    }
}
