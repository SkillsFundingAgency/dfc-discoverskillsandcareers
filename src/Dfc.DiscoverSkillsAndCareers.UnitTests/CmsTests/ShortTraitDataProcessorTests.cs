using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Dfc.UnitTests.CmsTests
{
    public class ShortTraitDataProcessorTests
    {
        private ISiteFinityHttpService _siteFinityHttpService;
        private IShortTraitRepository _shortTraitRepository;
        private ShortTraitDataProcessor _sut;
        private ILogger _logger;

        public ShortTraitDataProcessorTests()
        {
            _siteFinityHttpService = Substitute.For<ISiteFinityHttpService>();
            _shortTraitRepository = Substitute.For<IShortTraitRepository>();
            _logger = Substitute.For<ILogger>();

            _sut = new ShortTraitDataProcessor(_siteFinityHttpService, _shortTraitRepository);
        }

        [Fact]
        public async Task ShouldNot_CallTraitRepository_IfNoName()
        {
            _siteFinityHttpService.GetAll<SiteFinityTrait>("traits").Returns(Task.FromResult(new List<SiteFinityTrait>
            {
                new SiteFinityTrait { Name = null, UrlName = "leader", Id = "1"}
            }));

            await _sut.RunOnce(_logger);

            await _shortTraitRepository.DidNotReceiveWithAnyArgs().CreateTrait(Arg.Any<Trait>());
        }
        
        [Fact]
        public async Task Should_CallTraitRepository_IfValidTrait()
        {
            _siteFinityHttpService.GetAll<SiteFinityTrait>("traits").Returns(Task.FromResult(new List<SiteFinityTrait>
            {
                new SiteFinityTrait { Name = "Leader", UrlName = "leader", Id = "1", ResultDisplayText = "Test"}
            }));

            await _sut.RunOnce(_logger);

            await _shortTraitRepository.Received().CreateTrait(Arg.Is<Trait>(t => 
                t.TraitName == "Leader" 
                && t.Texts[0].Text == "Test" 
                && t.Texts[0].LanguageCode == "en"));
        }
    }
}