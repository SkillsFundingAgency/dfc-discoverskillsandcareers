using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp
{
    public class HttpHtmlWithSessionCookieResponse : HttpResponseMessage
    {
        public HttpHtmlWithSessionCookieResponse(HttpRequestMessage req, string html, string sessionId)
        {
            var sessionCookie = new CookieHeaderValue("ncs-session-id", sessionId);
            sessionCookie.Expires = DateTimeOffset.Now.AddDays(1);
            sessionCookie.Domain = req.RequestUri.Host;
            sessionCookie.Path = "/";
            this.Headers.AddCookies(new List<CookieHeaderValue>() { sessionCookie });
            this.StatusCode = HttpStatusCode.OK;
            this.Content = new StringContent(html);
            this.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
        }
    }
}
