using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public interface IFunctionalCompetencyDataProcessor
    {
        Task RunOnce(ILogger logger);
    }
}
