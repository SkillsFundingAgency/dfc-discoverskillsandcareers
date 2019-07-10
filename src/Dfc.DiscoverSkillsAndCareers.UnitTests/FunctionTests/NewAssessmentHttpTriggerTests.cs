﻿using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
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
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.AssessmentApi;
using Dfc.DiscoverSkillsAndCareers.Models;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Dfc.UnitTests.FunctionTests
{
    public class NewAssessmentHttpTriggerTests : IDisposable
    {
        private HttpRequest _request;
        private ILogger _log;
        private IHttpRequestHelper _httpRequestHelper;
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private IUserSessionRepository _userSessionRepository;
        private IQuestionSetRepository _questionSetRepository;
        private IOptions<AppSettings> _optsAppSettings;
        
        public NewAssessmentHttpTriggerTests()
        {
            _request = new DefaultHttpRequest(new DefaultHttpContext());
            _log = Substitute.For<ILogger>();
            _httpRequestHelper = Substitute.For<IHttpRequestHelper>();
            _httpResponseMessageHelper = Substitute.For<IHttpResponseMessageHelper>();
            _userSessionRepository = Substitute.For<IUserSessionRepository>();
            _questionSetRepository = Substitute.For<IQuestionSetRepository>();
            _optsAppSettings = Options.Create(new AppSettings { SessionSalt = "ncs" });
        }

        public void Dispose()
        {
            _httpRequestHelper = null;
            _httpResponseMessageHelper = null;
            _userSessionRepository = null;
        }

      

        private async Task<HttpResponseMessage> RunFunction()
        {
            return await NewAssessmentHttpTrigger.Run(
                _request,
                _log,
                _httpRequestHelper,
                _httpResponseMessageHelper,
                _userSessionRepository,
                _questionSetRepository,
                _optsAppSettings
            ).ConfigureAwait(false);
        }

        [Fact]
        public async Task NewSessionHttpTrigger_WithMissingOptions_ShouldReturnBadRequest()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            var result = await RunFunction();

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
        
        [Fact]
        public async Task NewSessionHttpTrigger_WithMissingQuestionSet_ShouldReturnNoContent()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            _questionSetRepository.GetCurrentQuestionSet("short").Returns(Task.FromResult<QuestionSet>(null));
            
            _request.QueryString = new QueryString("?assessmentType=short");

            var result = await RunFunction();

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }
        
        
        
        [Fact]
        public async Task NewSessionHttpTrigger_OnException_ShouldReturn500()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            _questionSetRepository.GetCurrentQuestionSet("short").Throws(new Exception());
            
            _request.QueryString = new QueryString("?assessmentType=short");

            var result = await RunFunction();

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }
        

        [Fact]
        public async Task NewSessionHttpTrigger_With_ShouldReturnNewSessionId()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            _questionSetRepository.GetCurrentQuestionSet("short").Returns(Task.FromResult(new QuestionSet
            {
                AssessmentType = "short",
                QuestionSetVersion = "QS-1",
                MaxQuestions = 5
            }));
            
            _request.QueryString = new QueryString("?assessmentType=short");

            var result = await RunFunction();
            var content = await result.Content.ReadAsAsync<FilterSessionResponse>();

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.NotEmpty(content.SessionId);
        }
    }
}
