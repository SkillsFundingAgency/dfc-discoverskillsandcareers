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
    public class NewFilterAssessmentHttpTriggerTests : IDisposable
    {
        private HttpRequest _request;
        private ILogger _log;
        private IHttpRequestHelper _httpRequestHelper;
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private IUserSessionRepository _userSessionRepository;

        public NewFilterAssessmentHttpTriggerTests()
        {
            _request = new DefaultHttpRequest(new DefaultHttpContext());
            _log = Substitute.For<ILogger>();
            _httpRequestHelper = Substitute.For<IHttpRequestHelper>();
            _httpResponseMessageHelper = Substitute.For<IHttpResponseMessageHelper>();
            _userSessionRepository = Substitute.For<IUserSessionRepository>();
        }

        public void Dispose()
        {
            _httpRequestHelper = null;
            _httpResponseMessageHelper = null;
            _userSessionRepository = null;
        }
        
        private async Task<HttpResponseMessage> RunFunction(string sessionId, string jobCategory)
        {
            return await NewFilterAssessmentHttpTrigger.Run(
                _request,
                sessionId,
                jobCategory,
                _log,
                _httpRequestHelper,
                _httpResponseMessageHelper,
                _userSessionRepository
            ).ConfigureAwait(false);
        }

        [Fact]
        public async Task NewFilterSessionHttpTrigger_WithMissingJobCategory_ShouldReturnBadRequest()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            
            _userSessionRepository.GetUserSession("session1").Returns(Task.FromResult(new UserSession
            {
                PartitionKey = "201901",
                ResultData = new ResultData
                {
                    JobCategories = new []
                    {
                        new JobCategoryResult
                        {
                            JobCategoryName = "Social Care",
                            
                        }
                    }
                }
            })); 
            
            var result = await RunFunction("session1", "social-care");

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task NewFilterSessionHttpTrigger_With_ShouldReturnSameSessionId()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            
            _userSessionRepository.GetUserSession("session1").Returns(Task.FromResult(new UserSession
            {
                PartitionKey = "201901",
                UserSessionId = "session1",
                ResultData = new ResultData
                {
                    JobCategories = new []
                    {
                        new JobCategoryResult
                        {
                            JobCategoryName = "Social Care",
                            
                        }
                    }
                }
            })); 

            var result = await RunFunction("session1", "social-care");
            var content = await result.Content.ReadAsAsync<DscSession>();

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("201901-session1",content.SessionId);
        }
    }
}