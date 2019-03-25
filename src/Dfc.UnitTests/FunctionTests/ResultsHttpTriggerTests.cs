﻿using Dfc.DiscoverSkillsAndCareers.ResultsFunctionApp;
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

namespace Dfc.UnitTests.FunctionTests
{
    public class ResultsHttpTriggerTests : IDisposable
    {
        public ResultsHttpTriggerTests()
        {
            _request = new DefaultHttpRequest(new DefaultHttpContext());
            _log = Substitute.For<ILogger>();
            _loggerHelper = Substitute.For<ILoggerHelper>();
            _httpRequestHelper = Substitute.For<IHttpRequestHelper>();
            _httpResponseMessageHelper = Substitute.For<IHttpResponseMessageHelper>();
            _userSessionRepository = Substitute.For<IUserSessionRepository>();
            _jobProfileRepository = Substitute.For<IJobProfileRepository>();
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
        private ILoggerHelper _loggerHelper;
        private IHttpRequestHelper _httpRequestHelper;
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private IUserSessionRepository _userSessionRepository;
        private IJobProfileRepository _jobProfileRepository;

        private async Task<HttpResponseMessage> RunFunction(string sessionId)
        {
            return await ResultsHttpTrigger.Run(
                _request,
                sessionId,
                _log,
                _loggerHelper,
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
        public async Task ResultsHttpTriggerTest_WithInvalidSessionId_ShouldReturnStatusCodeNoContent()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            var result = await RunFunction("invalid-session-id");

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Fact]
        public async Task GetContentHttpTrigger_WithIncompleteSession_ShouldReturnStatusCodeBadRequest()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            _userSessionRepository = new FakeUserSessionRepository();

            var result = await RunFunction("session1");

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetContentHttpTrigger_WithCompletedSession_ShouldReturnStatusCodeOK()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            _userSessionRepository = new FakeCompletedUserSessionRepository();

            var result = await RunFunction("session1");

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
