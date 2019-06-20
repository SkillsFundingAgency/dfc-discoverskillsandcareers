using System;
using System.Text;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.WebApp.Controllers;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Dfc.UnitTests.ControllerTests
{
    public class FinishControllerTests
    {
        private ILogger<FinishController> _logger;
        private IApiServices _apiServices;
        private ISession _session;
        private FinishController _controller;

        public FinishControllerTests()
        {
            _logger = Substitute.For<ILogger<FinishController>>();
            _apiServices = Substitute.For<IApiServices>();
            _session = Substitute.For<ISession>();
        
            _controller = new FinishController(_logger, _apiServices)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { Session = _session } }
            };
        }

        [Fact]
        public async Task Index_ShouldReturn_FinishView()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("Abc123");
                    return true;
                });
            
            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            
            Assert.Equal("Finish", viewResult.ViewName);

            var viewModel = Assert.IsType<FinishViewModel>(viewResult.Model);
            
            Assert.False(viewModel.IsFilterAssessment);
        }
        
        [Fact]
        public async Task Index_ShouldReturn_HomeRedirectIfNoSessionId()
        {
            var result = await _controller.Index();

            var viewResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/", viewResult.Url);
        }
        
        [Fact]
        public async Task Index_ShouldReturn_500WithException()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>()).Throws(new Exception());
            
            var result = await _controller.Index();

            var viewResult = Assert.IsType<StatusCodeResult>(result);
            
            Assert.Equal(500, viewResult.StatusCode);
        }
        
        [Fact]
        public async Task FinishWithJobCategory_ShouldReturn_FinishFiliteredView()
        {
           
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("Abc123");
                    return true;
                });
            
            var result = await _controller.FinishWithJobCategory("animal-care");

            var viewResult = Assert.IsType<ViewResult>(result);
            
            Assert.Equal("FinishFilteredAssessment", viewResult.ViewName);

            var viewModel = Assert.IsType<FinishViewModel>(viewResult.Model);
            
            Assert.True(viewModel.IsFilterAssessment);
            Assert.Equal("animal-care", viewModel.JobCategorySafeUrl);
        }

        [Fact]
        public async Task FinishWithJobCategory_ShouldReturn_HomeRedirectIfNoSessionId()
        {
            var result = await _controller.FinishWithJobCategory("animal-care");

            var viewResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/", viewResult.Url);
        }
        
        [Fact]
        public async Task FinishWithJobCategory_ShouldReturn_500WithException()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>()).Throws(new Exception());
            
            var result = await _controller.FinishWithJobCategory("animal-care");

            var viewResult = Assert.IsType<StatusCodeResult>(result);
            
            Assert.Equal(500, viewResult.StatusCode);
        }
        
    }
}