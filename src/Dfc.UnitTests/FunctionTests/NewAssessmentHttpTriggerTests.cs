using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models;
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
using System.Security;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.AssessmentApi;
using Xunit;

namespace Dfc.UnitTests.FunctionTests
{
    public class NewAssessmentHttpTriggerTests : IDisposable
    {
        public NewAssessmentHttpTriggerTests()
        {
            _request = new DefaultHttpRequest(new DefaultHttpContext());
            _log = Substitute.For<ILogger>();
            _httpRequestHelper = Substitute.For<IHttpRequestHelper>();
            _httpResponseMessageHelper = Substitute.For<IHttpResponseMessageHelper>();
            _userSessionRepository = Substitute.For<IUserSessionRepository>();
            _questionRepository = Substitute.For<IQuestionRepository>();
            _questionSetRepository = Substitute.For<IQuestionSetRepository>();
            _optsAppSettings = Options.Create(new AppSettings { SessionSalt = "ncs" });
        }

        public void Dispose()
        {
            _httpRequestHelper = null;
            _httpResponseMessageHelper = null;
            _userSessionRepository = null;
        }

        private HttpRequest _request;
        private ILogger _log;
        private IHttpRequestHelper _httpRequestHelper;
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private IUserSessionRepository _userSessionRepository;
        private IQuestionRepository _questionRepository;
        private IQuestionSetRepository _questionSetRepository;
        private IOptions<AppSettings> _optsAppSettings;

        private async Task<HttpResponseMessage> RunFunction()
        {
            return await NewAssessmentHttpTrigger.Run(
                _request,
                _log,
                _httpRequestHelper,
                _httpResponseMessageHelper,
                _userSessionRepository,
                _questionRepository,
                _questionSetRepository,
                _optsAppSettings
            ).ConfigureAwait(false);
        }

        [Fact]
        public async Task NewSessionHttpTrigger_WithMissingOptions_ShouldReturnBadRequest()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            _userSessionRepository = new FakeUserSessionRepository();
            _questionRepository = new FakeQuestionRepository();

            var result = await RunFunction();

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task NewSessionHttpTrigger_With_ShouldReturnNewSessionId()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            _userSessionRepository = new FakeUserSessionRepository();
            _questionRepository = new FakeQuestionRepository();
            _request.QueryString = new QueryString("?assessmentType=short");
            _questionSetRepository = new FakeQuestionSetRepository();

            var result = await RunFunction();
            var content = await result.Content.ReadAsAsync<DscSession>();

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.NotEmpty(content.SessionId);
        }
    }
}
