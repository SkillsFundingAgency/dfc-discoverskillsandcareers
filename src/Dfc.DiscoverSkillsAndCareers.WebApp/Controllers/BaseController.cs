using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public static class UrlExtensions
    {
        public static string TryApplyCompositeRoute(this IUrlHelper source, string str)
        {
            var settings = source.ActionContext.HttpContext.RequestServices.GetService<IOptions<AppSettings>>();

            return $"/{settings.Value.APIRootSegment.TrimStart('/')}/{str.TrimStart('/')}";
        }
    }

    public class CompositeUIBaseController : Controller
    {
        public override RedirectResult Redirect(string url)
        {
            var settings = Request.HttpContext?.RequestServices?.GetService<IOptions<AppSettings>>();
            var applicationUrl = !string.IsNullOrEmpty(settings?.Value.APIRootSegment.TrimStart('/'))
                ? $"/{settings?.Value.APIRootSegment.TrimStart('/')}/{url.TrimStart('/')}"
                : $"/{url.TrimStart('/')}";

            return new RedirectResult(applicationUrl);
        }
    }

    public abstract class BaseController : CompositeUIBaseController
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IDataProtector _dataProtector;

        public IFormCollection FormData { get; private set; }
        public NameValueCollection QueryDictionary { get; private set; }

        protected BaseController(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _dataProtector = _dataProtectionProvider.CreateProtector(nameof(BaseController));
        }

        protected void AppendCookie(string sessionId)
        {
            var value = _dataProtector.Protect(sessionId);
            Response.Cookies.Append(".dysac-session", value, new CookieOptions
            {
                Secure = true,
                IsEssential = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            });
        }

        protected async Task<string> TryGetSessionId([CallerMemberName]string memberName = null)
        {
            var request = Request;
            String sessionId = null;

            if (request.Cookies.TryGetValue(".dysac-session", out var cookieSessionId))
            {
                sessionId = _dataProtector.Unprotect(cookieSessionId);
            }

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
                    var formSessionId = GetFormValue("sessionId");

                    if (!string.IsNullOrEmpty(formSessionId))
                    {
                        sessionId = formSessionId;
                    }
                }
                catch { };
            }

            if (string.IsNullOrWhiteSpace(sessionId) && Request.Path.ToString() != "/")
            {
                var logger = request.HttpContext?.RequestServices?.GetService<ILogger<BaseController>>();
                logger?.LogWarning($"Unable to get session Id in  call {memberName} - {request.GetDisplayUrl()}");
            }

            return String.IsNullOrWhiteSpace(sessionId) ? null : sessionId;
        }

        protected string GetFormValue(string key)
        {
            if (FormData == null) return null;
            FormData.TryGetValue(key, out var stringValues);

            return stringValues.Count == 0 ? null : stringValues[0];
        }
    }
}