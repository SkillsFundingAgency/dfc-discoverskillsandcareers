using System.IO.Pipes;
using System.Linq;
using Dfc.DiscoverSkillsAndCareers.Models.Extensions;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class JobCategoryState
    {
        [JsonProperty("questionSetVersion")]
        public string QuestionSetVersion { get; set; }

        [JsonProperty("questions")] 
        public JobCategorySkill[] Skills { get; set; } = { };
        
        [JsonProperty("jobCategoryName")]
        public string JobCategoryName { get; set; }
        
        [JsonProperty("jobCategoryCode")]
        public string JobCategoryCode { get; set; }

        [JsonProperty("currentQuestion")] 
        public int CurrentQuestion { get; set; } = 1;
        
        [JsonIgnore]
        public string JobFamilyNameUrlSafe => JobCategoryName?.ToLower()?.Replace(" ", "-");

        public int UnansweredQuestions(Answer[] answers) =>
            answers == null 
                ? Skills.Length 
                : Skills.Count(s => !answers.Any(a => a.TraitCode.EqualsIgnoreCase(s.Skill)));

        public bool IsComplete(Answer[] answers) =>
            answers != null 
            && Skills.All(s => answers.Any(a => s.Skill.EqualsIgnoreCase(a.TraitCode)));
            
    }
}