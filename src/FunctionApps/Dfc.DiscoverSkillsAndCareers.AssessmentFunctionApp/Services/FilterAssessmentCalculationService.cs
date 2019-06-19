using System;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Dfc.DiscoverSkillsAndCareers.Models.Extensions;
using Microsoft.AspNetCore.Mvc.Formatters.Json.Internal;
using Microsoft.Extensions.Logging;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services
{
    public class FilterAssessmentCalculationService : IFilterAssessmentCalculationService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IJobCategoryRepository _jobCategoryRepository;

        public FilterAssessmentCalculationService(IQuestionRepository questionRepository, IJobCategoryRepository jobCategoryRepository)
        {
            _questionRepository = questionRepository;
            _jobCategoryRepository = jobCategoryRepository;
        }

        public async Task CalculateAssessment(UserSession userSession, ILogger log)
        {
            foreach (var jobCategoryState in userSession.FilteredAssessmentState.JobCategoryStates.Where(j => j.IsComplete(userSession.FilteredAssessmentState.RecordedAnswers)))
            {
                var questions = await _questionRepository.GetQuestions(jobCategoryState.QuestionSetVersion);
                var category = await _jobCategoryRepository.GetJobCategory(jobCategoryState.JobCategoryCode);

                var jobProfiles =
                    jobCategoryState.Skills.SelectMany((s, i) =>
                        {
                            var c = category.Skills.First(cs => cs.ONetAttribute.EqualsIgnoreCase(s.Skill));
                            return c.JobProfiles.Select(p => new
                            {
                                QuestionNumber = s.QuestionNumber,
                                Profile = p.JobProfile,
                                Answer = p.Included ? AnswerOption.Yes : AnswerOption.No
                            });
                        })
                        .GroupBy(p => p.Profile)
                        .Select(g => new {Profile = g.Key, Answers = g.OrderBy(q => q.QuestionNumber).Select(q => q.Answer)});
                
                
                var categoryAnswers =
                    userSession.FilteredAssessmentState.GetAnswersForCategory(jobCategoryState.JobCategoryCode);
                
                var answers =
                        categoryAnswers
                            .OrderBy(a => a.QuestionNumber)
                            .Select(a => a.SelectedOption)
                            .ToArray();

                var suggestedProfiles = new List<string>();
                
                foreach (var jobProfile in jobProfiles)
                {
                    if (jobProfile.Answers.SequenceEqual(answers, EqualityComparer<AnswerOption>.Default))
                    {
                        suggestedProfiles.Add(jobProfile.Profile);
                    }
                }
                
                var jobCategoryResult =
                    userSession.ResultData.JobCategories.Single(jf => jf.JobCategoryCode.EqualsIgnoreCase(jobCategoryState.JobCategoryCode));
                
                jobCategoryResult.FilterAssessmentResult = new FilterAssessmentResult
                {
                    JobFamilyName = jobCategoryState.JobCategoryName,
                    CreatedDt = DateTime.UtcNow,
                    QuestionSetVersion = userSession.CurrentQuestionSetVersion,
                    MaxQuestions = userSession.MaxQuestions,
                    SuggestedJobProfiles = suggestedProfiles,
                    RecordedAnswerCount = userSession.RecordedAnswers.Length,
                    RecordedAnswers = userSession.RecordedAnswers.ToArray(),
                    WhatYouToldUs = ComputeWhatYouToldUs(categoryAnswers, questions).Distinct().ToArray()
                }; 
            }
            
            
            userSession.UpdateJobCategoryQuestionCount();
        }

        public List<string> ComputeWhatYouToldUs(Answer[] categoryAnswers, Question[] questions)
        {
            var whatYouToldUs = new List<string>();

            foreach (var answer in categoryAnswers)
            {
                String text = null;
                if (answer.SelectedOption == AnswerOption.No)
                {
                    text = questions
                        .FirstOrDefault(q => q.QuestionId.EqualsIgnoreCase(answer.QuestionId))?.NegativeResultDisplayText;
                }
                else
                {
                    text = questions
                        .FirstOrDefault(q => q.QuestionId.EqualsIgnoreCase(answer.QuestionId))
                        ?.PositiveResultDisplayText;
                }

                if (!String.IsNullOrWhiteSpace(text))
                {
                    whatYouToldUs.Add(text);
                }
            }

            return whatYouToldUs;
        }
    }
}
