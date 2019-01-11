using System;
using System.Linq;
using Dfc.DiscoverSkillsAndCareers.Models;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.QuestionRouter
{
    public class BuildPageHtml
    {
        public string Html { get; private set; }

        public BuildPageHtml(SessionHelper sessionHelper, Question question)
        {
            var nextRoute = GetNextRoute(sessionHelper.Session);
            var buttonText = sessionHelper.Session.IsComplete ? "Finish" : "Continue";

            // Read the question page html file and replace text strings
            var html = System.IO.File.ReadAllText("pages/QuestionPage.html");
            html = html.Replace("[question_id]", question.QuestionId.ToString());
            html = html.Replace("[question_text]", question.Texts.Where(x => x.LanguageCode == sessionHelper.Session.LanguageCode).FirstOrDefault()?.Text);
            html = html.Replace("[question_number]", question.Order.ToString());
            html = html.Replace("[form_route]", nextRoute);
            html = html.Replace("[session_id]", sessionHelper.Session.PrimaryKey);
            html = html.Replace("[button_text]", buttonText);
            Html = html;
        }

        public static string GetNextRoute(UserSession userSession) => userSession.IsComplete ? "/results" : $"/q/{userSession.CurrentQuestion + 1}";
    }
}
