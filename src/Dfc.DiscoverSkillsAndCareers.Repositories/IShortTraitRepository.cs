using Dfc.DiscoverSkillsAndCareers.Models;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public interface IShortTraitRepository
    {
        Task<Trait> GetShortTrait(string name, string partitionKey);
        Task CreateTrait(Trait trait, string partitionKey);
        Task<Trait[]> GetTraits(string partitionKey);
    }
}
