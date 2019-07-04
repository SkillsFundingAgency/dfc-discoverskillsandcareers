using System;
using System.Collections.Generic;
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
    public class ShortQuestionSetDataProcessorTests
    {
        private IQuestionRepository _questionRepository;
        private IQuestionSetRepository _questionSetRepository;
        private ISiteFinityHttpService _siteFinityHttpService;
        private ILogger _logger;
        
        private ShortQuestionSetDataProcessor _sut;
        
        public ShortQuestionSetDataProcessorTests()
        {
            _questionRepository = Substitute.For<IQuestionRepository>();
            _questionSetRepository = Substitute.For<IQuestionSetRepository>();
            _siteFinityHttpService = Substitute.For<ISiteFinityHttpService>();
            _logger = Substitute.For<ILogger>();

            _sut = new ShortQuestionSetDataProcessor(_siteFinityHttpService, _questionRepository, _questionSetRepository);
        }

        [Fact]
        public async Task IfNoQuestionSets_ShouldReturn_AndLog()
        {
            _siteFinityHttpService.GetAll<SiteFinityShortQuestionSet>("shortquestionsets")
                .Returns(Task.FromResult<List<SiteFinityShortQuestionSet>>(null));

            await _sut.RunOnce(_logger);
            
            _logger.WasCalledOnce(LogLevel.Error, "No Short Question sets available");
        }

        [Fact]
        public async Task IfQuestionSetDoesNotRequireUpdate_ShouldLog()
        {
            var qsId = Guid.NewGuid().ToString();
            var localLastUpdated = new DateTimeOffset(2019, 6, 1, 0, 0, 0, TimeSpan.Zero);
            var sitefinityLastUpdated = localLastUpdated.AddDays(-1);
            
            _siteFinityHttpService.GetAll<SiteFinityShortQuestionSet>("shortquestionsets")
                .Returns(Task.FromResult(new List<SiteFinityShortQuestionSet>
                {
                    new SiteFinityShortQuestionSet
                    {
                        Id = qsId,
                        Title = "Default",
                        LastUpdated = sitefinityLastUpdated
                    }
                }));

            _questionSetRepository.GetLatestQuestionSetByTypeAndKey("short", "Default").Returns(Task.FromResult(
                new QuestionSet
                {
                    AssessmentType = "short",
                    QuestionSetVersion = "default-1",
                    Title = "default",
                    MaxQuestions = 3,
                    Version = 1,
                    LastUpdated = localLastUpdated,
                    IsCurrent = true
                }));
            
            await _sut.RunOnce(_logger);
            
            _logger.WasCalledOnce(LogLevel.Information, $"Question set {qsId} Default is upto date - no changes to be done");
        }
        
        [Fact]
        public async Task IfNoQuestionsForQuestionSet_ShouldLog()
        {
            var qsId = Guid.NewGuid().ToString();
            var localLastUpdated = new DateTimeOffset(2019, 6, 1, 0, 0, 0, TimeSpan.Zero);
            var sitefinityLastUpdated = localLastUpdated.AddDays(1);
            
            _siteFinityHttpService.GetAll<SiteFinityShortQuestionSet>("shortquestionsets")
                .Returns(Task.FromResult(new List<SiteFinityShortQuestionSet>
                {
                    new SiteFinityShortQuestionSet
                    {
                        Id = qsId,
                        Title = "Default",
                        LastUpdated = sitefinityLastUpdated
                    }
                }));

            _siteFinityHttpService.Get<List<SiteFinityShortQuestion>>(
                    $"shortquestionsets({qsId})/Questions?$expand=Trait")
                .Returns(Task.FromResult(new List<SiteFinityShortQuestion>()));

            
            _questionSetRepository.GetCurrentQuestionSet("short").Returns(Task.FromResult(
                new QuestionSet
                {
                    AssessmentType = "short",
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
        
        [Fact]
        public async Task ShouldSetQuestionSetAsNotCurrentAndShouldLog()
        {
            var qsId = Guid.NewGuid().ToString();
            var localLastUpdated = new DateTimeOffset(2019, 6, 1, 0, 0, 0, TimeSpan.Zero);
            var sitefinityLastUpdated = localLastUpdated.AddDays(1);
            
            _siteFinityHttpService.GetAll<SiteFinityShortQuestionSet>("shortquestionsets")
                .Returns(Task.FromResult(new List<SiteFinityShortQuestionSet>
                {
                    new SiteFinityShortQuestionSet
                    {
                        Id = qsId,
                        Title = "Default",
                        LastUpdated = sitefinityLastUpdated
                    }
                }));


            _siteFinityHttpService.Get<List<SiteFinityShortQuestion>>(
                    $"shortquestionsets({qsId})/Questions?$expand=Trait")
                .Returns(Task.FromResult(new List<SiteFinityShortQuestion>
                {
                    new SiteFinityShortQuestion { Trait = new SiteFinityTrait { Name = "Leader" }}
                }));

            _questionSetRepository.GetLatestQuestionSetByTypeAndKey("short", "Default").Returns(Task.FromResult(
                new QuestionSet
                {
                    AssessmentType = "short",
                    QuestionSetVersion = "default-as-1",
                    Title = "default",
                    MaxQuestions = 3,
                    Version = 1,
                    LastUpdated = localLastUpdated,
                    IsCurrent = true
                }));

            
            _questionSetRepository.GetCurrentQuestionSet("short").Returns(Task.FromResult(
                new QuestionSet
                {
                    AssessmentType = "short",
                    QuestionSetVersion = "default-1",
                    Title = "default",
                    MaxQuestions = 3,
                    Version = 1,
                    LastUpdated = localLastUpdated,
                    IsCurrent = true
                }));

            await _sut.RunOnce(_logger);

            await _questionSetRepository.Received(2).CreateOrUpdateQuestionSet(Arg.Is<QuestionSet>(q => !q.IsCurrent));
            _logger.WasCalledOnce(LogLevel.Information, $"Demoting question set default-1 from current");
            _logger.WasCalledOnce(LogLevel.Information, $"Demoting question set default-as-1 from current");
        }
    }
}