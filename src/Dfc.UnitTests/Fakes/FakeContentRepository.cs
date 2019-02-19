using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using System.Threading.Tasks;

namespace Dfc.UnitTests.Fakes
{
    public class FakeContentRepository : IContentRepository
    {
        public Task<Content> GetContent(string contentType)
        {
            var result = new Content()
            {
                Id = contentType,
                ContentType = contentType,
                ContentData = "{}"
            };
            return Task.FromResult<Content>(result);
        }
    }
}
