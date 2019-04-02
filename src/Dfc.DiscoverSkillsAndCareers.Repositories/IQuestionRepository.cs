using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public interface IQuestionRepository
    {
        Task<Question> GetQuestion(string questionId);
        Task<Document> CreateQuestion(Question question);
        Task<Question[]> GetQuestions(string assessmentType, string title, int version);
        Task<Question> GetQuestion(int questionNumber, string questionSetVersion);
        Task<Question[]> GetQuestions(string questionSetVersion);
    }
}
