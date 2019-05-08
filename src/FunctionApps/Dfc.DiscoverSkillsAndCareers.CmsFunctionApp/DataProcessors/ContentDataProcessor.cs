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
        readonly ISiteFinityHttpService _httpService;
        readonly IGetContentData<List<T>> _getContentData;
        readonly AppSettings _appSettings;
        readonly IContentRepository _contentRepository;

        public ContentDataProcessor(
            ISiteFinityHttpService httpService,
            IGetContentData<List<T>> getContentData,
            IOptions<AppSettings> appSettings,
            IContentRepository contentRepository)
        {
            _httpService = httpService;
            _getContentData = getContentData;
            _appSettings = appSettings.Value;
            _contentRepository = contentRepository;
        }

        public async Task RunOnce(ILogger logger, string siteFinityType, string contentType)
        {
            logger.LogInformation("Begin poll for Content");
            var data = await _getContentData.GetData(_appSettings.SiteFinityApiUrlbase, _appSettings.SiteFinityApiWebService, siteFinityType);

            var cmsContent = data.FirstOrDefault();

            if (cmsContent == null)
            {
                logger.LogInformation($"No cms content could be found for {siteFinityType}");
                return;
            }

            // Get the existing content
            var existingContent = await _contentRepository.GetContent(contentType);

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
            await _contentRepository.CreateContent(existingContent);

            logger.LogInformation("End poll for Content");
        }
    }
}
