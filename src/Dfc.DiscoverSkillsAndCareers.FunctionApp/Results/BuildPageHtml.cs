using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.Results
{
    public class BuildPageHtml
    {
        public string Html { get; private set; }

        public BuildPageHtml(SessionHelper sessionHelper)
        {
            var resultHtml = "<ul>";
            sessionHelper.Session.ResultData.Traits.ForEach(trait =>
            {
                resultHtml += $"<li>{trait.TraitName} {trait.TotalScore} {trait.TraitText}</li>";
            });
            resultHtml += "</ul>";
            string answersHtml = string.Empty;
            sessionHelper.Session.RecordedAnswers.ForEach(answer =>
            {
                answersHtml += $"<p>{answer.QuestionId} {answer.SelectedOption}</p>";
            });
            string html = $"<body><form><input type='hidden' name='sessionId' value='[session_id]'/></form>Results for {sessionHelper.Session.PrimaryKey} <br /> {resultHtml} <br /> {answersHtml}</body>";

            html = html.Replace("[session_id]", sessionHelper.Session.PrimaryKey);
            Html = html;
        }
    }
}
