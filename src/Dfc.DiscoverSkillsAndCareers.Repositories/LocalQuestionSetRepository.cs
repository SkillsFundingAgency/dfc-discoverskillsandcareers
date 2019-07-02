using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Models.Extensions;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public class LocalQuestionSetRepository : IQuestionSetRepository
    {
        private QuestionSet[] Data { get; set; }
        
        public LocalQuestionSetRepository()
        {
            using (var stream = Assembly.GetAssembly(typeof(LocalShortTraitRepository)).GetManifestResourceStream("Dfc.DiscoverSkillsAndCareers.Repositories.data.QuestionSets.json"))
            using(var sr = new StreamReader(stream))
            {
                Data = JsonConvert.DeserializeObject<QuestionSet[]>(sr.ReadToEnd());
            }
        }


        public Task<QuestionSet> GetCurrentQuestionSet(string assessmentType)
        {
            return Task.FromResult(Data.FirstOrDefault(q => StringExtensions.EqualsIgnoreCase(q.AssessmentType, assessmentType) && q.IsCurrent));
        }

        public Task<Document> CreateOrUpdateQuestionSet(QuestionSet questionSet)
        {
            return Task.FromResult(new Document());
        }

        public Task<QuestionSet> GetQuestionSetVersion(string assessmentType, string title, int version)
        {
            var titleLowercase = title.ToLower().Replace(" ", "-");
            return Task.FromResult(Data.FirstOrDefault(x => x.AssessmentType == assessmentType && x.QuestionSetKey == titleLowercase && x.Version == version));
        }

        public Task<List<QuestionSet>> GetCurrentFilteredQuestionSets()
        {
            return Task.FromResult(Data.Where(x => x.AssessmentType == "filtered" && x.IsCurrent == true).ToList());
        }

        public Task<QuestionSet> GetLatestQuestionSetByTypeAndKey(string assessmentType, string key)
        {
            var keyLowerCase = key.Replace(" ", "-").ToLower();
            return Task.FromResult(Data
                .Where(x => x.AssessmentType == assessmentType && x.QuestionSetKey == keyLowerCase)
                .OrderByDescending(x => x.Version).FirstOrDefault());
        }
    }
}