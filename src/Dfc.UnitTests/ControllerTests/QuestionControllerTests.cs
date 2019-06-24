using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Dfc.DiscoverSkillsAndCareers.WebApp.Controllers;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Dfc.UnitTests.ControllerTests
{
    
    public class QuestionControllerTests
    {
        private ILogger<QuestionController> _logger;
        private IApiServices _apiServices;
        private ISession _session;
        private QuestionController _controller;
        private IOptions<AppSettings> _appSettings;
        
        public QuestionControllerTests()
        {
            _logger = Substitute.For<ILogger<QuestionController>>();
            _apiServices = Substitute.For<IApiServices>();
            _session = Substitute.For<ISession>();
            _appSettings = Substitute.For<IOptions<AppSettings>>();
            
            
            _controller = new QuestionController(_logger, _apiServices, _appSettings)
            {
                ControllerContext = new ControllerContext
                {
                    
                    HttpContext = new DefaultHttpContext { Session = _session }
                }
            };
        }

        [Fact]
        public async Task AnswerQuestion_ShouldReturn_BadRequestIfNoSessionId()
        {
            var result = await _controller.AnswerQuestion("short", "1");

            Assert.IsType<BadRequestResult>(result);
        }
        
        [Fact]
        public async Task AnswerQuestion_ShouldReturn_BadRequestIfQuestionNotAnInteger()
        {
            var result = await _controller.AnswerQuestion("short", "dave");

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task AnswerQuestion_ShouldReturn_NoAnswerErrorMessageIfSelectedOptionNull()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("Abc123");
                    return true;
                });
            
            _controller.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                { "questionId", "1" },
                { "selected_answer", new StringValues() }
            });

            _apiServices.PostAnswer("Abc123", Arg.Any<PostAnswerRequest>(), Arg.Any<Guid>())
                .Returns(Task.FromResult(new PostAnswerResponse { NextQuestionNumber = 2, IsSuccess = true }));

            _apiServices.Question("Abc123", "short", 1, Arg.Any<Guid>())
                .Returns(Task.FromResult(new AssessmentQuestionResponse()));
            
            var result = await _controller.AnswerQuestion("short", "1");

            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<QuestionViewModel>(viewResult.Model);

            Assert.Equal(viewModel.NoAnswerErrorMessage, viewModel.ErrorMessage);

        }
        
        [Fact]
        public async Task AnswerQuestion_ShouldReturn_NoAnswerErrorMessageIfResponseNull()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("Abc123");
                    return true;
                });
            
            _controller.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                { "questionId", "1" },
                { "selected_answer", new StringValues() }
            });

            _apiServices.PostAnswer("Abc123", Arg.Any<PostAnswerRequest>(), Arg.Any<Guid>())
                .Returns(Task.FromResult<PostAnswerResponse>(null));

            _apiServices.Question("Abc123", "short", 1, Arg.Any<Guid>())
                .Returns(Task.FromResult(new AssessmentQuestionResponse()));
            
            var result = await _controller.AnswerQuestion("short", "1");

            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<QuestionViewModel>(viewResult.Model);

            Assert.Equal(viewModel.NoAnswerErrorMessage, viewModel.ErrorMessage);

        }
        
        [Fact]
        public async Task AnswerQuestion_ShouldReturn_500IfHttpException()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("Abc123");
                    return true;
                });
            
            _controller.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                { "questionId", "1" },
                { "selected_answer", new StringValues() }
            });

            _apiServices.PostAnswer("Abc123", Arg.Any<PostAnswerRequest>(), Arg.Any<Guid>())
                .Throws(new HttpRequestException());
            
            _apiServices.Question("Abc123", "short", 1, Arg.Any<Guid>())
                .Returns(Task.FromResult(new AssessmentQuestionResponse()));
            
            var result = await _controller.AnswerQuestion("short", "1");

            var viewResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal(500, viewResult.StatusCode);

        }
        
        [Fact]
        public async Task AnswerQuestion_ShouldReturn_500IfExceptionThrown()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("Abc123");
                    return true;
                });
            
            _controller.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                { "questionId", "1" },
                { "selected_answer", new StringValues("3") }
            });

            _apiServices.PostAnswer("Abc123", Arg.Any<PostAnswerRequest>(), Arg.Any<Guid>())
                .Throws(new HttpRequestException());
            
            var result = await _controller.AnswerQuestion("short", "1");

            var viewResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal(500, viewResult.StatusCode);
        }
        
        [Fact]
        public async Task AnswerQuestion_ShouldReturn_RedirectToFinishIfComplete()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("Abc123");
                    return true;
                });
            
            _controller.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                { "questionId", "1" },
                { "selected_answer", new StringValues("3") }
            });

            _apiServices.PostAnswer("Abc123", Arg.Any<PostAnswerRequest>(), Arg.Any<Guid>())
                .Returns(Task.FromResult(new PostAnswerResponse { NextQuestionNumber = 2, IsSuccess = true, IsComplete = true }));
            
            var result = await _controller.AnswerQuestion("short", "1");

            var viewResult = Assert.IsType<RedirectResult>(result);

            Assert.Equal("/finish", viewResult.Url);

        }
        
        [Fact]
        public async Task AnswerQuestion_ShouldReturn_RedirectToFilterAssessmentFinishIfComplete()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("Abc123");
                    return true;
                });
            
            _controller.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                { "questionId", "1" },
                { "selected_answer", new StringValues("3") }
            });

            _apiServices.PostAnswer("Abc123", Arg.Any<PostAnswerRequest>(), Arg.Any<Guid>())
                .Returns(Task.FromResult(new PostAnswerResponse
                {
                    NextQuestionNumber = 2, 
                    IsSuccess = true, 
                    IsComplete = true, 
                    IsFilterAssessment = true,
                    JobCategorySafeUrl = "animal-care"
                }));
            
            var result = await _controller.AnswerQuestion("short", "1");

            var viewResult = Assert.IsType<RedirectResult>(result);

            Assert.Equal("/finish/animal-care", viewResult.Url);
        }
        
        [Fact]
        public async Task AnswerQuestion_ShouldReturn_RedirectToNextQuestionIfNotComplete()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("Abc123");
                    return true;
                });
            
            _controller.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                { "questionId", "1" },
                { "selected_answer", new StringValues("3") }
            });

            _apiServices.PostAnswer("Abc123", Arg.Is<PostAnswerRequest>(r => r.QuestionId == "1" && r.SelectedOption == "3"), Arg.Any<Guid>())
                .Returns(Task.FromResult(new PostAnswerResponse
                {
                    NextQuestionNumber = 2, 
                    IsSuccess = true, 
                    IsComplete = false, 
                    IsFilterAssessment = false
                }));
            
            var result = await _controller.AnswerQuestion("short", "1");

            var viewResult = Assert.IsType<RedirectResult>(result);

            Assert.Equal("/q/short/02", viewResult.Url);
        }


        [Fact]
        public async Task NewAssessment_ShouldReturn_500IfApiCallNewAssessmentFails()
        {
            _apiServices.NewSession(Arg.Any<Guid>(), "short").Returns(Task.FromResult<NewSessionResponse>(null));

            var result = await _controller.NewAssessment("short");

            var scResult = Assert.IsType<StatusCodeResult>(result);
            
            Assert.Equal(500, scResult.StatusCode);
            
        }
        
        [Fact]
        public async Task NewAssessment_ShouldReturn_RedirectResultToQuestionUrl()
        {
            _apiServices.NewSession(Arg.Any<Guid>(), "short").Returns(Task.FromResult(new NewSessionResponse
            {
                SessionId = "Abc123"
            }));

            var result = await _controller.NewAssessment("short");

            var scResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/q/short/01", scResult.Url);
        }

        [Fact]
        public async Task NextQuestion_ShouldReturn_FilteringViewResult()
        {
            _apiServices.Question("Abc123", "short", 1, Arg.Any<Guid>())
                .Returns(Task.FromResult(new AssessmentQuestionResponse
                {
                    IsFilterAssessment = true
                }));
            
            var result = await _controller.NextQuestion("Abc123","short", 1, false);

            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<QuestionViewModel>(viewResult.Model);
            
            Assert.True(viewModel.IsFilterAssessment);
            Assert.Equal("FilteringQuestion", viewResult.ViewName);
        }
        
        [Fact]
        public async Task NextQuestion_ShouldReturn_QuestionViewResult()
        {
            _apiServices.Question("Abc123", "short", 1, Arg.Any<Guid>())
                .Returns(Task.FromResult(new AssessmentQuestionResponse
                {
                    IsFilterAssessment = false
                }));
            
            var result = await _controller.NextQuestion("Abc123","short", 1, false);

            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<QuestionViewModel>(viewResult.Model);
            
            Assert.False(viewModel.IsFilterAssessment);
            Assert.Equal("Question", viewResult.ViewName);
        }
        
        [Fact]
        public async Task NextQuestion_ShouldReturn_500IfApiResponseIsNull()
        {
            _apiServices.Question("Abc123", "short", 1, Arg.Any<Guid>())
                .Returns(Task.FromResult<AssessmentQuestionResponse>(null));
            
            var result = await _controller.NextQuestion("Abc123","short", 1, false);

            var viewResult = Assert.IsType<StatusCodeResult>(result);
  
            Assert.Equal(500, viewResult.StatusCode);
        }
    }
}