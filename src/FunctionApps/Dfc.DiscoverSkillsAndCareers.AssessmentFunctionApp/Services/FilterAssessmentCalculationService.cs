using System;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services
{
    public class FilterAssessmentCalculationService : IFilterAssessmentCalculationService
    {
        readonly IQuestionRepository QuestionRepository;
        readonly IJobProfileRepository JobProfileRepository;

        public FilterAssessmentCalculationService(
            IQuestionRepository questionRepository,
            IJobProfileRepository jobProfileRepository)
        {
            QuestionRepository = questionRepository;
            JobProfileRepository = jobProfileRepository;
        }

        public async Task CalculateAssessment(UserSession userSession, ILogger log)
        {            
            // All questions for this set version
            var questions = await QuestionRepository.GetQuestions(userSession.CurrentQuestionSetVersion);

            // All the job profiles for this job category
            var jobFamilyName = userSession.FilteredAssessmentState.JobFamilyName;
            var allJobProfiles = await JobProfileRepository.GetJobProfilesForJobFamily(jobFamilyName);
            var suggestedJobProfiles = allJobProfiles.ToList();

            // All answers in order 
            var answers = userSession.CurrentRecordedAnswers
                .OrderBy(x => x.QuestionNumber)
                .ToList();

            var whatYouToldUs = new List<string>();

            // Iterate through removing all in a "guess-who" fashion
            foreach (var answer in answers)
            {
                var question = questions.Where(x => x.QuestionId == answer.QuestionId).First();
                if (question.FilterTrigger == "No" && answer.SelectedOption == AnswerOption.No)
                {
                    suggestedJobProfiles.RemoveAll(x => question.ExcludesJobProfiles.Contains(x.Title));
                }
                else if (question.FilterTrigger == "Yes" && answer.SelectedOption == AnswerOption.Yes)
                {
                    suggestedJobProfiles.RemoveAll(x => question.ExcludesJobProfiles.Contains(x.Title));
                }

                if (answer.SelectedOption == AnswerOption.No)
                {
                    whatYouToldUs.Add(question.NegativeResultDisplayText);
                }
                else if (answer.SelectedOption == AnswerOption.Yes)
                {
                    whatYouToldUs.Add(question.PositiveResultDisplayText);
                }
            }

            // Update the filter assessment with the soc codes we are going to suggest
            // TODO: the quantity and order here is likely to change or handled on the UI?
            var socCodes = suggestedJobProfiles.Select(x => x.SocCode).ToArray();
            var jobFamily = userSession.ResultData.JobFamilies.First(jf => String.Equals(jf.JobFamilyName, userSession.FilteredAssessmentState.JobFamilyName, StringComparison.InvariantCultureIgnoreCase));

            jobFamily.FilterAssessment = new FilterAssessment
            {
                JobFamilyName = jobFamily.JobFamilyName,
                CreatedDt = DateTime.UtcNow,
                QuestionSetVersion = userSession.CurrentQuestionSetVersion,
                MaxQuestions = userSession.MaxQuestions,
                SuggestedJobProfiles = socCodes,
                RecordedAnswerCount = userSession.CurrentRecordedAnswers.Length,
                RecordedAnswers = userSession.CurrentRecordedAnswers.ToArray(),
                WhatYouToldUs = whatYouToldUs.Distinct().ToArray()
                
            };
        }
    }
}
