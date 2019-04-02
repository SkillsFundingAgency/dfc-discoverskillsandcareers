using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services
{
    public interface INotifyClient
    {
        Task SendEmail(string domain, string emailAddress, string templateId, string sessionId);
        Task SendSms(string domain, string mobileNumber, string templateId, string sessionId);
    }
}
