using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using System;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Services
{
    public interface IApiServices
    {
        Task<T> GetContentModel<T>(string contentType, Guid correlationId) where T : class;
        Task<NewSessionResponse> NewSession(Guid correlationId, string assessmentType);
        Task<AssessmentQuestionResponse> Question(string sessionId, string assessment, int questionNumber, Guid correlationId);
        Task<PostAnswerResponse> PostAnswer(string sessionId, PostAnswerRequest postAnswerRequest, Guid correlationId);
        Task<ResultsResponse> Results(string sessionId, Guid correlationId);
        Task<NewSessionResponse> StartFilteredForJobCategory(Guid correlationId, string sessionId, string jobCategory);
        Task<ResultsJobCategoryResult> ResultsForJobCategory(string sessionId, string jobCategory, Guid correlationId);
        Task<NotifyResponse> SendEmail(string domain, string emailAddress, string templateId, string sessionId, Guid correlationId);
        Task<NotifyResponse> SendSms(string domain, string mobileNumber, string templateId, string sessionId, Guid correlationId);
        Task<AssessmentQuestionResponse> Reload(string sessionId, Guid correlationId);
    }
}
