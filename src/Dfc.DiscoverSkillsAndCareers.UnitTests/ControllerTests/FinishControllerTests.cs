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
    public class FinishControllerTests
    {
        private readonly ILogger<FinishController> _logger;
        private readonly ILayoutService _layoutService;

        public FinishControllerTests()
        {
            _logger = Substitute.For<ILogger<FinishController>>();
            _layoutService = Substitute.For<ILayoutService>();
        }

        private FinishController CreateController(IDataProtectionProvider dataProtectionProvider = null, string cookie = null)
        {
            var _dataProtectionProvider = dataProtectionProvider ?? new EphemeralDataProtectionProvider();
            var controller = new FinishController(_logger, _dataProtectionProvider, _layoutService)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            if (!String.IsNullOrEmpty(cookie))
            {
                controller.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>
                {
                    {".dysac-session", _dataProtectionProvider.CreateProtector("BaseController").Protect(cookie)}
                });
            }

            return controller;
        }

        [Fact]
        public async Task Index_ShouldReturn_FinishView()
        {
            var controller = CreateController(cookie: "abc123");

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal("Finish", viewResult.ViewName);

            var viewModel = Assert.IsType<FinishViewModel>(viewResult.Model);

            Assert.False(viewModel.IsFilterAssessment);
        }

        [Fact]
        public async Task Index_ShouldReturn_HomeRedirectIfNoSessionId()
        {
            var controller = CreateController();
            var result = await controller.Index();

            var viewResult = Assert.IsType<RedirectResult>(result);

            Assert.Equal("/", viewResult.Url);
        }

        [Fact]
        public async Task Index_ShouldReturn_500WithException()
        {
            var _dataProtectionProvider = new EphemeralDataProtectionProvider();
            var controller = Substitute.ForPartsOf<FinishController>(new object[] { _logger, _dataProtectionProvider, _layoutService });
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>
            {
                {".dysac-session", _dataProtectionProvider.CreateProtector("BaseController").Protect("abc123")}
            });

            controller.View("Finish", Arg.Any<FinishViewModel>()).Throws(new Exception());

            var result = await controller.Index();

            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }

        [Fact]
        public async Task FinishWithJobCategory_ShouldReturn_FinishFiliteredView()
        {
            var controller = CreateController(cookie: "abc123");

            var result = await controller.FinishWithJobCategory("animal-care");

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal("FinishFilteredAssessment", viewResult.ViewName);

            var viewModel = Assert.IsType<FinishViewModel>(viewResult.Model);

            Assert.True(viewModel.IsFilterAssessment);
            Assert.Equal("animal-care", viewModel.JobCategorySafeUrl);
        }

        [Fact]
        public async Task FinishWithJobCategory_ShouldReturn_HomeRedirectIfNoSessionId()
        {
            var controller = CreateController();

            var result = await controller.FinishWithJobCategory("animal-care");

            var viewResult = Assert.IsType<RedirectResult>(result);

            Assert.Equal("/", viewResult.Url);
        }

        [Fact]
        public async Task FinishWithJobCategory_ShouldReturn_500WithException()
        {
            var _dataProtectionProvider = new EphemeralDataProtectionProvider();
            var controller = Substitute.ForPartsOf<FinishController>(new object[] { _logger, _dataProtectionProvider, _layoutService });
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>
            {
                {".dysac-session", _dataProtectionProvider.CreateProtector("BaseController").Protect("abc123")}
            });

            controller.View("FinishFilteredAssessment", Arg.Any<FinishViewModel>()).Throws(new Exception());

            var result = await controller.FinishWithJobCategory("animal-care");

            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }
    }
}