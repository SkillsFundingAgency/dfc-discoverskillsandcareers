using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.AssessmentApi;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using DFC.HTTP.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace Dfc.UnitTests.FunctionTests
{
    public class SendSessionSmsHttpTriggerTests : IDisposable
    {
        private ILogger _log;
        private IHttpRequestHelper _httpRequestHelper;
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private IUserSessionRepository _userSessionRepository;
        private INotifyClient _notifyClient;

        public SendSessionSmsHttpTriggerTests()
        {
            _log = Substitute.For<ILogger>();
            _httpRequestHelper = Substitute.For<IHttpRequestHelper>();
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            _userSessionRepository = Substitute.For<IUserSessionRepository>();
            _notifyClient = Substitute.For<INotifyClient>();
        }
        
        private async Task<HttpResponseMessage> RunFunction(SendSessionSmsRequest body)
        {
            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = new MemoryStream()
            };

            if (body != null)
            {
                var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
                request.Body.Write(bytes, 0, bytes.Length);
                request.Body.Seek(0, SeekOrigin.Begin);
            }
            
            return await SendSessionSmsHttpTrigger.Run(
                request,
                _log,
                _httpRequestHelper,
                _httpResponseMessageHelper,
                _userSessionRepository,
                _notifyClient
            ).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _httpRequestHelper = null;
            _httpResponseMessageHelper = null;
            _userSessionRepository = null;
            _userSessionRepository = null;
        }
        
        [Fact]
        public async Task ShouldReturnBadRequestWithEmptyBody()
        {
            var result = await RunFunction(null);

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
        
        [Fact]
        public async Task ShouldReturnBadRequestWithNoSessionId()
        {

            var result = await RunFunction(new SendSessionSmsRequest
            {
                Domain = "discover-skills-and-careers.co.uk",
                MobileNumber = "0123456789",
                SessionId = "",
                TemplateId = "3B840739-F4C5-40AA-93E9-6827A6F29003"
            });

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
        
        [Fact]
        public async Task ShouldReturnBadRequestWithNoMobileNumber()
        {

            var result = await RunFunction(new SendSessionSmsRequest
            {
                Domain = "discover-skills-and-careers.co.uk",
                MobileNumber = "",
                SessionId = "201904-282gk265gzmzyz",
                TemplateId = "3B840739-F4C5-40AA-93E9-6827A6F29003"
            });

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
        
        [Fact]
        public async Task ShouldReturnBadRequestWithNoDomain()
        {

            var result = await RunFunction(new SendSessionSmsRequest
            {
                Domain = "",
                MobileNumber = "0123456789",
                SessionId = "201904-282gk265gzmzyz",
                TemplateId = "3B840739-F4C5-40AA-93E9-6827A6F29003"
            });

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
        
        [Fact]
        public async Task ShouldReturnBadRequestWithNoTemplateId()
        {

            var result = await RunFunction(new SendSessionSmsRequest
            {
                Domain = "discover-skills-and-careers.co.uk",
                MobileNumber = "0123456789",
                SessionId = "201904-282gk265gzmzyz",
                TemplateId = ""
            });

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
        
        [Fact]
        public async Task ShouldReturnOkWithValidRequest()
        {
            var body = new SendSessionSmsRequest
            {
                Domain = "discover-skills-and-careers.co.uk",
                MobileNumber = "0123456789",
                SessionId = "201904-282gk265gzmzyz",
                TemplateId = "3B840739-F4C5-40AA-93E9-6827A6F29003"
            };

            _userSessionRepository.GetUserSession("201904-282gk265gzmzyz").Returns(Task.FromResult(new UserSession
            {
                UserSessionId = "201904-282gk265gzmzyz"
            }));

            _notifyClient.SendSms(body.Domain, body.MobileNumber, body.TemplateId, body.SessionId).Returns(Task.CompletedTask);
            
            var result = await RunFunction(body);

            Assert.IsType<HttpResponseMessage>(result);
            var content = await result.Content.ReadAsAsync<SendSessionSmsResponse>();
            
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.True(content.IsSuccess);
        }
        
        
    }
}