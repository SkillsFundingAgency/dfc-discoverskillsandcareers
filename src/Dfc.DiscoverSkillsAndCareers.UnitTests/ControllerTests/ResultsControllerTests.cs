using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Dfc.DiscoverSkillsAndCareers.WebApp.Controllers;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Dfc.UnitTests.ControllerTests
{
    public class ResultsControllerTests
    {
        private ILogger<ResultsController> _logger;
        private IApiServices _apiServices;
        private ResultsController _controller;
        private IOptions<AppSettings> _appSettings;
        private IDataProtectionProvider _dataProtectionProvider;

        public ResultsControllerTests()
        {
            _logger = Substitute.For<ILogger<ResultsController>>();
            _apiServices = Substitute.For<IApiServices>();
            _appSettings = Substitute.For<IOptions<AppSettings>>();
            _dataProtectionProvider = new EphemeralDataProtectionProvider();
            _appSettings.Value.Returns(new AppSettings());
            
            _controller = new ResultsController(_logger, _apiServices, _appSettings, _dataProtectionProvider)
            {
                ControllerContext = new ControllerContext
                {
                    
                    HttpContext = new DefaultHttpContext { }
                }
            };
            
            _controller.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>
            {
                {".dysac-session", _dataProtectionProvider.CreateProtector("BaseController").Protect("201904-Abc123")}
            });
        }

        [Fact]
        public async Task Index_ShouldReturn_RedirectToReloadOnErrorWithSessionId()
        {
            _appSettings.Value.Returns(new AppSettings());

            _apiServices.Results("201904-Abc123", Arg.Any<Guid>())
                .Throws(new HttpRequestException());
            
            var result = await _controller.Index();

            var viewResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/reload", viewResult.Url);
        }
        
        [Fact]
        public async Task Index_ShouldReturn_500OnException()
        {
            _appSettings.Value.Returns(new AppSettings());

            _apiServices.Results("201904-Abc123", Arg.Any<Guid>())
                .Throws(new Exception());
            
            var result = await _controller.Index();

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }
        
        [Fact]
        public async Task Index_ShouldReturn_RedirectToHomeOnErrorWithNoSessionId()
        {
            _controller.Request.Cookies = new RequestCookieCollection();
            _apiServices.Results("201904-Abc123", Arg.Any<Guid>())
                .Throws(new HttpRequestException());
            
            var result = await _controller.Index();

            var viewResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/", viewResult.Url);
        }
        
        [Fact]
        public async Task Index_ShouldReturn_RedirectIfNoSessionId()
        {
            _appSettings.Value.Returns(new AppSettings());
            _controller.Request.Cookies = new RequestCookieCollection();
            
            var result = await _controller.Index();

            var viewResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/", viewResult.Url);
        }

        [Fact]
        public async Task Index_ShouldReturn_ResultsViewModelIfNoFilterResultsInSession()
        {
            _appSettings.Value.UseFilteringQuestions = true;
            
            _apiServices.Results("201904-Abc123", Arg.Any<Guid>())
                .Returns(Task.FromResult(new ResultsResponse
                {
                    JobCategories = new JobCategoryResult[]{}
                }));

            
            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Results", viewResult.ViewName);

            var viewModel = Assert.IsType<ResultsViewModel>(viewResult.Model);
            
            Assert.Equal("201904-Abc123", viewModel.SessionId);
        }
        
        [Fact]
        public async Task Index_ShouldReturn_RedirectIfHasFilterResult()
        {
            _appSettings.Value.Returns(new AppSettings());

            _apiServices.Results("201904-Abc123", Arg.Any<Guid>())
                .Returns(Task.FromResult(new ResultsResponse
                {
                    
                    JobCategories = new []
                    {
                        new JobCategoryResult
                        {
                            FilterAssessmentResult = new FilterAssessmentResult { JobFamilyName = "Animal Care" }
                        }, 
                    }
                }));
            
            var result = await _controller.Index();

            var viewResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/results/animal-care", viewResult.Url);
        }

        [Fact]
        public async Task StartFilteredForJobCategory_ShouldReturn_NotFoundIfNotFilteringQuestions()
        {
            _appSettings.Value.Returns(new AppSettings {UseFilteringQuestions = false});

            var result = await _controller.StartFilteredForJobCategory("animal-care");

            Assert.IsType<NotFoundResult>(result);
        }
        
        [Fact]
        public async Task StartFilteredForJobCategory_ShouldReturn_RedirectToHomeIfNoSessionId()
        {
            _appSettings.Value.UseFilteringQuestions = true;
            _controller.Request.Cookies = new RequestCookieCollection();

            var result = await _controller.StartFilteredForJobCategory("animal-care");

            var viewResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/", viewResult.Url);
        }
        
        [Fact]
        public async Task StartFilteredForJobCategory_ShouldReturn_RedirectToFilteringQuestionStart()
        {
            _appSettings.Value.UseFilteringQuestions = true;

            _apiServices.StartFilteredForJobCategory(Arg.Any<Guid>(), "201904-Abc123", "animal-care")
                .Returns(Task.FromResult(new NewSessionResponse
                {
                    SessionId = "201904-Abc123",
                    QuestionNumber = 1
                }));
            var result = await _controller.StartFilteredForJobCategory("animal-care");

            var viewResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/q/animal-care/01", viewResult.Url);
        }
        
        [Fact]
        public async Task StartFilteredForJobCategory_ShouldReturn_500OnError()
        {
            _appSettings.Value.UseFilteringQuestions = true;
            _apiServices.StartFilteredForJobCategory(Arg.Any<Guid>(), Arg.Any<string>(), "animal-care")
                .Throws(new Exception());
            
            var result = await _controller.StartFilteredForJobCategory("animal-care");

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }
        
        [Fact]
        public async Task ResultsFilteredForJobCategory_ShouldReturn_NotFoundIfNotFilteringQuestions()
        {
            _appSettings.Value.UseFilteringQuestions = false;

            var result = await _controller.ResultsFilteredForJobCategory("animal-care");

            Assert.IsType<NotFoundResult>(result);
        }
        
        [Fact]
        public async Task ResultsFilteredForJobCategory_ShouldReturn_RedirectToHomeIfNoSessionId()
        {
            _appSettings.Value.UseFilteringQuestions = true;
            _controller.Request.Cookies = new RequestCookieCollection();

            var result = await _controller.ResultsFilteredForJobCategory("animal-care");

            var viewResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/", viewResult.Url);
        }
        
        [Fact]
        public async Task ResultsFilteredForJobCategory_ShouldReturn_500ResultOnApiFailure()
        {
            
            _appSettings.Value.UseFilteringQuestions = true;
            
            _apiServices.ResultsForJobCategory("201905-Abc123", "animal-care", Arg.Any<Guid>()).Throws(new HttpRequestException());

            var result = await _controller.ResultsFilteredForJobCategory("animal-care");

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }
        
        [Fact]
        public async Task ResultsFilteredForJobCategory_ShouldReturn_500OnError()
        {
            _appSettings.Value.UseFilteringQuestions = true;

            var _dataProtectionProvider = new EphemeralDataProtectionProvider();
            var controller = Substitute.ForPartsOf<ResultsController>(new object[] { _logger, _apiServices, _appSettings, _dataProtectionProvider});
            controller.ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext()};
            controller.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>
            {
                {".dysac-session", _dataProtectionProvider.CreateProtector("BaseController").Protect("abc123")}
            });

            controller.View("ResultsForJobCategory", Arg.Any<ResultsViewModel>()).Throws(new Exception());
            
            var result = await controller.ResultsFilteredForJobCategory("animal-care");

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }
        
        [Fact]
        public async Task ResultsFilteredForJobCategory_ShouldReturn_ResultsView()
        {
            _appSettings.Value.UseFilteringQuestions = true;
            
            _apiServices.ResultsForJobCategory("201904-Abc123","animal-care", Arg.Any<Guid>()).Returns(new ResultsResponse
            {
                SessionId = "201904-Abc123",
                JobCategories = new JobCategoryResult[]{}
            });
            
            var result = await _controller.ResultsFilteredForJobCategory("animal-care");

            var viewResult = Assert.IsType<ViewResult>(result);
            
            Assert.Equal("ResultsForJobCategory", viewResult.ViewName);

            var viewModel = Assert.IsType<ResultsViewModel>(viewResult.Model);
            
            Assert.Equal("201904-Abc123", viewModel.SessionId);
        }
    }
}