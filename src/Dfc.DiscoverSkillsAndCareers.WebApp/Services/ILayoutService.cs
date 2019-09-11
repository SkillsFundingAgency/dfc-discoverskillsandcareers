using Microsoft.AspNetCore.Http;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Services
{
    public interface ILayoutService
    {
        string GetLayout(HttpRequest request);
    }
}