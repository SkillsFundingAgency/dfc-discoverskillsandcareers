using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public interface IJobCategoryDataProcessor
    {
        Task RunOnce();
    }
}
