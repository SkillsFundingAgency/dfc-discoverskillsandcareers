using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.QuestionsFunctionApp.QuestionApi;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using DFC.HTTP.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Dfc.UnitTests.FunctionTests
{
    public class QuestionSetHttpTriggerTests : IDisposable
    {
        public QuestionSetHttpTriggerTests()
        {
            _request = new DefaultHttpRequest(new DefaultHttpContext());
            _log = Substitute.For<ILogger>();
            _httpRequestHelper = Substitute.For<IHttpRequestHelper>();
            _httpResponseMessageHelper = Substitute.For<IHttpResponseMessageHelper>();
            _questionSetRepository = Substitute.For<IQuestionSetRepository>();
        }

        public void Dispose()
        {
            _httpRequestHelper = null;
            _httpResponseMessageHelper = null;
        }

        private HttpRequest _request;
        private ILogger _log;
        private IHttpRequestHelper _httpRequestHelper;
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private IQuestionSetRepository _questionSetRepository;

        private async Task<HttpResponseMessage> RunFunction(string assessmentType, string title, int version)
        {
            return await QuestionSetHttpTrigger.Run(
                _request,
                assessmentType,
                title,
                version,
                _log,
                _httpRequestHelper,
                _httpResponseMessageHelper,
                _questionSetRepository
            ).ConfigureAwait(false);
        }

        [Fact]
        public async Task QuestionSetTrigger_Should_ReturnQuestionSet()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            _questionSetRepository.GetQuestionSetVersion("short", "default", 1)
                .Returns(Task.FromResult(new QuestionSet { Title = "default" }));
            
            var result = await RunFunction("short", "default", 1);

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
        
        [Fact]
        public async Task QuestionSetTrigger_ShouldReturn_500OnException()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            _questionSetRepository.GetQuestionSetVersion("short", "default", 1)
                .Throws(new Exception());
            
            var result = await RunFunction("short", "default", 1);

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }
        
        [Fact]
        public async Task QuestionSetTrigger_ShouldReturn_NoContentIfNoQuestionSet()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            _questionSetRepository.GetQuestionSetVersion("short", "default", 1)
                .Returns(Task.FromResult<QuestionSet>(null));
            
            var result = await RunFunction("short", "default", 1);

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }
    }

}