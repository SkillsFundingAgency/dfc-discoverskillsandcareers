using Dfc.DiscoverSkillsAndCareers.Models;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Services
{
    public interface IAssessmentCalculationService
    {
        Task CalculateShortAssessment(UserSession userSession);
    }
}
