using Dfc.DiscoverSkillsAndCareers.WebApp.Controllers;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Dfc.UnitTests.ControllerTests
{
    public class QuestionControllerTests
    {
        private readonly IApiServices _apiServices;
        private readonly QuestionController _controller;

        public QuestionControllerTests()
        {
            _apiServices = Substitute.For<IApiServices>();
            var logger = Substitute.For<ILogger<QuestionController>>();
            var dataProtectionProvider = new EphemeralDataProtectionProvider();
            var layoutService = Substitute.For<ILayoutService>();

            _controller = new QuestionController(logger, _apiServices, dataProtectionProvider, layoutService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { }
                }
            };

            _controller.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>
            {
                {".dysac-session", dataProtectionProvider.CreateProtector("BaseController").Protect("Abc123")}
            });
        }

        [Fact]
        public async Task AnswerQuestion_ShouldReturn_BadRequestIfNoSessionId()
        {
            _controller.Request.Cookies = new RequestCookieCollection();

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
        public async Task AnswerQuestion_ShouldReturn_ViewIfNoOptionSelected()
        {
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

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal("Question", viewResult.ViewName);
        }

        [Fact]
        public async Task AnswerQuestion_ShouldReturn_500IfExceptionThrown()
        {
            _controller.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                { "questionId", "1" },
                { "selected_answer", new StringValues("3") }
            });

            _apiServices.PostAnswer("Abc123", Arg.Any<PostAnswerRequest>(), Arg.Any<Guid>())
                .Throws(new HttpRequestException());

            var result = await _controller.AnswerQuestion("short", "1");

            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }

        [Fact]
        public async Task AnswerQuestion_ShouldReturn_RedirectToFinishIfComplete()
        {
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

            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }

        [Fact]
        public async Task NewAssessment_ShouldReturn_FirstQuestionView()
        {
            _apiServices.NewSession(Arg.Any<Guid>(), "short").Returns(Task.FromResult(new NewSessionResponse
            {
                SessionId = "Abc123"
            }));

            _apiServices.Question("Abc123", "short", 1, Arg.Any<Guid>()).Returns(Task.FromResult(
                new AssessmentQuestionResponse
                {
                    SessionId = "Abc123",
                    QuestionNumber = 1,
                    NextQuestionNumber = 1
                }));

            var result = await _controller.NewAssessment("short");

            var scResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<QuestionViewModel>(scResult.Model);

            Assert.Equal("Question", scResult.ViewName);
            Assert.Equal(1, model.QuestionNumber);
        }

        [Fact]
        public async Task NextQuestion_ShouldReturn_FilteringViewResult()
        {
            _apiServices.Question("Abc123", "short", 1, Arg.Any<Guid>())
                .Returns(Task.FromResult(new AssessmentQuestionResponse
                {
                    IsFilterAssessment = true,
                    NextQuestionNumber = 1
                }));

            var result = await _controller.NextQuestion("Abc123", "short", 1, false);

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
                    IsFilterAssessment = false,
                    NextQuestionNumber = 1
                }));

            var result = await _controller.NextQuestion("Abc123", "short", 1, false);

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

            var result = await _controller.NextQuestion("Abc123", "short", 1, false);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }
    }
}