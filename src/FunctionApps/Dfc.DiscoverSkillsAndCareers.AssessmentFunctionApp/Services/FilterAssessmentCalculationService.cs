using System;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Formatters.Json.Internal;
using Microsoft.Extensions.Logging;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services
{
    public class FilterAssessmentCalculationService : IFilterAssessmentCalculationService
    {
        readonly IQuestionRepository _questionRepository;
        readonly IJobProfileRepository _jobProfileRepository;

        public FilterAssessmentCalculationService(
            IQuestionRepository questionRepository,
            IJobProfileRepository jobProfileRepository)
        {
            _questionRepository = questionRepository;
            _jobProfileRepository = jobProfileRepository;
        }

        public async Task CalculateAssessment(UserSession userSession, ILogger log)
        {            
            // All questions for this set version
            var questions = await _questionRepository.GetQuestions(userSession.CurrentQuestionSetVersion);

            // All the job profiles for this job category
            var jobFamilyName = userSession.FilteredAssessmentState.JobFamilyName;
            var allJobProfiles = await _jobProfileRepository.JobProfilesForJobFamily(jobFamilyName);
            var suggestedJobProfiles = allJobProfiles.ToList();

            // All answers in order 
            var answers = userSession.CurrentRecordedAnswers
                .OrderBy(x => x.QuestionNumber)
                .ToList();

            var whatYouToldUs = new List<string>();

            // Iterate through removing all in a "guess-who" fashion
            foreach (var answer in answers)
            {
                var question = questions.First(x => x.QuestionId == answer.QuestionId);
                //TODO: Fix this up
//                if (question.FilterTrigger == "No" && answer.SelectedOption == AnswerOption.No)
//                {
//                    suggestedJobProfiles.RemoveAll(x => question.ExcludesJobProfiles.Contains(x.Title));
//                }
//                else if (question.FilterTrigger == "Yes" && answer.SelectedOption == AnswerOption.Yes)
//                {
//                    suggestedJobProfiles.RemoveAll(x => question.ExcludesJobProfiles.Contains(x.Title));
//                }
//
//                if (answer.SelectedOption == AnswerOption.No)
//                {
//                    whatYouToldUs.Add(question.NegativeResultDisplayText);
//                }
//                else if (answer.SelectedOption == AnswerOption.Yes)
//                {
//                    whatYouToldUs.Add(question.PositiveResultDisplayText);
//                }
            }

            IDictionary<string, string> socCodes = new Dictionary<string, string>();
            IDictionary<string, int> dup = new Dictionary<string, int>();
            
            foreach (var jp in suggestedJobProfiles)
            {
                if (socCodes.ContainsKey(jp.SocCode))
                {
                    if (String.Equals(jp.Title, socCodes[jp.SocCode], StringComparison.InvariantCultureIgnoreCase))
                    {
                        log.LogInformation($"Duplicate job profile soc-code found {jp.SocCode}:{jp.Title} matches {jp.SocCode}:{socCodes[jp.SocCode]} augmenting SOC-CODE");

                        if (!dup.TryGetValue(jp.SocCode, out var count))
                        {
                            count = 0;
                        }
                        
                        count++;
                        dup[jp.SocCode] = count;
                        socCodes.Add($"{jp.SocCode}-{count}", jp.Title);

                    }
                }
                else
                {
                    socCodes.Add(jp.SocCode, jp.Title);
                }
            }
            
            
            var jobFamily = userSession.ResultData.JobFamilies.First(jf => String.Equals(jf.JobFamilyName,
                userSession.FilteredAssessmentState.JobFamilyName, StringComparison.InvariantCultureIgnoreCase));

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
