using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public abstract class BaseController : Controller
    {
        public IFormCollection FormData { get; private set; }
        public NameValueCollection QueryDictionary { get; private set; }

        protected void AppendCookie(string sessionId)
        {
            HttpContext.Session.SetString("session-id", sessionId);
        }

        protected async Task<string> TryGetSessionId(HttpRequest request)
        {
            string sessionId = string.Empty;
            string cookieSessionId = HttpContext.Session.GetString("session-id");

            sessionId = cookieSessionId;

            QueryDictionary = System.Web.HttpUtility.ParseQueryString(request.QueryString.ToString());
            var code = QueryDictionary.Get("sessionId");
            if (string.IsNullOrEmpty(code) == false)
            {
                sessionId = code;
            }

            if (request.HasFormContentType)
            {
                try
                {
                    FormData = await request.ReadFormAsync();
                    string formSessionId = GetFormValue("sessionId");

                    if (string.IsNullOrEmpty(formSessionId) == false)
                    {
                        sessionId = formSessionId;
                    }
                }
                catch { };
            }
            return sessionId;
        }

        protected string GetFormValue(string key)
        {
            if (FormData == null) return null;
            StringValues value;
            FormData.TryGetValue(key, out value);
            if (value.Count == 0) return null;
            return value[0];
        }

        protected int? GetFormNumberValue(string key)
        {
            var value = GetFormValue(key);
            int number;
            if (int.TryParse(value, out number))
            {
                return number;
            }
            return null;
        }
    }
}
