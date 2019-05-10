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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dfc.IntegrationTests
{

    //TODO: Remove the magic ids from these tests they WONT be the 
    //same across the envrionments
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
        public async Task GetFilteringQuestionSets_FromSiteFinity_ShouldContainValues()
        {
            var questionDataGetter = Substitute.For<IGetFilteringQuestionData>();

            questionDataGetter
                .When(g => g.GetData(_appSettings.SiteFinityApiUrlbase, _appSettings.SiteFinityApiWebService, Arg.Any<string>())
                            .Returns(Task.FromResult(new List<FilteringQuestion>()))
                     );


            var requester = new GetFilteringQuestionSetData(_service, questionDataGetter);
            var data = await requester.GetData(_appSettings.SiteFinityApiUrlbase, _appSettings.SiteFinityApiWebService);

            Assert.NotEmpty(data);
        }
        

        [Fact]
        public async Task GetShortQuestions_FromSiteFinity_ShouldContainValues()
        {
            var requester = new GetShortQuestionData(_service);
            var data = await requester.GetData(_appSettings.SiteFinityApiUrlbase, _appSettings.SiteFinityApiWebService, "a05754b1-25ec-459b-910d-6013632c6e2c");

            Assert.NotEmpty(data);
        }

        [Fact]
        public async Task GetQuestionSets_FromSiteFinity_ShouldContainValues()
        {
            var requester = new GetShortQuestionSetData(_service);
            var data = await requester.GetData(_appSettings.SiteFinityApiUrlbase, _appSettings.SiteFinityApiWebService);

            Assert.NotEmpty(data);
        }

        [Fact]
        public async Task GetJobCategories_FromSiteFinity_ShouldContainValues()
        {
            var requester = new GetJobCategoriesData(_service);
            var data = await requester.GetData(_appSettings.SiteFinityApiUrlbase, _appSettings.SiteFinityApiWebService, _appSettings.SiteFinityJobCategoriesTaxonomyId);

            Assert.NotEmpty(data);
        }

        [Fact]
        public async Task GetTraitData_FromSiteFinity_ShouldContainValues()
        {
            var requester = new GetShortTraitData(_service);
            var data = await requester.GetData(_appSettings.SiteFinityApiUrlbase, _appSettings.SiteFinityApiWebService);

            Assert.NotEmpty(data);
        }
    }
}