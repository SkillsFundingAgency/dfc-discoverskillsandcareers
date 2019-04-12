using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public interface IContentDataProcessor<T> where T : IContentPage
    {
        Task RunOnce(ILogger logger, string siteFinityType, string contentType);
    }
}
