using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public interface IShortTraitDataProcessor
    {
        Task RunOnce(ILogger log);
    }
}
