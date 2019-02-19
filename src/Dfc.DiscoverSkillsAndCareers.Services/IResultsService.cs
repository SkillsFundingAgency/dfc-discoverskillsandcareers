using Dfc.DiscoverSkillsAndCareers.Models;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Services
{
    public interface IResultsService
    {
        Task CalculateShortAssessment(UserSession userSession);
    }
}
