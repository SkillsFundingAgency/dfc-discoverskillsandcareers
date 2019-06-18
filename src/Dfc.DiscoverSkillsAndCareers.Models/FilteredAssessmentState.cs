using System;
using System.Collections.Generic;
using System.Linq;
using Dfc.DiscoverSkillsAndCareers.Models.Extensions;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class FilteredAssessmentState : AssessmentStateBase
    {

        [JsonProperty("jobCategories")]
        public List<JobCategoryState> JobCategoryStates { get; set; } = new List<JobCategoryState>();

        [JsonIgnore] 
        public override int MaxQuestions => CurrentState.Skills.Length;
        
        [JsonProperty("recordedAnswers")]
        public Answer[] RecordedAnswers { get; set; } = {};
        
        [JsonProperty("currentFilterAssessmentCode")]
        public string CurrentFilterAssessmentCode { get; set; }

        [JsonIgnore]
        public override string QuestionSetVersion => CurrentState.QuestionSetVersion;

        [JsonIgnore]
        public override int CurrentQuestion
        {
            get => CurrentState?.CurrentQuestion ?? 0;
            protected set
            {
                if (CurrentState != null)
                {
                    CurrentState.CurrentQuestion = value;
                }
            }
        }

        [JsonIgnore]
        private JobCategoryState CurrentState =>
            JobCategoryStates.Single(jc => jc.JobCategoryCode.EqualsIgnoreCase(CurrentFilterAssessmentCode));

        [JsonIgnore]
        public override bool IsComplete
        {
            get
            {
                var complete = CurrentState.IsComplete(RecordedAnswers);
                if (complete && !CompleteDt.HasValue)
                {
                    CompleteDt = DateTime.UtcNow;
                }

                return complete;
            }
        }

        [JsonIgnore]
        public string JobFamilyNameUrlSafe => CurrentState.JobFamilyNameUrlSafe;

        public void CreateOrResetCategoryState(string questionSetVersion, Question[] questions, JobCategory category)
        {
            if (!TryGetJobCategoryState(category.Code, out var cat))
            {
                var skills = new List<JobCategorySkill>();
                
                foreach (var skill in category.Skills)
                {
                    var question = questions.FirstOrDefault(q => q.TraitCode.EqualsIgnoreCase(skill.ONetAttribute));

                    if (question != null)
                    {
                        skills.Add(new JobCategorySkill
                        {
                            Skill = skill.ONetAttribute,
                            QuestionNumber = question.Order,
                        });
                    }
                }

                cat = new JobCategoryState
                {
                    JobCategoryCode = category.Code,
                    JobCategoryName = category.Name,
                    Skills = skills.ToArray()
                };
                
                JobCategoryStates.Add(cat);
            }

            
            cat.QuestionSetVersion = questionSetVersion;
            cat.CurrentQuestion = cat.Skills[0].QuestionNumber;
        }

        public void RemoveAnswersForCategory(string jobCategoryCode)
        {
            var category = JobCategoryStates.FirstOrDefault(jc => jc.JobCategoryCode.EqualsIgnoreCase(jobCategoryCode));
            if (category != null)
            {
                RecordedAnswers = 
                    RecordedAnswers
                        .Where(a => !category.Skills.Any(s => s.Skill.EqualsIgnoreCase(a.TraitCode))).ToArray();
            }
        }
        
        public override int MoveToNextQuestion()
        {
            var number =  
                CurrentState?.Skills
                    .FirstOrDefault(q => !RecordedAnswers.Any(a => a.TraitCode.EqualsIgnoreCase(q.Skill)))
                    ?.QuestionNumber;

            if (number.HasValue)
            {
                CurrentQuestion = number.Value;
            }
            else
            {
                CurrentQuestion = CurrentState?.Skills[0].QuestionNumber ?? 0;
            }

            return CurrentQuestion;
        }

        public bool TryGetJobCategoryState(string jobCategoryCode, out JobCategoryState state)
        {
            state = JobCategoryStates.FirstOrDefault(s => s.JobCategoryCode.EqualsIgnoreCase(jobCategoryCode));
            return state != null;
        }

        public Answer[] GetAnswersForCategory(string jobCategoryCode)
        {
            if(!TryGetJobCategoryState(jobCategoryCode, out var state))
                return new Answer[]{};

            return RecordedAnswers
                .Where(a => state.Skills.Any(q => a.TraitCode.EqualsIgnoreCase(q.Skill)))
                .ToArray();
        }
    }
}