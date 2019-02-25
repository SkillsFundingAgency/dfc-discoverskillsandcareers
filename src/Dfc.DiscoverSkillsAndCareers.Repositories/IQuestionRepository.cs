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
        Task<List<Question>> GetQuestions(string assessmentType, string title, int version);
        Task<Question> GetQuestion(int questionNumber, string questionSetVersion);
    }
}
