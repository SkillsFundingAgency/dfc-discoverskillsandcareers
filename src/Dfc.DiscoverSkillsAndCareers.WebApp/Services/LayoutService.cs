using Microsoft.AspNetCore.Http;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Services
{
    public class LayoutService : ILayoutService
    {
        public string GetLayout(HttpRequest request)
        {
            request.Query.TryGetValue("isStandalone", out var output);
            return output.Count > 0 ? "_LayoutStandalone" : "_LayoutComposite";
        }
    }
}