using System;
using System.Text;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Dfc.DiscoverSkillsAndCareers.WebApp.Controllers;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.ApplicationInsights.WindowsServer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Dfc.UnitTests.ControllerTests
{
    public class SaveProgressControllerTests
    {
        private ILogger<SaveProgressController> _logger;
        private IApiServices _apiServices;
        private ISession _session;
        private SaveProgressController _controller;
        private IOptions<AppSettings> _appSettings;
        private ITempDataDictionary _tempData;
        private IDataProtectionProvider _dataProtectionProvider;

        public SaveProgressControllerTests()
        {
            _logger = Substitute.For<ILogger<SaveProgressController>>();
            _apiServices = Substitute.For<IApiServices>();
            _session = Substitute.For<ISession>();
            _appSettings = Substitute.For<IOptions<AppSettings>>();
            
            _dataProtectionProvider = Substitute.For<IDataProtectionProvider>();
            _tempData = Substitute.For<ITempDataDictionary>();
            
            _appSettings.Value.Returns(new AppSettings());
            
            _controller = new SaveProgressController(_logger, _apiServices, _appSettings, _dataProtectionProvider)
            {
                TempData = _tempData,
                ControllerContext = new ControllerContext
                {
                    
                    HttpContext = new DefaultHttpContext { Session = _session }
                }
            };
        }

        [Theory]
        [InlineData("email", "EmailInput")]
        [InlineData("sms", "SmsInput")]
        [InlineData("reference", "ReferenceNumber")]
        public async Task SaveProgressOption_ShouldReturn_Redirect(string option, string url)
        {
            var result = await _controller.SaveProgressOption(new SaveProgressOptionRequest {SelectedOption = option});

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal(url, redirectResult.ActionName);
        }

        [Fact]
        public async Task SaveProgressOption_ShouldReturn_ViewResultIfNotValidOptionWithErrorMessage()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });
            
            var result = await _controller.SaveProgressOption(new SaveProgressOptionRequest {SelectedOption = "foo"});

            var viewResult = Assert.IsType<ViewResult>(result);
            
            Assert.Equal("SaveProgress", viewResult.ViewName);

            var viewModel = Assert.IsType<SaveProgressViewModel>(viewResult.Model);
            
            Assert.True(viewModel.HasError);
        }
        
        [Fact]
        public async Task SaveProgressOption_ShouldReturn_RedirectToIndexIfOptionInvalidAndNoSession()
        {
            var result = await _controller.SaveProgressOption(new SaveProgressOptionRequest {SelectedOption = "foo"});

            var viewResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/", viewResult.Url);
        }

        [Fact]
        public async Task Index_ShouldReturn_RedirectToHomeIfNoSession()
        {
            var result = await _controller.Index();

            var viewResult = Assert.IsType<RedirectResult>(result);

            Assert.Equal("/", viewResult.Url);
        }
        
        
        [Fact]
        public async Task Index_ShouldReturn_ViewResult()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });
            
            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal("SaveProgress", viewResult.ViewName);
            
            var viewModel = Assert.IsType<SaveProgressViewModel>(viewResult.Model);
            
            Assert.False(viewModel.HasError);
            Assert.Null(viewModel.ErrorMessage);
        }
        
        
        [Fact]
        public async Task Index_ShouldReturn_ViewResultWithErrorMessage()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });
            
            var result = await _controller.Index(true);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal("SaveProgress", viewResult.ViewName);
            
            var viewModel = Assert.IsType<SaveProgressViewModel>(viewResult.Model);
            
            Assert.True(viewModel.HasError);
            Assert.Equal("Choose how you would like to return to your assessment", viewModel.ErrorMessage);
        }

        [Fact]
        public async Task Index_ShouldReturn_500OnError()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });

            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Throws(new Exception());
            
            var result = await _controller.Index();

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }

        [Fact]
        public void EmailSent_ShouldReturn_ViewResultIfSentEmailNotEmpty()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });

            _tempData["SentEmail"].Returns("my@email.com");
            
            var result = _controller.EmailSent();

            var viewResult = Assert.IsType<ViewResult>(result);
            
            Assert.Equal("EmailSent", viewResult.ViewName);

            var viewModel = Assert.IsType<SaveProgressViewModel>(viewResult.Model);
            
            Assert.Equal("my@email.com",viewModel.SentTo);
        }
        
        [Fact]
        public void EmailSent_ShouldReturn_RedirectIfSentEmailEmpty()
        {
            var result = _controller.EmailSent();

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("Index", viewResult.ActionName);
        }
        
        [Fact]
        public void SmsSent_ShouldReturn_ViewResultIfSentEmailNotEmpty()
        {
            _tempData["SentSms"].Returns("07900003000");
            
            var result = _controller.SmsSent();

            var viewResult = Assert.IsType<ViewResult>(result);
            
            Assert.Equal("SmsSent", viewResult.ViewName);

            var viewModel = Assert.IsType<SaveProgressViewModel>(viewResult.Model);
            
            Assert.Equal("07900003000",viewModel.SentTo);
        }
        
        [Fact]
        public void SmsSent_ShouldReturn_RedirectIfSentEmailEmpty()
        {
            var result = _controller.EmailSent();

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("Index", viewResult.ActionName);
        }

        [Theory]
        [InlineData("1", "Enter an email address")]
        [InlineData("2", "Enter an email address in the correct format, like name@example.com")]
        public async Task EmailInput_ShouldReturn_CorrectErrorMessageForInput(string error, string expectedMessage)
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });

            var result = await _controller.EmailInput(error);
            var viewResult = Assert.IsType<ViewResult>(result);
            
            Assert.Equal("EmailInput", viewResult.ViewName);

            var viewModel = Assert.IsType<SaveProgressViewModel>(viewResult.Model);
            
            Assert.True(viewModel.HasError);
            Assert.Equal(expectedMessage, viewModel.ErrorMessage);
        }
        
        [Fact]
        public async Task EmailInput_ShouldReturn_500WhenFailsToSendEmail()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });

            var result = await _controller.EmailInput("3");
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }
        
        [Fact]
        public async Task EmailInput_ShouldReturn_500OnError()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Throws(new Exception());

            var result = await _controller.EmailInput();
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }
        
        [Theory]
        [InlineData("1", "Enter a phone number")]
        [InlineData("2", "Enter a mobile phone number, like 07700 900 982.")]
        public async Task SmsInput_ShouldReturn_CorrectErrorMessageForInput(string error, string expectedMessage)
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });

            var result = await _controller.SmsInput(error);
            var viewResult = Assert.IsType<ViewResult>(result);
            
            Assert.Equal("SmsInput", viewResult.ViewName);

            var viewModel = Assert.IsType<SaveProgressViewModel>(viewResult.Model);
            
            Assert.True(viewModel.HasError);
            Assert.Equal(expectedMessage, viewModel.ErrorMessage);
        }
        
        [Fact]
        public async Task SmsInput_ShouldReturn_500WhenFailsToSendEmail()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });

            var result = await _controller.SmsInput("3");
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }
        
        [Fact]
        public async Task SmsInput_ShouldReturn_500OnError()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Throws(new Exception());

            var result = await _controller.SmsInput();
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }

        [Fact]
        public async Task SendEmail_ShouldReturn_RedirectOnMissingSession()
        {
            var result = await _controller.SendEmail(new SendEmailRequest {Email = "my@email.com"});
            var redirectResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/", redirectResult.Url);
        }
        
        [Fact]
        public async Task SendEmail_ShouldReturn_500OnError()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>()).Throws(new Exception());
            
            var result = await _controller.SendEmail(new SendEmailRequest {Email = "my@email.com"});
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }
        
        [Fact]
        public async Task SendEmail_ShouldReturn_RedirectOnInvalidEmail()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });
            
            var result = await _controller.SendEmail(new SendEmailRequest {Email = "email"});
            var redirectResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/save-my-progress/email?e=2", redirectResult.Url);
        }
        
        [Fact]
        public async Task SendEmail_ShouldReturn_RedirectOnMissingEmail()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });
            
            var result = await _controller.SendEmail(new SendEmailRequest {Email = ""});
            var redirectResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/save-my-progress/email?e=1", redirectResult.Url);
        }
        
        [Fact]
        public async Task SendEmail_ShouldReturn_RedirectToEmailSentOnSuccess()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });

            _apiServices.SendEmail(
                Arg.Any<string>(), 
                "my@email.com", 
                Arg.Any<string>(), 
                "201904-Abc123",
                Arg.Any<Guid>()).Returns(Task.FromResult(new NotifyResponse { IsSuccess = true }));
            
            var result = await _controller.SendEmail(new SendEmailRequest {Email = "my@email.com"});
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("EmailSent", redirectResult.ActionName);
        }
        
        [Fact]
        public async Task SendEmail_ShouldReturn_RedirectToOnNotifyFailure()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });

            _apiServices.SendEmail(
                Arg.Any<string>(), 
                "my@email.com", 
                Arg.Any<string>(), 
                "201904-Abc123",
                Arg.Any<Guid>()).Returns(Task.FromResult(new NotifyResponse { IsSuccess = false }));
            
            var result = await _controller.SendEmail(new SendEmailRequest {Email = "my@email.com"});
            var redirectResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/save-my-progress/email?e=3", redirectResult.Url);
        }
        
        [Fact]
        public async Task SendSms_ShouldReturn_RedirectOnMissingSession()
        {
            var result = await _controller.SendSms(new SendSmsRequest() {MobileNumber = "0790009200"});
            var redirectResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/", redirectResult.Url);
        }
        
        [Fact]
        public async Task SendSms_ShouldReturn_500OnError()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>()).Throws(new Exception());
            
            var result = await _controller.SendSms(new SendSmsRequest() {MobileNumber = "0790009200"});
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }
        
        [Fact]
        public async Task SendSms_ShouldReturn_RedirectOnInvalidMobileNumber()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });
            
            
            var result = await _controller.SendSms(new SendSmsRequest() {MobileNumber = "MobileNumber"});
            var redirectResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/save-my-progress/reference?e=2", redirectResult.Url);
        }
        
        [Fact]
        public async Task SendSms_ShouldReturn_RedirectOnMissingMobileNumber()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });
            
            var result = await _controller.SendSms(new SendSmsRequest() {MobileNumber = ""});
            var redirectResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/save-my-progress/reference?e=1", redirectResult.Url);
        }
        
        [Fact]
        public async Task SendSms_ShouldReturn_RedirectToSmsOnSuccess()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });

            _apiServices.SendSms(
                Arg.Any<string>(), 
                "0790009200", 
                Arg.Any<string>(), 
                "201904-Abc123",
                Arg.Any<Guid>()).Returns(Task.FromResult(new NotifyResponse { IsSuccess = true }));
            
            _apiServices.Reload("201904-Abc123", Arg.Any<Guid>()).Returns(Task.FromResult(new AssessmentQuestionResponse
            {
                ReloadCode = "Abc123"
            }));
            
            var result = await _controller.SendSms(new SendSmsRequest() {MobileNumber = "0790009200"});
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("SmsSent", redirectResult.ActionName);
        }
        
        [Fact]
        public async Task SendSms_ShouldReturn_RedirectToSmsOnNotifyFailure()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });

            _apiServices.SendSms(
                Arg.Any<string>(), 
                "0790009200", 
                Arg.Any<string>(), 
                "201904-Abc123",
                Arg.Any<Guid>()).Returns(Task.FromResult(new NotifyResponse { IsSuccess = false }));

            _apiServices.Reload("201904-Abc123", Arg.Any<Guid>()).Returns(Task.FromResult(new AssessmentQuestionResponse
            {
                ReloadCode = "Abc123"
            }));
            
            var result = await _controller.SendSms(new SendSmsRequest() {MobileNumber = "0790009200"});
            var redirectResult = Assert.IsType<RedirectResult>(result);
            
            Assert.Equal("/save-my-progress/reference?e=3", redirectResult.Url);
        }
        
        [Theory]
        [InlineData("1", "Enter a phone number")]
        [InlineData("2", "Enter a mobile phone number, like 07700 900 982.")]
        public async Task ReferenceNumber_ShouldReturn_CorrectErrorMessageForInput(string error, string expectedMessage)
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });

            var result = await _controller.SmsInput(error);
            var viewResult = Assert.IsType<ViewResult>(result);
            
            Assert.Equal("SmsInput", viewResult.ViewName);

            var viewModel = Assert.IsType<SaveProgressViewModel>(viewResult.Model);
            
            Assert.True(viewModel.HasError);
            Assert.Equal(expectedMessage, viewModel.ErrorMessage);
        }
        
        [Fact]
        public async Task ReferenceNumber_ShouldReturn_500WhenFailsToSendSms()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Returns(x => { 
                    x[1] = Encoding.UTF8.GetBytes("201904-Abc123");
                    return true;
                });

            var result = await _controller.SmsInput("3");
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }
        
        [Fact]
        public async Task ReferenceNumber_ShouldReturn_500OnError()
        {
            _session.TryGetValue("session-id", out Arg.Any<byte[]>())
                .Throws(new Exception());

            var result = await _controller.SmsInput();
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("Error500", viewResult.ActionName);
            Assert.Equal("Error", viewResult.ControllerName);
        }
    }
}