using System;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Services
{
    public interface IHttpService 
    {
        Task<string> GetString(string url, Guid? dssCorrelationId);
        Task<string> PostData(string url, object data, Guid? dssCorrelationId);
        void SetAuthCode(string code);
    }
}
