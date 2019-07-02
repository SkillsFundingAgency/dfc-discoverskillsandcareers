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
    public class LocalQuestionRepository : IQuestionRepository
    {
        private Question[] Data { get; set; }
        
        public LocalQuestionRepository()
        {
            using (var stream = Assembly.GetAssembly(typeof(LocalShortTraitRepository)).GetManifestResourceStream("Dfc.DiscoverSkillsAndCareers.Repositories.data.Questions.json"))
            using(var sr = new StreamReader(stream))
            {
                Data = JsonConvert.DeserializeObject<Question[]>(sr.ReadToEnd());
            }
        }

        public Task<Question> GetQuestion(string questionId)
        {
            return Task.FromResult(Data.FirstOrDefault(q => StringExtensions.EqualsIgnoreCase(q.QuestionId, questionId)));
        }

        public Task<Document> CreateQuestion(Question question)
        {
            return Task.FromResult(new Document());
        }

        public Task<Question[]> GetQuestions(string assessmentType, string title, int version)
        {
            return Task.FromResult(Data
                .Where(q => q.PartitionKey == $"{assessmentType.ToLower()}-{title.ToLower()}-{version}").ToArray());
        }

        public async Task<Question> GetQuestion(int questionNumber, string questionSetVersion)
        {
            var questionId = $"{questionSetVersion}-{questionNumber}";
            return await GetQuestion(questionId);
        }

        public Task<Question[]> GetQuestions(string questionSetVersion)
        {
            return Task.FromResult(Data.Where(x => x.PartitionKey == questionSetVersion).ToArray());
        }
    }
}