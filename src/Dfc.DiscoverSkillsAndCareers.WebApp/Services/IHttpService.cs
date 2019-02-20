using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Services
{
    public interface IHttpService
    {
        Task<string> GetString(string url);
        Task<string> PostData(string url, object data);
    }
}
