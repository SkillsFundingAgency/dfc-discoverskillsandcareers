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

        public Task<QuestionSet> GetCurrentQuestionSetVersion()
        {
            var result = new QuestionSet()
            {
                AssessmentType = "short",
                MaxQuestions = 4,
                QuestionSetVersion = "fakeset"
            };
            return Task.FromResult<QuestionSet>(result);
        }

        public Task<Question> GetQuestion(string questionId)
        {
            var result = new Question()
            {
                QuestionId = "1",
                Texts = new []
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
                Texts = new []
                  {
                       new QuestionText() { LanguageCode = "en", Text = "Unit test question" }
                  },
                Order = 1,
                TraitCode = "DOER"
            };
            return Task.FromResult<Question>(result);
        }

        public Task<Question[]> GetQuestions(string assessmentType, string title, int version)
        {
            throw new System.NotImplementedException();
        }

        public Task<QuestionSet> GetQuestionSetInfo(string version)
        {
            throw new System.NotImplementedException();
        }
    }
}
