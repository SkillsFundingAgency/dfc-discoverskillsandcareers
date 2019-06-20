using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using DFC.HTTP.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.AssessmentApi;
using Dfc.DiscoverSkillsAndCareers.Models;
using Xunit;

namespace Dfc.UnitTests.FunctionTests
{
    public class PostAnswerHttpTriggerTests : IDisposable
    {
        public PostAnswerHttpTriggerTests()
        {
            _request = new DefaultHttpRequest(new DefaultHttpContext());
            _log = Substitute.For<ILogger>();
            _httpRequestHelper = Substitute.For<IHttpRequestHelper>();
            _httpResponseMessageHelper = Substitute.For<IHttpResponseMessageHelper>();
            _userSessionRepository = Substitute.For<IUserSessionRepository>();
            _questionRepository = Substitute.For<IQuestionRepository>();
            _resultsService = Substitute.For<IAssessmentCalculationService>();
            _filterAssessmentCalculationService = Substitute.For<IFilterAssessmentCalculationService>();
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
        private IHttpRequestHelper _httpRequestHelper;
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private IUserSessionRepository _userSessionRepository;
        private IQuestionRepository _questionRepository;
        private IAssessmentCalculationService _resultsService;
        private IFilterAssessmentCalculationService _filterAssessmentCalculationService;

        private async Task<HttpResponseMessage> RunFunction(string sessionId)
        {
            return await PostAnswerHttpTrigger.Run(
                _request,
                sessionId,
                _log,
                _httpRequestHelper,
                _httpResponseMessageHelper,
                _userSessionRepository,
                _questionRepository,
                _resultsService,
                _filterAssessmentCalculationService
            ).ConfigureAwait(false);
        }

        [Fact]
        public async Task PostAnswerHttpTrigger_WithInvalidAnswerValue_ShouldReturnBadRequest()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            var postAnswerRequest = new PostAnswerRequest()
            {
                QuestionId = "1",
                SelectedOption = "invalid-answer"
            };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(postAnswerRequest);
            _request.Body = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));


            var result = await RunFunction("session1");
            
            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task PostAnswerHttpTrigger_WithInvalidSessionId_ShouldReturnNoContent()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            var postAnswerRequest = new PostAnswerRequest()
            {
                QuestionId = "1",
                SelectedOption = "Agree"
            };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(postAnswerRequest);
            _request.Body = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));

            var result = await RunFunction("invalid-session-id");

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Fact]
        public async Task PostAnswerHttpTrigger_WithMissingSessionId_ShouldReturnBadRequest()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();

            var result = await RunFunction(null);

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task PostAnswerHttpTrigger_WithValidAnswerValue_ShouldHaveIsSuccessInModel()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            var postAnswerRequest = new PostAnswerRequest()
            {
                QuestionId = "1",
                SelectedOption = "Agree"
            };

            _questionRepository.GetQuestion("1").Returns(Task.FromResult(new Question {QuestionId = "1"}));
            
            _userSessionRepository.GetUserSession("session1").Returns(Task.FromResult(new UserSession
            {
                AssessmentState = new AssessmentState("QS-1", 5)
            }));
            
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(postAnswerRequest);
            _request.Body = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));

            var result = await RunFunction("session1");
            var content = await result.Content.ReadAsAsync<PostAnswerResponse>();

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.True(content.IsSuccess);
        }
        
        [Fact]
        public async Task PostAnswerHttpTrigger_WithValidAnswerValue_ShouldReturnNextQuestionEvenIfComplete()
        {
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            var postAnswerRequest = new PostAnswerRequest()
            {
                QuestionId = "short-1-1",
                SelectedOption = "Agree"
            };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(postAnswerRequest);
            _request.Body = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            
            _questionRepository.GetQuestion("short-1-1").Returns(Task.FromResult(new Question
            {
                QuestionId = "short-1-1",
                IsFilterQuestion = false,
                Order = 1,
                Texts = new []
                {
                    new QuestionText { LanguageCode = "en", Text = "q1" }, 
                }
            }));

            var session = new UserSession
            {
                AssessmentType = "short",
                AssessmentState = new AssessmentState("QS-1", 2)
                {
                    RecordedAnswers = new[]
                    {
                        new Answer
                        {
                            QuestionId = "1", QuestionNumber = 1, QuestionSetVersion = "QS-1",
                            SelectedOption = AnswerOption.Agree
                        },
                    }
                }
            };
            
            
            _userSessionRepository.GetUserSession("session1").Returns(Task.FromResult(session));
            
            var result = await RunFunction("session1");
            var content = await result.Content.ReadAsAsync<PostAnswerResponse>();

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.True(content.IsSuccess);
            Assert.Equal(2,content.NextQuestionNumber);
        }
    }
}


//                    FilteredAssessmentState = new FilteredAssessmentState
//                    {
//                        CurrentFilterAssessmentCode = "filtered-social-care-1",
//                        CompleteDt = new DateTime(2019,1,1),
//                        JobCategoryStates = new List<JobCategoryState> {
//                            new JobCategoryState
//                            {
//                                JobCategoryCode = "filtered-social-care-1",
//                                CurrentQuestion = 1,
//                                JobCategoryName = "socail-care",
//                                QuestionSetVersion = "filtered-social-care-1",
//                                Skills = new []
//                                {
//                                    new JobCategorySkill { Skill = "Initative" }, 
//                                    new JobCategorySkill { Skill = "Self control" } 
//                                }
//                            }
//                        },
//                        RecordedAnswers = new []
//                        {
//                            new Answer { 
//                                AnsweredDt = new DateTime(2019,1,1), 
//                                QuestionSetVersion = "filtered-social-care-1",
//                                IsNegative = false,
//                                QuestionId = "filtered-social-care-1-1",
//                                QuestionNumber = 1,
//                                QuestionText = "Question 1",
//                                SelectedOption = AnswerOption.Yes
//                            },
//                            new Answer { 
//                                AnsweredDt = new DateTime(2019,1,1), 
//                                QuestionSetVersion = "filtered-social-care-1",
//                                IsNegative = false,
//                                QuestionId = "filtered-social-care-1-2",
//                                QuestionNumber = 2,
//                                QuestionText = "Question 2",
//                                SelectedOption = AnswerOption.No
//                            }
//                        }
//                    }
