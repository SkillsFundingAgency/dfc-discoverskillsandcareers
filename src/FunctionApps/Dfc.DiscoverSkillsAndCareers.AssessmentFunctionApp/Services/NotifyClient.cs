using Notify.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models.Extensions;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services
{
    public class NotifyClient : INotifyClient
    {
        readonly INotificationClient NotificationClient;

        public NotifyClient(INotificationClient notificationClient)
        {
            NotificationClient = notificationClient;
        }

        public static Dictionary<String, dynamic> GetPersonalisation(string domain, string sessionId) => new Dictionary<String, dynamic>
            {
                { "session_id", sessionId.FormatReferenceCode() },
                { "assessment_date",DateTime.Now.ToString("dd MM yyyy") },
                { "reload_url",  $"{domain}/reload?sessionId={sessionId}" }
            };

        public async Task SendEmail(string domain, string emailAddress, string templateId, string sessionId)
        {
            var personalisation = GetPersonalisation(domain, sessionId);
            var response = NotificationClient.SendEmail(emailAddress, templateId, personalisation);
            await Task.CompletedTask;
        }

        public async Task SendSms(string domain, string mobileNumber, string templateId, string sessionId)
        {
            var personalisation = GetPersonalisation(domain, sessionId);
            var response = NotificationClient.SendSms(mobileNumber, templateId, personalisation);
            await Task.CompletedTask;
        }
    }
}
