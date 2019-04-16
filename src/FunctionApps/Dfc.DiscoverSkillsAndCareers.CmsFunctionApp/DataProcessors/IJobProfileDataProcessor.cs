using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public interface IJobProfileDataProcessor
    {
        Task RunOnce(ILogger logger);
    }
}
