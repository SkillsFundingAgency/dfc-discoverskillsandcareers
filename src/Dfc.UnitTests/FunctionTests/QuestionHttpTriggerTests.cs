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
    public class QuestionHttpTriggerTests : IDisposable
    {
        public QuestionHttpTriggerTests()
        {
            _request = new DefaultHttpRequest(new DefaultHttpContext());
            _log = Substitute.For<ILogger>();
            _httpRequestHelper = Substitute.For<IHttpRequestHelper>();
            _httpResponseMessageHelper = Substitute.For<IHttpResponseMessageHelper>();
            _questionRepository = Substitute.For<IQuestionRepository>();
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
        private IQuestionRepository _questionRepository;

        private async Task<HttpResponseMessage> RunFunction(string assessmentType, string title, int version)
        {
            return await QuestionsHttpTrigger.Run(
                _request,
                assessmentType,
                title,
                version,
                _log,
                _httpRequestHelper,
                _httpResponseMessageHelper,
                _questionRepository
            ).ConfigureAwait(false);
        }

        
        
        [Fact]
        public async Task QuestionTrigger_Should_ReturnQuestionSet()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            _questionRepository.GetQuestions("short", "default", 1)
                .Returns(Task.FromResult(new [] { new Question { QuestionId = "1" },  }));
            
            var result = await RunFunction("short", "default", 1);

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
        
        [Fact]
        public async Task QuestionSetTrigger_ShouldReturn_500OnException()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            _questionRepository.GetQuestions("short", "default", 1)
                .Throws(new Exception());
            
            var result = await RunFunction("short", "default", 1);

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }
        
        [Fact]
        public async Task QuestionSetTrigger_ShouldReturn_NoContentIfNoQuestionSet()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            _questionRepository.GetQuestions("short", "default", 1)
                .Returns(Task.FromResult<Question[]>(null));
            
            var result = await RunFunction("short", "default", 1);

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }
    }
}