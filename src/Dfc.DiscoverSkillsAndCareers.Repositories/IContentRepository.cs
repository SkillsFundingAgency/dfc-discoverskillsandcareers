using Dfc.DiscoverSkillsAndCareers.Models;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public interface IContentRepository
    {
        Task<Content> GetContent(string contentType);
        Task CreateContent(Content contentModel);
    }
}
