using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public abstract class BaseController : Controller
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

        protected async Task<string> TryGetSessionId(HttpRequest request)
        {
            string sessionId = string.Empty;

            if (request.Cookies.TryGetValue(".dysac-session", out var cookieSessionId))
            {
                sessionId = _dataProtector.Unprotect(cookieSessionId);
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
            
            return String.IsNullOrWhiteSpace(sessionId) ? null : sessionId;
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
