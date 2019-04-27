using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Dfc.UnitTests.Fakes;
using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NSubstitute.ReturnsExtensions;
using Xunit;
using Xunit.Abstractions;

namespace Dfc.UnitTests.FunctionTests
{
    public class AssessmentQuestionHttpTriggerTests : IDisposable
    {
        private HttpRequest _request;
        private ILogger _log;
        private IHttpRequestHelper _httpRequestHelper;
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private IUserSessionRepository _userSessionRepository;
        private IQuestionRepository _questionRepository;
        private IOptions<AppSettings> _optsAppSettings;
        
        public AssessmentQuestionHttpTriggerTests()
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

        private async Task<HttpResponseMessage> RunFunction(string sessionId,string assessment, int questionNumber)
        {
            return await AssessmentQuestionHttpTrigger.Run(
                _request,
                sessionId,
                assessment,
                questionNumber,
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
            _userSessionRepository = new FakeUserSessionRepository();
            _questionRepository = new FakeQuestionRepository();

            var result = await RunFunction("201901-session1", "short", 1);
            var content = await result.Content.ReadAsAsync<AssessmentQuestionResponse>();

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(1, content.QuestionNumber);
        }
        
        [Fact]
        public async Task ShouldReturnNoContentIfQuestionOutOfSet()
        {
            _userSessionRepository = new FakeUserSessionRepository();
            _questionRepository.GetQuestion(41, Arg.Any<string>()).Returns(Task.FromResult<Question>(null));

            var result = await RunFunction("201901-session1", "short", 41);
            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Fact]
        public async Task AssessmentQuestionHttpTrigger_WithMissingSessionId_ShouldReturnBadRequest()
        {
            
            
            
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            var result = await RunFunction(null,"short",1);

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task AssessmentQuestionHttpTrigger_WithInvalidSessionId_ShouldReturnNoContent()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            var result = await RunFunction("invalid-session-id","short",1);

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Fact]
        public async Task AssessmentQuestionHttpTrigger_WithMissingQuestion_ShouldReturnNoContent()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            _userSessionRepository = new FakeUserSessionRepository();

            var result = await RunFunction("invalid-session-id","short",1);

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }
    }
}
