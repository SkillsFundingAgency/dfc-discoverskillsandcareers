using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public class ContentDataProcessor<T> : IContentDataProcessor<T> where T : IContentPage
    {
        readonly ISiteFinityHttpService HttpService;
        readonly IGetContentData<List<T>> GetContentData;
        readonly AppSettings AppSettings;
        readonly IContentRepository ContentRepository;

        public ContentDataProcessor(
            ISiteFinityHttpService httpService,
            IGetContentData<List<T>> getContentData,
            IOptions<AppSettings> appSettings,
            IContentRepository contentRepository)
        {
            HttpService = httpService;
            GetContentData = getContentData;
            AppSettings = appSettings.Value;
            ContentRepository = contentRepository;
        }

        public async Task RunOnce(ILogger logger, string siteFinityType, string contentType)
        {
            logger.LogInformation("Begin poll for Content");
            var data = await GetContentData.GetData(AppSettings.SiteFinityApiUrlbase, AppSettings.SiteFinityApiWebService, siteFinityType);

            var cmsContent = data.FirstOrDefault();

            if (cmsContent == null)
            {
                logger.LogInformation($"No cms content could be found for {siteFinityType}");
                return;
            }

            // Get the existing content
            var existingContent = await ContentRepository.GetContent(contentType);

            // Determine if an update is required i.e. the last updated datetime stamp has changed
            bool updateRequired = existingContent == null || (cmsContent.LastUpdated != existingContent.LastUpdated);

            // Nothing to do so log and exit
            if (!updateRequired)
            {
                logger.LogInformation($"Content {existingContent.ContentType} is upto date - no changes to be done");
                return;
            }

            // Create new if required, otherwise update the content type
            if (existingContent == null)
            {
                existingContent = new Content { ContentType = contentType.ToLower() };
            }
            existingContent.ContentData = JsonConvert.SerializeObject(cmsContent);
            existingContent.LastUpdated = cmsContent.LastUpdated;
            await ContentRepository.CreateContent(existingContent);

            logger.LogInformation("End poll for Content");
        }
    }
}
