using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public interface IQuestionSetRepository
    {
        Task<QuestionSet> GetCurrentQuestionSet(string assessmentType, string title);
        Task<Document> CreateQuestionSet(QuestionSet questionSet);
        Task<QuestionSet> GetQuestionSetVersion(string assessmentType, string title, int version);
    }
}
