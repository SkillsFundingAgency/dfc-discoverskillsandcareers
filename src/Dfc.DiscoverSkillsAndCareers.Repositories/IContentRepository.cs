using Dfc.DiscoverSkillsAndCareers.Models;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public interface IContentRepository
    {
        Task<Content> GetContent(string contentType);
    }
}
