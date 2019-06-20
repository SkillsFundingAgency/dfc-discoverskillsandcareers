using Dfc.DiscoverSkillsAndCareers.Models;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public interface IShortTraitRepository
    {
        Task CreateTrait(Trait trait, string partitionKey = "traits");
        Task<Trait[]> GetTraits(string partitionKey = "traits");
    }
}
