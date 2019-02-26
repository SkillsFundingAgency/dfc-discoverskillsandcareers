using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public class ShortTraitDataProcessor : IShortTraitDataProcessor
    {
        readonly ILogger<ShortQuestionSetDataProcessor> Logger;
        readonly IHttpService HttpService;
        readonly IGetShortTraitData GetShortTraitData;

        public ShortTraitDataProcessor(
            ILogger<ShortQuestionSetDataProcessor> logger,
            IHttpService httpService,
            IGetShortTraitData getShortTraitData)
        {
            Logger = logger;
            HttpService = httpService;
            GetShortTraitData = getShortTraitData;
        }

        public async Task RunOnce()
        {
            Logger.LogInformation("Begin poll for ShortTraits");

            string url = "https://dfc-my-skillscareers-sf.azurewebsites.net/api/default/shorttraits"; // TODO: CMS endpoint
            var data = await GetShortTraitData.GetData(url);
        }
    }
}
