using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Dfc.DiscoverSkillsAndCareers.WebApp.Controllers;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.Http;
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
        private ISession _session;
        private ResultsController _controller;
        private IOptions<AppSettings> _appSettings;
        
        public ResultsControllerTests()
        {
            _logger = Substitute.For<ILogger<ResultsController>>();
            _apiServices = Substitute.For<IApiServices>();
            _session = Substitute.For<ISession>();
            _appSettings = Substitute.For<IOptions<AppSettings>>();
            
            _appSettings.Value.Returns(new AppSettings());
            
            _controller = new ResultsController(_logger, _apiServices, _appSettings)
            {
                ControllerContext = new ControllerContext
                {
                    
                    HttpContext = new DefaultHttpContext { Session = _session }
                }
            };
        }

        [Fact]
        public async Task Index_ShouldReturn_RedirectToReloadOnErrorWithSessionId()
        {
            _appSettings.Value.Returns(new AppSettings());
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });

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
            
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });

            _apiServices.Results("201904-Abc123", Arg.Any<Guid>())
                .Throws(new Exception());
            
            var result = await _controller.Index();

            var viewResult = Assert.IsType<StatusCodeResult>(result);
            
            Assert.Equal(500, viewResult.StatusCode);
        }
        
        [Fact]
        public async Task Index_ShouldReturn_RedirectToHomeOnErrorWithNoSessionId()
        {
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
            
            var result = await _controller.Index();

            var viewResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/", viewResult.Url);
        }

        [Fact]
        public async Task Index_ShouldReturn_ResultsViewModelIfNoFilterResultsInSession()
        {
            _appSettings.Value.UseFilteringQuestions = true;
            
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });

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
            
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201905-Abc123");
                    return true;
                });

            _apiServices.Results("201905-Abc123", Arg.Any<Guid>())
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

            var result = await _controller.StartFilteredForJobCategory("animal-care");

            var viewResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/", viewResult.Url);
        }
        
        [Fact]
        public async Task StartFilteredForJobCategory_ShouldReturn_RedirectToFilteringQuestionStart()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201905-Abc123");
                    return true;
                });
            
            _appSettings.Value.UseFilteringQuestions = true;

            var result = await _controller.StartFilteredForJobCategory("animal-care");

            var viewResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/q/animal-care/01", viewResult.Url);
        }
        
        [Fact]
        public async Task StartFilteredForJobCategory_ShouldReturn_500OnError()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Throws(new Exception());
            
            _appSettings.Value.UseFilteringQuestions = true;

            var result = await _controller.StartFilteredForJobCategory("animal-care");

            var viewResult = Assert.IsType<StatusCodeResult>(result);
            
            Assert.Equal(500, viewResult.StatusCode);
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

            var result = await _controller.ResultsFilteredForJobCategory("animal-care");

            var viewResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/", viewResult.Url);
        }
        
        [Fact]
        public async Task ResultsFilteredForJobCategory_ShouldReturn_RedirectToResultsOnApiFailure()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201905-Abc123");
                    return true;
                });
            
            _appSettings.Value.UseFilteringQuestions = true;

            _apiServices.Results("201905-Abc123", Arg.Any<Guid>()).Throws(new HttpRequestException());

            var result = await _controller.ResultsFilteredForJobCategory("animal-care");

            var viewResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/results", viewResult.Url);
        }
        
        [Fact]
        public async Task ResultsFilteredForJobCategory_ShouldReturn_500OnError()
        {
            _appSettings.Value.UseFilteringQuestions = true;
            
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Throws(new Exception());
            
            var result = await _controller.ResultsFilteredForJobCategory("animal-care");

            var viewResult = Assert.IsType<StatusCodeResult>(result);
            
            Assert.Equal(500, viewResult.StatusCode);
        }
        
        [Fact]
        public async Task ResultsFilteredForJobCategory_ShouldReturn_ResultsView()
        {
            _appSettings.Value.UseFilteringQuestions = true;
            
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201905-Abc123");
                    return true;
                });
            
            _apiServices.Results("201905-Abc123", Arg.Any<Guid>()).Returns(new ResultsResponse
            {
                SessionId = "201905-Abc123",
                JobCategories = new JobCategoryResult[]{}
            });
            
            var result = await _controller.ResultsFilteredForJobCategory("animal-care");

            var viewResult = Assert.IsType<ViewResult>(result);
            
            Assert.Equal("ResultsForJobCategory", viewResult.ViewName);

            var viewModel = Assert.IsType<ResultsViewModel>(viewResult.Model);
            
            Assert.Equal("201905-Abc123", viewModel.SessionId);
        }
    }
}