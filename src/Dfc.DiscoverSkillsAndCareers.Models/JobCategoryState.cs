using System;
using System.IO.Pipes;
using System.Linq;
using Dfc.DiscoverSkillsAndCareers.Models.Extensions;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class JobCategoryState
    {
        public JobCategoryState(string code, string name, string questionSetVersion, JobCategorySkill[] skills, string currentQuestionId = null)
        {
            if(skills?.Length == 0)
                throw new ArgumentException($"No skills were passed for job category {name}", nameof(skills));
                
            JobCategoryCode = code;
            JobCategoryName = name;
            QuestionSetVersion = questionSetVersion;
            Skills = skills.OrderBy(s => s.QuestionNumber).ToArray();

            if (!String.IsNullOrWhiteSpace(currentQuestionId))
            {
                CurrentQuestion = Skills.First(q => q.QuestionId == currentQuestionId).QuestionNumber;
            }
            else
            {
                CurrentQuestion = Skills.First().QuestionNumber;
            }
        }
        
        [JsonProperty("questionSetVersion")]
        public string QuestionSetVersion { get; set; }

        [JsonProperty("questions")] 
        public JobCategorySkill[] Skills { get; set; }
        
        [JsonProperty("jobCategoryName")]
        public string JobCategoryName { get; set; }
        
        [JsonProperty("jobCategoryCode")]
        public string JobCategoryCode { get; set; }

        [JsonProperty("currentQuestion")]
        public int CurrentQuestion { get; private set; }
        

        [JsonIgnore]
        public string JobFamilyNameUrlSafe => JobCategoryName?.ToLower()?.Replace(" ", "-");

        [JsonIgnore]
        public string CurrentQuestionId => Skills.First(q => q.QuestionNumber == CurrentQuestion).QuestionId;

        public int UnansweredQuestions(Answer[] answers) =>
            answers == null 
                ? Skills.Length 
                : Skills.Count(s => !answers.Any(a => a.TraitCode.EqualsIgnoreCase(s.Skill)));

        public bool IsComplete(Answer[] answers) =>
            answers != null 
            && Skills.All(s => answers.Any(a => s.Skill.EqualsIgnoreCase(a.TraitCode)));

        public void SetCurrentQuestion(int questionNumber)
        {
            CurrentQuestion = Skills.First(q => q.QuestionNumber == questionNumber).QuestionNumber;
        }
   
            
    }
}