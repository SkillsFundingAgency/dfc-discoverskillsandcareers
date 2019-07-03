using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public class LocalShortTraitRepository : IShortTraitRepository
    {
        private Trait[] Data { get; set; }
        
        public LocalShortTraitRepository()
        {
            using (var stream = Assembly.GetAssembly(typeof(LocalShortTraitRepository)).GetManifestResourceStream("Dfc.DiscoverSkillsAndCareers.Repositories.data.Traits.json"))
            using(var sr = new StreamReader(stream))
            {
                Data = JsonConvert.DeserializeObject<Trait[]>(sr.ReadToEnd());
            }
        }

        public Task CreateTrait(Trait trait, string partitionKey = "traits")
        {
            return Task.CompletedTask;
        }

        public Task<Trait[]> GetTraits(string partitionKey = "traits")
        {
            return Task.FromResult(Data);
        }
    }
}