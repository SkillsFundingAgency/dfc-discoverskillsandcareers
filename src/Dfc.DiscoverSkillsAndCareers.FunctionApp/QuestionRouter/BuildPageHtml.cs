using System;
using System.Linq;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.QuestionRouter
{
    public class BuildPageHtml
    {
        public string Html { get; private set; }

        public BuildPageHtml(SessionHelper sessionHelper, Question question)
        {
            var html = BlobStorageHelper.GetBlob("questions.html").Result;

            string errorMessage = sessionHelper.HasInputError ? "Please select an option above to continue" : string.Empty;
            int percentComplete = Convert.ToInt32(((sessionHelper.Session.CurrentQuestion - 1) / Convert.ToDecimal(sessionHelper.Session.MaxQuestions)) * 100);
            var nextRoute = GetNextRoute(sessionHelper.Session);
            var buttonText = sessionHelper.Session.IsComplete ? "Finish" : "Continue";

            // Replace placeholder text strings
            html = html.Replace("[question_id]", question.QuestionId.ToString());
            html = html.Replace("[question_text]", question.Texts.Where(x => x.LanguageCode == sessionHelper.Session.LanguageCode).FirstOrDefault()?.Text);
            html = html.Replace("[question_number]", question.Order.ToString());
            html = html.Replace("[trait_code]", question.TraitCode);
            html = html.Replace("[form_route]", nextRoute);
            html = html.Replace("[session_id]", sessionHelper.Session.PrimaryKey);
            html = html.Replace("[button_text]", buttonText);
            html = html.Replace("[error_message]", errorMessage);
            html = html.Replace("[percentage]", percentComplete.ToString());
            Html = html;
        }

        public static string GetNextRoute(UserSession userSession) => userSession.IsComplete ? "/results" : $"/q/{userSession.CurrentQuestion + 1}";
    }
}
