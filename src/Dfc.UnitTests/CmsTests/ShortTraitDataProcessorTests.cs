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
        private IShortTraitRepository _shortTraitRepository;
        private IGetShortTraitData _getShortTraitData;
        private ShortTraitDataProcessor _sut;
        private ILogger _logger;

        public ShortTraitDataProcessorTests()
        {
            _shortTraitRepository = Substitute.For<IShortTraitRepository>();
            _getShortTraitData = Substitute.For<IGetShortTraitData>();
            _logger = Substitute.For<ILogger>();
  
            _sut = new ShortTraitDataProcessor(_getShortTraitData, _shortTraitRepository);
        }

        [Fact]
        public async Task IfTraitNameIsEmpty_Should_Skip()
        {
            _getShortTraitData.GetData().Returns(Task.FromResult(new List<ShortTrait>
            {
                new ShortTrait { Id = "trait-1" }
            }));

            await _shortTraitRepository.DidNotReceive().CreateTrait(Arg.Any<Trait>(), "shorttrait-cms");
            
            await _sut.RunOnce(_logger);
        }
    }
}