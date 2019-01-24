using System;
using System.Linq;
using Dfc.DiscoverSkillsAndCareers.FunctionApp.Helpers;
using Dfc.DiscoverSkillsAndCareers.Models;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.QuestionRouter
{
    public class BuildPageHtml
    {
        public string Html { get; private set; }

        public BuildPageHtml(string html, SessionHelper sessionHelper, Question question)
        {
            string errorMessage = sessionHelper.HasInputError ? "Please select an option above or this does not apply to continue" : string.Empty;
            int percentComplete = Convert.ToInt32(((sessionHelper.Session.RecordedAnswers.Count) / Convert.ToDecimal(sessionHelper.Session.MaxQuestions)) * 100);
            int displayPercentComplete = percentComplete - (percentComplete % 10);
            var nextRoute = GetNextRoute(sessionHelper.Session);
            var buttonText = sessionHelper.Session.IsComplete ? "Finish" : "Continue";

            // Replace placeholder text strings
            html = html.Replace("/assets/css/main", $"{sessionHelper.Config.StaticSiteDomain}/assets/css/main");
            html = html.Replace("[question_id]", question.QuestionId.ToString());
            html = html.Replace("[question_text]", question.Texts.Where(x => x.LanguageCode.ToLower() == sessionHelper.Session.LanguageCode.ToLower()).FirstOrDefault()?.Text);
            html = html.Replace("[question_number]", question.Order.ToString());
            html = html.Replace("[trait_code]", question.TraitCode);
            html = html.Replace("[form_route]", nextRoute);
            html = html.Replace("[session_id]", sessionHelper.Session.PrimaryKey);
            html = html.Replace("[button_text]", buttonText);
            html = html.Replace("[error_message]", errorMessage);
            html = html.Replace("[percentage]", displayPercentComplete.ToString());
            html = html.Replace("[percentage_left]", displayPercentComplete == 0 ? "" : displayPercentComplete.ToString());
            html = html.Replace("[code]", sessionHelper.Session.UserSessionId);
            Html = html;
        }

        public static string GetNextRoute(UserSession userSession)
        {
            if (userSession.IsComplete || userSession.RecordedAnswers.Count + 1 >= userSession.MaxQuestions)
            {
                return "/finish";
            }
            else if (userSession.CurrentQuestion + 1 <= userSession.MaxQuestions)
            {
                return $"/q/{userSession.CurrentQuestion + 1}";
            }
            else
            {
                // Goto last unaswered question
                int questionNumber = 1;
                for (int i = 1; i < userSession.MaxQuestions; i++)
                {
                    if (userSession.RecordedAnswers.Any(x => x.QuestionNumber == i.ToString()) == false)
                    {
                        questionNumber = i;
                        break;
                    }
                }
                return $"/q/{questionNumber}";
            }
        }
    }
}
