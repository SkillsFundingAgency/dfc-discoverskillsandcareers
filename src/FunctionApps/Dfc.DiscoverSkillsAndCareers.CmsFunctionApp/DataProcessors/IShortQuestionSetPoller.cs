using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public interface IShortQuestionSetPoller
    {
        Task RunOnce();
    }
}
