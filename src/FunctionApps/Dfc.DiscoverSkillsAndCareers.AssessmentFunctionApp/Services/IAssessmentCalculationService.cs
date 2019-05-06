using Dfc.DiscoverSkillsAndCareers.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services
{
    public interface IAssessmentCalculationService
    {
        Task CalculateAssessment(UserSession userSession, ILogger log);
    }
}
