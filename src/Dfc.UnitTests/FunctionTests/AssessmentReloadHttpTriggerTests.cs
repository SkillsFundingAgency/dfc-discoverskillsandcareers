using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.AssessmentApi;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using DFC.HTTP.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Dfc.UnitTests.FunctionTests
{
    public class AssessmentReloadHttpTriggerTests : IDisposable
    {
        private HttpRequest _request;
        private ILogger _log;
        private IHttpRequestHelper _httpRequestHelper;
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private IUserSessionRepository _userSessionRepository;
        private IQuestionRepository _questionRepository;
        private IOptions<AppSettings> _optsAppSettings;
        
        public AssessmentReloadHttpTriggerTests()
        {
            _request = new DefaultHttpRequest(new DefaultHttpContext());
            _log = Substitute.For<ILogger>();
            _httpRequestHelper = Substitute.For<IHttpRequestHelper>();
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            _userSessionRepository = Substitute.For<IUserSessionRepository>();
            _questionRepository = Substitute.For<IQuestionRepository>();
            _optsAppSettings = Options.Create(new AppSettings { SessionSalt = "ncs" });
        }

        public void Dispose()
        {
            _httpRequestHelper = null;
            _httpResponseMessageHelper = null;
            _userSessionRepository = null;
            _userSessionRepository = null;
        }

        private async Task<HttpResponseMessage> RunFunction(string sessionId)
        {
            return await AssessmentReloadHttpTrigger.Run(
                _request,
                sessionId,
                _log,
                _httpRequestHelper,
                _httpResponseMessageHelper,
                _userSessionRepository,
                _questionRepository,
                _optsAppSettings
            ).ConfigureAwait(false);
        }

        [Fact]
        public async Task ShouldReturnQuestionThatIsRequested()
        {
            _userSessionRepository.GetUserSession(Arg.Any<string>()).Returns(Task.FromResult(new UserSession()
            {
                AssessmentState = new AssessmentState
                {
                    MaxQuestions = 5,
                    CurrentQuestion =1,
                    QuestionSetVersion = "question-set"
                }
            }));
            
            _questionRepository.GetQuestion(1, "question-set")
                .Returns(Task.FromResult(new Question
                {
                    QuestionId = "1",
                    Texts = new []
                    {
                        new QuestionText() { LanguageCode = "en", Text = "Unit test question" }
                    },
                    Order = 1,
                    TraitCode = "DOER"
                }));

            var result = await RunFunction("201901-session1");
            var content = await result.Content.ReadAsAsync<AssessmentQuestionResponse>();

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(1, content.QuestionNumber);
            Assert.Equal(1, content.NextQuestionNumber);
        }

        [Fact]
        public async Task ShouldReturnBadRequestIfCouldNotDecodeSessionId()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            var result = await RunFunction("234");

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
        
        [Fact]
        public async Task ShouldReturnOkWithCorrectSessionIdAndPartitionKey()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            _userSessionRepository.GetUserSession("201904-282gk265gzmzyz").Returns(Task.FromResult(new UserSession()
            {
                PartitionKey = "201904",
                UserSessionId = "282gk265gzmzyz",
                AssessmentState = new AssessmentState
                {
                    CurrentQuestion = 1,
                    MaxQuestions = 5,
                    QuestionSetVersion = "question-set"
                }
            }));
            
            _questionRepository.GetQuestion(1, "question-set")
                .Returns(Task.FromResult(new Question
                {
                    QuestionId = "1",
                    Texts = new []
                    {
                        new QuestionText() { LanguageCode = "en", Text = "Unit test question" }
                    },
                    Order = 1,
                    TraitCode = "DOER"
                }));

            var result = await RunFunction("282gk265gzmzyz");

            Assert.IsType<HttpResponseMessage>(result);
            var content = await result.Content.ReadAsAsync<AssessmentQuestionResponse>();
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("201904-282gk265gzmzyz", content.SessionId);
        }
        

        [Fact]
        public async Task ShouldReturnBadRequestWithMissingSessionId()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            var result = await RunFunction(null);

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
        
        [Fact]
        public async Task ShouldReturnBadRequestWithMissingSessionSalt()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            _optsAppSettings = Options.Create(new AppSettings { SessionSalt = null });

            var result = await RunFunction("201904-282gk265gzmzyz");

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnNoContentWithInvalidSessionId()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            var result = await RunFunction("invalid-session-id");

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }
    }
}