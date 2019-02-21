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
using NSubstitute;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Dfc.UnitTests.FunctionTests
{
    public class NextQuestionHttpTriggerTests : IDisposable
    {
        public NextQuestionHttpTriggerTests()
        {
            _request = new DefaultHttpRequest(new DefaultHttpContext());
            _log = Substitute.For<ILogger>();
            _loggerHelper = Substitute.For<ILoggerHelper>();
            _httpRequestHelper = Substitute.For<IHttpRequestHelper>();
            _httpResponseMessageHelper = Substitute.For<IHttpResponseMessageHelper>();
            _userSessionRepository = Substitute.For<IUserSessionRepository>();
            _questionRepository = Substitute.For<IQuestionRepository>();
        }

        public void Dispose()
        {
            _httpRequestHelper = null;
            _httpResponseMessageHelper = null;
            _userSessionRepository = null;
            _userSessionRepository = null;
        }

        private HttpRequest _request;
        private ILogger _log;
        private ILoggerHelper _loggerHelper;
        private IHttpRequestHelper _httpRequestHelper;
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private IUserSessionRepository _userSessionRepository;
        private IQuestionRepository _questionRepository;

        private async Task<HttpResponseMessage> RunFunction(string sessionId)
        {
            return await NextQuestionHttpTrigger.Run(
                _request,
                sessionId,
                _log,
                _loggerHelper,
                _httpRequestHelper,
                _httpResponseMessageHelper,
                _userSessionRepository,
                _questionRepository
            ).ConfigureAwait(false);
        }

        [Fact]
        public async Task NextQuestionHttpTrigger_With_ShouldReturnNewSessionId()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            _userSessionRepository = new FakeUserSessionRepository();
            _questionRepository = new FakeQuestionRepository();

            var result = await RunFunction("session1");
            var content = await result.Content.ReadAsAsync<NextQuestionResponse>();

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.NotEmpty(content.QuestionId);
        }

        [Fact]
        public async Task NextQuestionHttpTrigger_WithMissingSessionId_ShouldReturnBadRequest()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            var result = await RunFunction(null);

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task NextQuestionHttpTrigger_WithInvalidSessionId_ShouldReturnNoContent()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            var result = await RunFunction("invalid-session-id");

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Fact]
        public async Task NextQuestionHttpTrigger_WithMissingQuestion_ShouldReturnNoContent()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            _userSessionRepository = new FakeUserSessionRepository();

            var result = await RunFunction("invalid-session-id");

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }
    }
}
