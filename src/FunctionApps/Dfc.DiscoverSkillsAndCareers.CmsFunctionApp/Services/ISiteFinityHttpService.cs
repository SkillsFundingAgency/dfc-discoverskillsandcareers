using System;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services
{
    public interface ISiteFinityHttpService
    {
        Task<string> GetString(string url);
        Task<string> PostData(string url, object data);
        
        Task<string> PostData(string url, string data);
        Task Delete(string url);
    }
}
