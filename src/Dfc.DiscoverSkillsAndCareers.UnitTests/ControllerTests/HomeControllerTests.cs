using Dfc.DiscoverSkillsAndCareers.WebApp.Controllers;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Dfc.UnitTests.ControllerTests
{
    public class HomeControllerTests
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILayoutService _layoutService;
        private readonly IApiServices _apiServices;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _logger = Substitute.For<ILogger<HomeController>>();
            _apiServices = Substitute.For<IApiServices>();
            var session = Substitute.For<ISession>();
            var dataProtectionProvider = new EphemeralDataProtectionProvider();
            _layoutService = Substitute.For<ILayoutService>();

            _controller = new HomeController(_logger, _apiServices, dataProtectionProvider, _layoutService)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { Session = session } }
            };
        }

        [Fact]
        public async Task IndexMethod_ShouldReturn_IndexViewAndViewModel()
        {
            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal("Index", viewResult.ViewName);
            Assert.IsType<IndexViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task IndexMethod_ShouldReturn_SessionIdInViewModel()
        {
            var _dataProtectionProvider = new EphemeralDataProtectionProvider();
            var controller = Substitute.ForPartsOf<HomeController>(new object[] { _logger, _apiServices, _dataProtectionProvider, _layoutService });
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>
            {
                {".dysac-session", _dataProtectionProvider.CreateProtector("BaseController").Protect("abc123")}
            });

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<IndexViewModel>(viewResult.Model);

            Assert.Equal("abc123", viewModel.SessionId);
        }

        [Fact]
        public async Task IndexMethod_ShouldReturn_MissingReferenceErrorMessage()
        {
            var result = await _controller.Index("1");

            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<IndexViewModel>(viewResult.Model);

            Assert.True(viewModel.HasError);
            Assert.Equal("Enter your reference", viewModel.ErrorMessage);
        }

        [Fact]
        public async Task IndexMethod_ShouldReturn_InvalidReferenceErrorMessage()
        {
            var result = await _controller.Index("2");

            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<IndexViewModel>(viewResult.Model);

            Assert.True(viewModel.HasError);
            Assert.Equal("The reference could not be found", viewModel.ErrorMessage);
        }

        [Fact]
        public async Task IndexMethod_ShouldReturn_500WhenErrorThrown()
        {
            var _dataProtectionProvider = new EphemeralDataProtectionProvider();
            var controller = Substitute.ForPartsOf<HomeController>(new object[] { _logger, _apiServices, _dataProtectionProvider, _layoutService });
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>
            {
                {".dysac-session", _dataProtectionProvider.CreateProtector("BaseController").Protect("abc123")}
            });

            controller.View("Index", Arg.Any<IndexViewModel>()).Throws(new Exception());

            var result = await controller.Index();

            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }

        [Fact]
        public async Task ReloadGet_ShouldReturn_ShouldRedirectToHomeIfNoSessionId()
        {
            var result = await _controller.ReloadGet();

            var redirect = Assert.IsType<RedirectResult>(result);

            Assert.Equal("/", redirect.Url);
        }

        [Fact]
        public async Task Reload_ShouldReturn_RedirectToMissingSessionId()
        {
            var result = await _controller.Reload(new HomeController.ReloadRequest { Code = null });

            var redirect = Assert.IsType<RedirectResult>(result);

            Assert.Equal("/?e=1", redirect.Url);
        }

        [Fact]
        public async Task Reload_ShouldReturn_RedirectToInvalidSessionIdIfCodeNotValid()
        {
            var result = await _controller.Reload(new HomeController.ReloadRequest { Code = "My code" });

            var redirect = Assert.IsType<RedirectResult>(result);

            Assert.Equal("/?e=2", redirect.Url);
        }

        [Fact]
        public async Task Reload_ShouldReturn_RedirectToInvalidSessionIdIfApiResponseIsNull()
        {
            _apiServices.Reload("abc123", Arg.Any<Guid>()).Returns(Task.FromResult<AssessmentQuestionResponse>(null));

            var result = await _controller.Reload(new HomeController.ReloadRequest { Code = "Abc123" });

            var redirect = Assert.IsType<RedirectResult>(result);

            Assert.Equal("/?e=2", redirect.Url);
        }

        [Fact]
        public async Task Reload_ShouldReturn_RedirectToResultsIfIsCompleteTrue()
        {
            _apiServices.Reload("abc123", Arg.Any<Guid>()).Returns(Task.FromResult(new AssessmentQuestionResponse
            {
                IsComplete = true,
                SessionId = "abc123"
            }));

            var result = await _controller.Reload(new HomeController.ReloadRequest { Code = "Abc123" });

            var redirect = Assert.IsType<RedirectResult>(result);

            Assert.Equal("/results", redirect.Url);
        }

        [Fact]
        public async Task Reload_ShouldReturn_RedirectToFilterAssessmentIsCompleteAndIsFilterAssessmentTrue()
        {
            _apiServices.Reload("abc123", Arg.Any<Guid>()).Returns(Task.FromResult(new AssessmentQuestionResponse
            {
                IsComplete = true,
                IsFilterAssessment = true,
                JobCategorySafeUrl = "animal-care",
                SessionId = "abc123"
            }));

            var result = await _controller.Reload(new HomeController.ReloadRequest { Code = "abc123" });

            var redirect = Assert.IsType<RedirectResult>(result);

            Assert.Equal("/results/animal-care", redirect.Url);
        }

        [Fact]
        public async Task Reload_ShouldReturn_RedirectToShortAssessmentQuestionUrl()
        {
            _apiServices.Reload("abc123", Arg.Any<Guid>()).Returns(Task.FromResult(new AssessmentQuestionResponse
            {
                IsComplete = false,
                IsFilterAssessment = false,
                NextQuestionNumber = 9,
                MaxQuestionsCount = 40,
                SessionId = "abc123"
            }));

            var result = await _controller.Reload(new HomeController.ReloadRequest { Code = "abc123" });

            var redirect = Assert.IsType<RedirectResult>(result);

            Assert.Equal("/q/short/09", redirect.Url);
        }

        [Fact]
        public async Task Reload_ShouldReturn_RedirectToFilterAssessmentQuestionUrl()
        {
            _apiServices.Reload("abc123", Arg.Any<Guid>()).Returns(Task.FromResult(new AssessmentQuestionResponse
            {
                IsComplete = false,
                IsFilterAssessment = true,
                JobCategorySafeUrl = "animal-care",
                NextQuestionNumber = 2,
                MaxQuestionsCount = 3,
                SessionId = "abc123"
            }));

            var result = await _controller.Reload(new HomeController.ReloadRequest { Code = "abc123" });

            var redirect = Assert.IsType<RedirectResult>(result);

            Assert.Equal("/q/animal-care/02", redirect.Url);
        }

        [Fact]
        public async Task Reload_ShouldReturn_500IfSessionCodeIsNullOrMissing()
        {
            _apiServices.Reload("abc123", Arg.Any<Guid>()).Returns(Task.FromResult(new AssessmentQuestionResponse
            {
                SessionId = null
            }));

            var result = await _controller.Reload(new HomeController.ReloadRequest { Code = "abc123" });

            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }
    }
}