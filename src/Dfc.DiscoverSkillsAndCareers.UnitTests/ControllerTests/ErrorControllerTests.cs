using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.WebApp.Controllers;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Dfc.UnitTests.ControllerTests
{
    public class ErrorControllerTests
    {
        private ILogger<ErrorController> _logger;
        private IApiServices _apiServices;
        private ErrorController _controller;
        private IDataProtectionProvider _dataProtectionProvider;

        public ErrorControllerTests()
        {
            _logger = Substitute.For<ILogger<ErrorController>>();
            _apiServices = Substitute.For<IApiServices>();
            _dataProtectionProvider = Substitute.For<IDataProtectionProvider>();
            
            _controller = new ErrorController(_logger, _apiServices, _dataProtectionProvider)
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