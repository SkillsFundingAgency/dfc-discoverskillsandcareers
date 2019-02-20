using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Services
{
    public interface IApiServices
    {
        Task<T> GetContentModel<T>(string contentType) where T : class;
        Task<NewSessionResponse> NewSession();
        Task<NextQuestionResponse> NextQuestion(string sessionId);
        Task<PostAnswerResponse> PostAnswer(string sessionId, PostAnswerRequest postAnswerRequest);
    }
}
