using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Xunit;

namespace Dfc.UnitTests.CmsTests
{
    public static class NSubsituteExtensions
    {
        public static void WasCalledOnce(this ILogger logger, LogLevel level, string msg)
        {
            Assert.Single(logger.ReceivedCalls(), call =>
            {
                var args = call.GetArguments();
                var logLevel = (LogLevel)args[0];
                var message = (args[2] as FormattedLogValues)?.ToString();
                return logLevel == level && message == msg;
            });
        }
    }
    
    public class FilteredQuestionSetDataProcessorTests
    {
        private ISiteFinityHttpService _sitefinityService;
        private IQuestionSetRepository _questionSetRepository;
        private IQuestionRepository _questionRepository;
        private IGetFilteringQuestionSetData _getFilteringQuestionSetData;
        private IOptions<AppSettings> _appSetting;
        private FilteredQuestionSetDataProcessor _sut;
        private ILogger _logger;
        private const string SiteFinityUrl = "https://localhost:8080";
        private const string SiteFinityBase = "dsac";

        public FilteredQuestionSetDataProcessorTests()
        {
            _sitefinityService = Substitute.For<ISiteFinityHttpService>();
            _questionSetRepository = Substitute.For<IQuestionSetRepository>();
            _questionRepository = Substitute.For<IQuestionRepository>();
            _getFilteringQuestionSetData = Substitute.For<IGetFilteringQuestionSetData>();
            _appSetting = Substitute.For<IOptions<AppSettings>>();
            _logger = Substitute.For<ILogger>();

            _appSetting.Value.Returns(new AppSettings
            {
                SiteFinityApiUrlbase = SiteFinityUrl,
                SiteFinityApiWebService = SiteFinityBase
            });
            
            _sut = new FilteredQuestionSetDataProcessor(_sitefinityService, _questionRepository, _questionSetRepository,
                _getFilteringQuestionSetData, _appSetting);
        }
        
        [Fact]
        public async Task IfNoQuestionSets_ShouldReturn_AndLog()
        {
            await _sut.RunOnce(_logger);
            
            _logger.WasCalledOnce(LogLevel.Error, "No data returned from CMS while trying to extract filtering question sets");
        }

        [Fact]
        public async Task IfNoJobCategoryAssignedToQuestionSet_ShouldContinue_ButLog()
        {
            var qsId = Guid.NewGuid().ToString();
            var title = "QS-1";
            
            _getFilteringQuestionSetData.GetData(SiteFinityUrl, SiteFinityBase)
                .Returns(Task.FromResult(new List<FilteringQuestionSet>
                {
                    new FilteringQuestionSet
                    {
                        Id = qsId,
                        Title = title,
                        JobProfileTaxonomy = new[] {new TaxonomyHierarchy {Title = null}}
                    }
                }));

            await _sut.RunOnce(_logger);
            
            _logger.WasCalledOnce(LogLevel.Warning, $"No job category assigned to filtering question set {title}, skipping.");
        }
    }
}