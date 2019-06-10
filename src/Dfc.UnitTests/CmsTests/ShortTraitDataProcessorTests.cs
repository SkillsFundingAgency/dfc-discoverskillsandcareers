using System.Collections.Generic;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Build.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Dfc.UnitTests.CmsTests
{
    public class ShortTraitDataProcessorTests
    {
        private ISiteFinityHttpService _sitefinityService;
        private IShortTraitRepository _shortTraitRepository;
        private IGetShortTraitData _getShortTraitData;
        private IOptions<AppSettings> _appSetting;
        private ShortTraitDataProcessor _sut;
        private ILogger _logger;
        private const string SiteFinityUrl = "https://localhost:8080";
        private const string SiteFinityBase = "dsac";

        public ShortTraitDataProcessorTests()
        {
            _sitefinityService = Substitute.For<ISiteFinityHttpService>();
            _shortTraitRepository = Substitute.For<IShortTraitRepository>();
            _getShortTraitData = Substitute.For<IGetShortTraitData>();
            _appSetting = Substitute.For<IOptions<AppSettings>>();
            _logger = Substitute.For<ILogger>();

            _appSetting.Value.Returns(new AppSettings
            {
                SiteFinityApiUrlbase = SiteFinityUrl,
                SiteFinityApiWebService = SiteFinityBase
            });
            
            _sut = new ShortTraitDataProcessor(_sitefinityService,_getShortTraitData, _appSetting, _shortTraitRepository);
        }

        [Fact]
        public async Task IfTraitNameIsEmpty_Should_Skip()
        {
            _getShortTraitData.GetData(SiteFinityUrl, SiteFinityBase).Returns(Task.FromResult(new List<ShortTrait>
            {
                new ShortTrait { Id = "trait-1" }
            }));

            await _shortTraitRepository.DidNotReceive().CreateTrait(Arg.Any<Trait>(), "shorttrait-cms");
            
            await _sut.RunOnce(_logger);
        }
    }
}