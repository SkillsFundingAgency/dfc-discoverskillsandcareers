using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public interface IContentDataProcessor<T> where T : IContentPage
    {
        Task RunOnce(string siteFinityType, string contentType);
    }
}
