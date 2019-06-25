using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Dfc.UnitTests.CmsTests
{
    public class FilteredQuestionSetDataProcessorTests
    {
        private IQuestionRepository _questionRepository;
        private IQuestionSetRepository _questionSetRepository;
        private ISiteFinityHttpService _siteFinityHttpService;
        private ILogger _logger;
        
        private FilteredQuestionSetDataProcessor _sut;
        
        public FilteredQuestionSetDataProcessorTests()
        {
            _questionRepository = Substitute.For<IQuestionRepository>();
            _questionSetRepository = Substitute.For<IQuestionSetRepository>();
            _siteFinityHttpService = Substitute.For<ISiteFinityHttpService>();
            _logger = Substitute.For<ILogger>();

            _sut = new FilteredQuestionSetDataProcessor(_questionRepository, _questionSetRepository,
                _siteFinityHttpService);
        }

        [Fact]
        public async Task IfNoQuestionSets_ShouldReturn_AndLog()
        {
            _siteFinityHttpService.GetAll<SiteFinityFilteringQuestionSet>("filteringquestionsets")
                .Returns(Task.FromResult<List<SiteFinityFilteringQuestionSet>>(null));

            await _sut.RunOnce(_logger);
            
            _logger.WasCalledOnce(LogLevel.Error, "No Filtered Question sets available");
        }

        [Fact]
        public async Task IfQuestionSetDoesNotRequireUpdate_ShouldLog()
        {
            var qsId = Guid.NewGuid().ToString();
            var localLastUpdated = new DateTimeOffset(2019, 6, 1, 0, 0, 0, TimeSpan.Zero);
            var sitefinityLastUpdated = localLastUpdated.AddDays(-1);
            
            _siteFinityHttpService.GetAll<SiteFinityFilteringQuestionSet>("filteringquestionsets")
                .Returns(Task.FromResult(new List<SiteFinityFilteringQuestionSet>
                {
                    new SiteFinityFilteringQuestionSet
                    {
                        Id = qsId,
                        Title = "Default",
                        LastUpdated = sitefinityLastUpdated
                    }
                }));

            _questionSetRepository.GetCurrentQuestionSet("filtered").Returns(Task.FromResult(
                new QuestionSet
                {
                    AssessmentType = "filtered",
                    QuestionSetVersion = "default-1",
                    Title = "default",
                    MaxQuestions = 3,
                    Version = 1,
                    LastUpdated = localLastUpdated,
                    IsCurrent = true
                }));
            
            await _sut.RunOnce(_logger);
            
            _logger.WasCalledOnce(LogLevel.Information, $"Filtered Question set {qsId} Default is upto date - no changes to be done");
        }
        
        [Fact]
        public async Task IfNoQuestionsForQuestionSet_ShouldLog()
        {
            var qsId = Guid.NewGuid().ToString();
            var localLastUpdated = new DateTimeOffset(2019, 6, 1, 0, 0, 0, TimeSpan.Zero);
            var sitefinityLastUpdated = localLastUpdated.AddDays(1);
            
            _siteFinityHttpService.GetAll<SiteFinityFilteringQuestionSet>("filteringquestionsets")
                .Returns(Task.FromResult(new List<SiteFinityFilteringQuestionSet>
                {
                    new SiteFinityFilteringQuestionSet
                    {
                        Id = qsId,
                        Title = "Default",
                        LastUpdated = sitefinityLastUpdated
                    }
                }));

            _siteFinityHttpService.Get<List<SiteFinityFilteringQuestion>>(
                $"filteringquestionsets({qsId})/Questions?$expand=RelatedSkill")
                .Returns(Task.FromResult(new List<SiteFinityFilteringQuestion>()));

            _questionSetRepository.GetCurrentQuestionSet("filtered").Returns(Task.FromResult(
                new QuestionSet
                {
                    AssessmentType = "filtered",
                    QuestionSetVersion = "default-1",
                    Title = "default",
                    MaxQuestions = 3,
                    Version = 1,
                    LastUpdated = localLastUpdated,
                    IsCurrent = true
                }));

            await _sut.RunOnce(_logger);
            
            _logger.WasCalledOnce(LogLevel.Information, $"Question set {qsId} doesn't have any questions");
        }
    }
}