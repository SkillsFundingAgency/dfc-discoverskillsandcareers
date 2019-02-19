using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Azure.Documents;

namespace Dfc.UnitTests.Fakes
{
    public class FakeQuestionRepository : IQuestionRepository
    {
        public Task<Document> CreateQuestion(Question question)
        {
            throw new System.NotImplementedException();
        }

        public Task<QuestionSetInfo> GetCurrentQuestionSetVersion()
        {
            var result = new QuestionSetInfo()
            {
                AssessmentType = "short",
                MaxQuestions = 4,
                QuestionSetVersion = "fakeset"
            };
            return Task.FromResult<QuestionSetInfo>(result);
        }

        public Task<Question> GetQuestion(string questionId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Question> GetQuestion(int questionNumber, string questionSetVersion)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Question>> GetQuestions(string assessmentType, string version)
        {
            throw new System.NotImplementedException();
        }

        public Task<QuestionSetInfo> GetQuestionSetInfo(string version)
        {
            throw new System.NotImplementedException();
        }
    }
}
