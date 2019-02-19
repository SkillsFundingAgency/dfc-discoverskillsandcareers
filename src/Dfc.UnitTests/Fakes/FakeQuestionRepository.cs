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
            var result = new Question()
            {
                QuestionId = "1",
                Texts = new List<QuestionText>()
                  {
                       new QuestionText() { LanguageCode = "en", Text = "Unit test question" }
                  },
                Order = 1,
                TraitCode = "DOER"
            };
            return Task.FromResult<Question>(result);
        }

        public Task<Question> GetQuestion(int questionNumber, string questionSetVersion)
        {
            var result = new Question()
            {
                QuestionId = "1",
                Texts = new List<QuestionText>()
                  {
                       new QuestionText() { LanguageCode = "en", Text = "Unit test question" }
                  },
                Order = 1,
                TraitCode = "DOER"
            };
            return Task.FromResult<Question>(result);
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
