using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public interface IQuestionRepository
    {
        Task<Question> GetQuestion(string questionId);
        Task<Document> CreateQuestion(Question question);
        Task<QuestionSetInfo> GetCurrentQuestionSetVersion();
        Task<List<Question>> GetQuestions(string assessmentType, string version);
        Task<QuestionSetInfo> GetQuestionSetInfo(string version);
        Task<Question> GetQuestion(int questionNumber, string questionSetVersion);
    }
}
