using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public interface IFilteredQuestionSetDataProcessor
    {
        Task RunOnce();
    }
}
