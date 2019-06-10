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
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Xunit;

namespace Dfc.UnitTests.CmsTests
{
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
        public async Task IfNoJobCategoryAssignedToQuestionSet_ShouldLog()
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

        [Fact]
        public async Task IfQuestionSetDoesNotRequireUpdate_ShouldLog()
        {
            var qsId = Guid.NewGuid().ToString();
            var jobCategory = "Animal care";
            var localLastUpdated = new DateTimeOffset(2019, 6, 1, 0, 0, 0, TimeSpan.Zero);
            var sitefinityLastUpdated = localLastUpdated.AddDays(-1);
            
            _getFilteringQuestionSetData.GetData(SiteFinityUrl, SiteFinityBase)
                .Returns(Task.FromResult(new List<FilteringQuestionSet>
                {
                    new FilteringQuestionSet
                    {
                        Id = qsId,
                        Title = "Animal care",
                        JobProfileTaxonomy = new[] {new TaxonomyHierarchy {Title = jobCategory}},
                        LastUpdated = sitefinityLastUpdated
                    }
                }));

            _questionSetRepository.GetLatestQuestionSetByTypeAndKey("filtered", jobCategory).Returns(Task.FromResult(
                new QuestionSet
                {
                    AssessmentType = "filtered",
                    QuestionSetVersion = "filtered-animal care-1",
                    Title = jobCategory,
                    MaxQuestions = 3,
                    Version = 1,
                    LastUpdated = localLastUpdated,
                    IsCurrent = true
                }));
            
            await _sut.RunOnce(_logger);
            
            _logger.WasCalledOnce(LogLevel.Information, $"Filtering Question Set {jobCategory} is upto date - no changes to be done");
        }
    }
}