using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public class Constants
    {
        public const string CompositePathHeader = "x-composite-base-route";
    }
    
    public static class UrlExtensions
    {
        public static string TryApplyCompositeRoute(this IUrlHelper source, string str)
        {
            if (source.ActionContext.HttpContext.Request.Headers.TryGetValue(Constants.CompositePathHeader,
                out var path))
            {
                return $"/{path[0].TrimStart('/')}/{str.TrimStart('/')}";
            }

            return str;
        }
    }
    
    public class CompositeUIBaseController : Controller
    {
        public string ApplyCompositeBasePath(string url)
        {
            
                if (Request.Headers.TryGetValue(Constants.CompositePathHeader, out var path))
                    return $"/{path[0].TrimStart('/')}/{url.TrimStart('/')}";

                return url;
               
        }
        
        public override RedirectResult Redirect(string url)
        {
            return new RedirectResult(ApplyCompositeBasePath(url));
        }
        
    }
    
    public abstract class BaseController : CompositeUIBaseController //Controller
    {
        public IFormCollection FormData { get; private set; }
        public NameValueCollection QueryDictionary { get; private set; }

        protected void AppendCookie(string sessionId)
        {
            HttpContext.Session.SetString("session-id", sessionId);
        }

        protected async Task<string> TryGetSessionId([CallerMemberName]string memberName = null)
        {
            string sessionId = string.Empty;
            string cookieSessionId = HttpContext.Session.GetString("session-id");
            var request = Request;
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

            if (String.IsNullOrWhiteSpace(sessionId) && Request.Path.ToString() != "/")
            {
                var logger = Request.HttpContext.RequestServices.GetService<ILogger<BaseController>>();
                logger.LogWarning($"Unable to get session Id in  call {memberName} - {request.GetDisplayUrl()}");
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
        
    }
}
