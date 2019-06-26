using Dfc.DiscoverSkillsAndCareers.Repositories;
using DFC.HTTP.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.ResultsFunctionApp.ResultsApi;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Dfc.UnitTests.FunctionTests
{
    public class ResultsHttpTriggerTests : IDisposable
    {
        public ResultsHttpTriggerTests()
        {
            _request = new DefaultHttpRequest(new DefaultHttpContext());
            _log = Substitute.For<ILogger>();
            _httpRequestHelper = Substitute.For<IHttpRequestHelper>();
            _httpResponseMessageHelper = Substitute.For<IHttpResponseMessageHelper>();
            _userSessionRepository = Substitute.For<IUserSessionRepository>();
            _jobProfileRepository = Substitute.For<IJobProfileRepository>();
            _questionSetRepository = Substitute.For<IQuestionSetRepository>();
    
        }

        public void Dispose()
        {
            _httpRequestHelper = null;
            _httpResponseMessageHelper = null;
            _userSessionRepository = null;
            _jobProfileRepository = null;
        }

        private HttpRequest _request;
        private ILogger _log;
        private IHttpRequestHelper _httpRequestHelper;
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private IUserSessionRepository _userSessionRepository;
        private IJobProfileRepository _jobProfileRepository;
        private IQuestionSetRepository _questionSetRepository;

        private async Task<HttpResponseMessage> RunFunction(string sessionId, string jobCategory = null)
        {
            return await ResultsHttpTrigger.Run(
                _request,
                sessionId,
                jobCategory,
                _log,
                _httpRequestHelper,
                _httpResponseMessageHelper,
                _userSessionRepository,
                _jobProfileRepository
            ).ConfigureAwait(false);
        }

        [Fact]
        public async Task ResultsHttpTriggerTest_WithMissingSessionId_ShouldReturnStatusCodeBadRequest()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            var result = await RunFunction(null);

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
        
        [Fact]
        public async Task ResultsHttpTriggerTest_OnException_ShouldReturn500()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            _userSessionRepository.GetUserSession("session1").Throws(new Exception());
            
            var result = await RunFunction("session1");

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Fact]
        public async Task ResultsHttpTriggerTest_WithInvalidSessionId_ShouldReturnStatusCodeNoContent()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            var result = await RunFunction("invalid-session-id");

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Fact]
        public async Task GetResultHttpTrigger_WithIncompleteSession_ShouldReturnStatusCodeBadRequest()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            _userSessionRepository.GetUserSession("session1").Returns(Task.FromResult(new UserSession
            {
                ResultData = null
            }));
            
            var result = await RunFunction("session1");

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetResultHttpTrigger_WithCompletedSession_ShouldReturnStatusCodeOK()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            _questionSetRepository.GetCurrentQuestionSet("filtered").Returns(Task.FromResult(new QuestionSet()));
            
            _userSessionRepository.GetUserSession("session1").Returns(Task.FromResult(new UserSession
            {
                ResultData = new ResultData
                {
                    JobCategories = new []
                    {
                        new JobCategoryResult
                        {
                            JobCategoryName = "Managerial",
                            FilterAssessmentResult = new FilterAssessmentResult
                            {
                                SuggestedJobProfiles =  new List<string> { "Jp1" }
                            }
                        }
                    },
                    Traits = new [] { new TraitResult { TraitCode = "LEADER", TotalScore = 8 },  }
                }
            }));

            _jobProfileRepository.JobProfilesTitle(Arg.Is<List<string>>(v => v.Contains("Jp1"))).Returns(Task.FromResult(new[]
            {
                new JobProfile {Title = "Jp1 "},
            }));

            var result = await RunFunction("session1", "Managerial");

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
