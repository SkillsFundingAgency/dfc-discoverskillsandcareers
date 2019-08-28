using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Notify.Interfaces;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Xunit;

namespace Dfc.UnitTests.FunctionTests
{
    public class NotifyClientTests
    {
        private NotifyClient _sut;
        private readonly INotificationClient _notificationClient;

        public NotifyClientTests()
        {
            _notificationClient = Substitute.For<INotificationClient>();
            _sut = new NotifyClient(_notificationClient);
        }

        [Fact]
        public void Validate_Personalisation_Dictionary()
        {
            var result = NotifyClient.GetPersonalisation("my.com", "abc123");
            
            Assert.Equal("ABC1 23", result["session_id"]);
            Assert.Equal(DateTime.Now.ToString("dd MM yyyy"), result["assessment_date"]);
            Assert.Equal($"my.com/reload?sessionId=abc123", result["reload_url"]);
        }

        [Fact]
        public async Task Check_SendEmail_HasCorrectParameters()
        {
            var templateId = Guid.Empty.ToString();
            
            await _sut.SendEmail("my.com", "test@my.com", templateId, "abc123");
            
            _notificationClient.Received(1).SendEmail("test@my.com", templateId, Arg.Any<Dictionary<String, dynamic>>());

        }
        
        [Fact]
        public async Task Check_SendSms_HasCorrectParameters()
        {
            var templateId = Guid.Empty.ToString();
            
            await _sut.SendSms("my.com", "07000007909", templateId, "abc123");
            
            _notificationClient.Received(1).SendSms("07000007909", templateId, Arg.Any<Dictionary<String, dynamic>>());

        }
    }
}