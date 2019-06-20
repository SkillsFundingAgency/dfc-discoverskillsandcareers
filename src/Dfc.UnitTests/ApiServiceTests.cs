using System;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace Dfc.UnitTests
{
    public class ApiServiceTests
    {
        private IHttpService _httpService;
        private IOptions<AppSettings> _appSettings;
        private const string ApiRoot = "https://localhost:8080/api";
        private ApiServices _api;

        public ApiServiceTests()
        {
            _httpService = Substitute.For<IHttpService>();
            _appSettings = Substitute.For<IOptions<AppSettings>>();

            _appSettings.Value.Returns(new AppSettings
            {
                SessionApiRoot = ApiRoot,
                ResultsApiRoot = ApiRoot
            });
            
            _api = new ApiServices(_httpService, _appSettings);
        }

        [Fact]
        public async Task NewSession_ShouldCall_CorrectUrl()
        {
            var guid = Guid.NewGuid();
            var url = $"{ApiRoot}/assessment?assessmentType=short";
            var response = new NewSessionResponse
            {
                SessionId = "Abc123"
            };
            
            _httpService.PostData(url, "", guid).Returns(Task.FromResult(JsonConvert.SerializeObject(response)));

            var result = await _api.NewSession(guid, "short");
            
            Assert.Equal(response.SessionId, result.SessionId);
        }
        
        [Fact]
        public async Task StartFilteredForJobCategory_ShouldCall_CorrectUrl()
        {
            var guid = Guid.NewGuid();
            var url = $"{ApiRoot}/assessment/filtered/Abc123/animal-care";
            var response = new NewSessionResponse
            {
                SessionId = "Abc123"
            };
            
            _httpService.PostData(url, "", guid).Returns(Task.FromResult(JsonConvert.SerializeObject(response)));

            var result = await _api.StartFilteredForJobCategory(guid, "Abc123", "animal-care");
            
            Assert.Equal(response.SessionId, result.SessionId);
        }
        
        [Fact]
        public async Task Reload_ShouldCall_CorrectUrl()
        {
            var guid = Guid.NewGuid();
            var url = $"{ApiRoot}/assessment/Abc123/reload";
            var response = new AssessmentQuestionResponse
            {
                SessionId = "Abc123"
            };
            
            _httpService.GetString(url, guid).Returns(Task.FromResult(JsonConvert.SerializeObject(response)));

            var result = await _api.Reload("Abc123", guid);
            
            Assert.Equal(response.SessionId, result.SessionId);
        }
        
        [Fact]
        public async Task Question_ShouldCall_CorrectUrl()
        {
            var guid = Guid.NewGuid();
            var sessionId = "Abc123";
            var assessment = "short";
            var questionNumber = 1;
            var url = $"{ApiRoot}/assessment/{sessionId}/{assessment}/q/{questionNumber}";
            var response = new AssessmentQuestionResponse
            {
                SessionId = sessionId
            };
            
            _httpService.GetString(url, guid).Returns(Task.FromResult(JsonConvert.SerializeObject(response)));

            var result = await _api.Question(sessionId, assessment, questionNumber, guid);
            
            Assert.Equal(response.SessionId, result.SessionId);
        }
        
        [Fact]
        public async Task PostAnswer_ShouldCall_CorrectUrl()
        {
            var guid = Guid.NewGuid();
            var sessionId = "Abc123";
            var url = $"{ApiRoot}/assessment/{sessionId}";

            var request = new PostAnswerRequest
            {
                SelectedOption = "3",
                QuestionId = "QS-1-1"
            };
            
            var response = new PostAnswerResponse()
            {
                NextQuestionNumber = 4
            };
            
            _httpService.PostData(url, request,  guid).Returns(Task.FromResult(JsonConvert.SerializeObject(response)));

            var result = await _api.PostAnswer(sessionId, request, guid);
            
            Assert.Equal(response.NextQuestionNumber, result.NextQuestionNumber);
        }
        
        [Fact]
        public async Task Results_ShouldCall_CorrectUrl()
        {
            var guid = Guid.NewGuid();
            var sessionId = "Abc123";
            var url = $"{ApiRoot}/result/{sessionId}/short";
            var response = new ResultsResponse
            {
                SessionId = sessionId
            };
            
            _httpService.GetString(url, guid).Returns(Task.FromResult(JsonConvert.SerializeObject(response)));

            var result = await _api.Results(sessionId, guid);
            
            Assert.Equal(response.SessionId, result.SessionId);
        }
        
        [Fact]
        public async Task ResultsForJobCategory_ShouldCall_CorrectUrl()
        {
            var guid = Guid.NewGuid();
            var url = $"{ApiRoot}/result/Abc123/animal-care";
            var response = new ResultsJobCategoryResult()
            {
                SessionId = "Abc123",
                JobProfiles = new []
                {
                    new JobProfile(), 
                }
            };
            
            _httpService.GetString(url, guid).Returns(Task.FromResult(JsonConvert.SerializeObject(response)));

            var result = await _api.ResultsForJobCategory("Abc123", "animal-care", guid);
            
            Assert.Equal(response.SessionId, result.SessionId);
            Assert.Single(response.JobProfiles);
        }
        
        [Fact]
        public async Task SendEmail_ShouldCall_CorrectUrl()
        {
            var guid = Guid.NewGuid();
            var sessionId = "Abc123";
            var url = $"{ApiRoot}/assessment/notify/email";

            
            var response = new NotifyResponse()
            {
                IsSuccess = true
            };
            
            _httpService.PostData(url, Arg.Any<object>(), guid).Returns(Task.FromResult(JsonConvert.SerializeObject(response)));

            var result = await _api.SendEmail("disco", "my@email.com", "0", sessionId, guid);
            
            Assert.Equal(response.IsSuccess, result.IsSuccess);
        }
        
        
        [Fact]
        public async Task SendSms_ShouldCall_CorrectUrl()
        {
            var guid = Guid.NewGuid();
            var sessionId = "Abc123";
            var url = $"{ApiRoot}/assessment/notify/sms";
            
            var response = new NotifyResponse()
            {
                IsSuccess = true
            };
            
            _httpService.PostData(url, Arg.Any<object>(), guid).Returns(Task.FromResult(JsonConvert.SerializeObject(response)));

            var result = await _api.SendSms("disco", "0790002200", "0", sessionId, guid);
            
            Assert.Equal(response.IsSuccess, result.IsSuccess);
        }
        
    }
}