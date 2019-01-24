using Dfc.DiscoverSkillsAndCareers.FunctionApp.Helpers;
using System;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.Results
{
    public class BuildPageHtml
    {
        public string Html { get; private set; }

        public BuildPageHtml(string html, SessionHelper sessionHelper)
        {
            var jobFamilyHtml = "";
            sessionHelper.Session.ResultData.JobFamilies.ForEach(jobFamily =>
            {
                jobFamilyHtml += $"<li class=\"app-result\"><h3 class=\"govuk-heading-m app-result__header\">{jobFamily.JobFamilyName}</h3><div class=\"app-result__body\"><p>{jobFamily.JobFamilyText}</p><a href=\"{jobFamily.Url}\" role=\"button\" draggable=\"false\" class=\"govuk-button govuk-button--start app-button--alt\">See job category</a></div></li>";
            });

            var traitHtml = "";
            bool isLocal = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));
            sessionHelper.Session.ResultData.Traits.ForEach(trait =>
            {
                if (isLocal)
                {
                    traitHtml += $"<li>{trait.TraitName} {trait.TotalScore} {trait.TraitText}</li>";
                }
                else
                {
                    traitHtml += $"<li>{trait.TraitText}</li>";
                }
            });

            string answersHtml = string.Empty;
            sessionHelper.Session.RecordedAnswers.ForEach(answer =>
            {
                answersHtml += $"<p>{answer.QuestionId} {answer.SelectedOption}</p>";
            });

            // Replace placeholder text strings
            html = html.Replace("/assets/css/main", $"{sessionHelper.Config.StaticSiteDomain}/assets/css/main");
            html = html.Replace("[session_id]", sessionHelper.Session.PrimaryKey);
            html = html.Replace("[job_families_li_html]", jobFamilyHtml);
            html = html.Replace("[traits_li_html]", traitHtml);
            Html = html;
        }
    }
}
