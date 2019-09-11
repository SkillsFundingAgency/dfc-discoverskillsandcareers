using Dfc.DiscoverSkillsAndCareers.WebApp.Controllers;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Dfc.UnitTests.ControllerTests
{
    public class ErrorControllerTests
    {
        private readonly ErrorController _controller;

        public ErrorControllerTests()
        {
            var logger = Substitute.For<ILogger<ErrorController>>();
            var dataProtectionProvider = Substitute.For<IDataProtectionProvider>();
            var layoutService = Substitute.For<ILayoutService>();

            _controller = new ErrorController(logger, dataProtectionProvider, layoutService)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
        }

        [Fact]
        public async Task Error404_ShouldReturn_404View()
        {
            var result = await _controller.Error404();

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal("404", viewResult.ViewName);
        }

        [Fact]
        public async Task Error500_ShouldReturn_500View()
        {
            var result = await _controller.Error500();

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal("500", viewResult.ViewName);
        }
    }
}