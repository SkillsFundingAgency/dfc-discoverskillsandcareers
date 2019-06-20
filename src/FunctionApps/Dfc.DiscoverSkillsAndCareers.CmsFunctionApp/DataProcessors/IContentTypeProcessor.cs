using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public interface IContentTypeProcessor<T>
    {
        Task RunOnce(ILogger logger);
    }
}
