using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public interface IGetShortQuestionSetData
    {
        Task<ShortQuestionSet> GetData(string url);
    }
}
