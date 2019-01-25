using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.Helpers
{
    public static class HttpResponseHelpers
    {
        public static string GetHost(Uri uri) => $"{uri.Scheme}://{uri.Authority}";

        public static HttpResponseMessage RedirectToNewSession(HttpRequestMessage req)
        {
            var redirectResponse = req.CreateResponse(HttpStatusCode.Redirect);
            var host = GetHost(req.RequestUri);
            redirectResponse.Headers.Location = new Uri($"{host}/q/1");
            return redirectResponse;
        }

        public static HttpResponseMessage RedirectToQuestionNumber(HttpRequestMessage req, SessionHelper sessionHelper) => RedirectToQuestionNumber(req, sessionHelper.Session.CurrentQuestion, sessionHelper.Session.PrimaryKey);

        public static HttpResponseMessage RedirectToQuestionNumber(HttpRequestMessage req, int questionNumber, string sessionId)
        {
            var redirectResponse = req.CreateResponse(HttpStatusCode.Redirect);
            var host = GetHost(req.RequestUri);
            redirectResponse.Headers.Location = new Uri($"{host}/q/{questionNumber}");
            var sessionCookie = CreateSessionCookie(req.RequestUri.Host, sessionId);
            redirectResponse.Headers.AddCookies(new List<CookieHeaderValue>() { sessionCookie });
            return redirectResponse;
        }

        public static HttpResponseMessage OKHtmlWithCookie(HttpRequestMessage req, string html, string sessionId)
        {
            var okResponse = req.CreateResponse(HttpStatusCode.OK);
            if (sessionId != null)
            {
                var sessionCookie = CreateSessionCookie(req.RequestUri.Host, sessionId);
                okResponse.Headers.AddCookies(new List<CookieHeaderValue>() { sessionCookie });
            }
            okResponse.StatusCode = HttpStatusCode.OK;
            okResponse.Content = new StringContent(html);
            okResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return okResponse;
        }

        private static CookieHeaderValue CreateSessionCookie(string domain, string sessionId)
        {
            var sessionCookie = new CookieHeaderValue("ncs-session-id", sessionId);
            sessionCookie.Expires = DateTimeOffset.Now.AddDays(1);
            sessionCookie.Domain = domain;
            sessionCookie.Path = "/";
            return sessionCookie;
        }
    }
}
