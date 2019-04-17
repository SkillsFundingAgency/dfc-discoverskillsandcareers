using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public interface IFilteredQuestionSetDataProcessor
    {
        Task RunOnce(ILogger logger);
    }
}
