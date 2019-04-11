using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dfc.IntegrationTests
{
    public class CmsTests
    {
        private ILogger<SiteFinityHttpService> _logger;
        private AppSettings _appSettings;
        private SiteFinityHttpService _service;
        private readonly IConfiguration Configuration;

        public CmsTests()
        {
            _logger = Substitute.For<ILogger<SiteFinityHttpService>>();
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            Configuration = builder.Build();
            _appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            _service = new SiteFinityHttpService(_logger, new OptionsWrapper<AppSettings>(_appSettings));

        }

        [Fact]
        public async Task GetJobProfileData_WithLocalData_ShouldContainJobProfiles()
        {
            var requester = new GetJobProfileData(_service);

            var data = await requester.GetData(null);

            Assert.True(data.Count > 0);
            Assert.Equal("Video editor", data.First(x => x.SocCode == "3416I").Title);
        }

        [Fact]
        public async Task GetIndexPageContentData_FromSiteFinity_ShouldContainContent()
        {
            var requester = new GetContentData<ContentIndexPage>(_service);

            string url = $"{_appSettings.SiteFinityApiUrlbase}/api/{_appSettings.SiteFinityApiWebService}/indexpagecontents";
            var data = await requester.GetData(url);

            Assert.NotEmpty(data.Headline);
        }

        [Fact]
        public async Task GetFinishedContentData_FromSiteFinity_ShouldContainContent()
        {
            var requester = new GetContentData<ContentFinishPage>(_service);

            string url = $"{_appSettings.SiteFinityApiUrlbase}/api/{_appSettings.SiteFinityApiWebService}/finishpagecontents";
            var data = await requester.GetData(url);

            Assert.NotEmpty(data.Headline);
        }

        [Fact]
        public async Task GetQuestionPageContentData_FromSiteFinity_ShouldContainContent()
        {
            var requester = new GetContentData<ContentQuestionPage>(_service);

            string url = $"{_appSettings.SiteFinityApiUrlbase}/api/{_appSettings.SiteFinityApiWebService}/questionpagecontents";
            var data = await requester.GetData(url);

            Assert.NotEmpty(data.Title);
        }

        [Fact]
        public async Task GetShortResultsPageContentData_FromSiteFinity_ShouldContainContent()
        {
            var requester = new GetContentData<ContentShortResultsPage>(_service);

            string url = $"{_appSettings.SiteFinityApiUrlbase}/api/{_appSettings.SiteFinityApiWebService}/shortfinishcontents";
            var data = await requester.GetData(url);

            Assert.NotEmpty(data.Title);
        }

        [Fact]
        public async Task GetSaveProgressContentsData_FromSiteFinity_ShouldContainContent()
        {
            var requester = new GetContentData<ContentSaveProgressPage>(_service);

            string url = $"{_appSettings.SiteFinityApiUrlbase}/api/{_appSettings.SiteFinityApiWebService}/saveprogresscontents";
            var data = await requester.GetData(url);

            Assert.NotEmpty(data.Title);
        }

        [Fact]
        public async Task GetTraitData_FromSiteFinity_ShouldContainValues()
        {
            var requester = new GetShortTraitData(_service);

            string url = $"{_appSettings.SiteFinityApiUrlbase}/api/{_appSettings.SiteFinityApiWebService}/traits";
            var data = await requester.GetData(url);

            Assert.NotEmpty(data);
        }
    }
}
