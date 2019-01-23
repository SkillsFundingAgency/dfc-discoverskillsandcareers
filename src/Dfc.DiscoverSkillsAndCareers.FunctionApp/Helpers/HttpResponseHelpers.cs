using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.Helpers
{
    public static class HttpResponseHelpers
    {
        public static HttpResponseMessage RedirectStartAtQuestionOne(HttpRequestMessage req)
        {
            var redirectResponse = req.CreateResponse(HttpStatusCode.Redirect);
            var uri = req.RequestUri;
            var host = uri.AbsoluteUri.Replace(uri.AbsolutePath, "");
            redirectResponse.Headers.Location = new Uri($"{host}/q/1");
            return redirectResponse;
        }

        public static HttpResponseMessage RedirectToQuestionNumber(HttpRequestMessage req, SessionHelper sessionHelper) => RedirectToQuestionNumber(req, sessionHelper.Session.CurrentQuestion, sessionHelper.Session.PrimaryKey);

        public static HttpResponseMessage RedirectToQuestionNumber(HttpRequestMessage req, int questionNumber, string sessionId)
        {
            var redirectResponse = req.CreateResponse(HttpStatusCode.Redirect);
            var uri = req.RequestUri;
            var host = uri.AbsoluteUri.Replace(uri.AbsolutePath, "");
            redirectResponse.Headers.Location = new System.Uri($"{host}/q/{questionNumber}");
            var sessionCookie = new CookieHeaderValue("ncs-session-id", sessionId);
            sessionCookie.Expires = DateTimeOffset.Now.AddDays(1);
            sessionCookie.Domain = req.RequestUri.Host;
            sessionCookie.Path = "/";
            redirectResponse.Headers.AddCookies(new List<CookieHeaderValue>() { sessionCookie });
            return redirectResponse;
        }

        public static HttpResponseMessage RedirectToNewSession(HttpRequestMessage req)
        {
            var redirectResponse = req.CreateResponse(HttpStatusCode.Redirect);
            var uri = req.RequestUri;
            var host = uri.AbsoluteUri.Replace(uri.AbsolutePath, "");
            redirectResponse.Headers.Location = new System.Uri($"{host}/q/1");
            return redirectResponse;
        }
    }
}
