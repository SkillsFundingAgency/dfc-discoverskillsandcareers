using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using System.Threading.Tasks;
using System.Linq;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services
{
    public class FilterAssessmentCalculationService : IAssessmentCalculationService
    {
        readonly IQuestionRepository QuestionRepository;

        public FilterAssessmentCalculationService(
            IQuestionRepository questionRepository)
        {
            QuestionRepository = questionRepository;
        }

        public async Task CalculateAssessment(UserSession userSession)
        {
            var questions = await QuestionRepository.GetQuestions(userSession.CurrentQuestionSetVersion);

            var suggestedJobProfiles = questions.ToList()
                .Select(x => x.ExcludesJobProfiles)
                .SelectMany(x => x)
                .Distinct()
                .ToList();

            var noanswers = userSession.CurrentRecordedAnswers
                .Where(x => x.SelectedOption == AnswerOption.No)
                .OrderBy(x => x.QuestionNumber)
                .ToList();

            foreach (var answer in noanswers)
            {
                var question = questions.Where(x => x.QuestionId == answer.QuestionId).First();
                var toremove = question.ExcludesJobProfiles;
                suggestedJobProfiles.RemoveAll(x => toremove.Contains(x));
            }

            var filterAssessment = userSession.CurrentFilterAssessment;

            userSession.CurrentFilterAssessment.SuggestedJobProfiles = suggestedJobProfiles.ToArray();
        }
    }
}
