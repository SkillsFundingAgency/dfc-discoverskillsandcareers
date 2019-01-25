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
            int displayCounter = 0;
            bool displayAll = true;
            sessionHelper.Session.ResultData.JobFamilies.ForEach(jobFamily =>
            {
                if (displayCounter < 3 || displayAll)
                {
                    jobFamilyHtml += $"<li class=\"app-result\"><h3 class=\"govuk-heading-m app-result__header\">{jobFamily.JobFamilyName}</h3><div class=\"app-result__body\"><p>{jobFamily.JobFamilyText}</p><a href=\"{jobFamily.Url}\" role=\"button\" draggable=\"false\" class=\"govuk-button govuk-button--start app-button--alt\">See job category</a></div></li>";
                    displayCounter++;
                }
            });

            var traitHtml = "";
            displayCounter = 0;
            int maxTraitDisplay = 3;
            bool isLocal = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));
            // Top 3 traits get displayed; unless 3 and 4 draw then it's 4
            if (sessionHelper.Session.ResultData.Traits.Count > 3)
            {
                if (sessionHelper.Session.ResultData.Traits[2].TotalScore == sessionHelper.Session.ResultData.Traits[3].TotalScore)
                {
                    maxTraitDisplay += 1;
                }
            }
            // List the traits to display
            sessionHelper.Session.ResultData.Traits.ForEach(trait =>
            {
                if (isLocal)
                {
                    traitHtml += $"<p>{trait.TraitName} {trait.TotalScore} {trait.TraitText}</p>";
                }
                else if (displayCounter < maxTraitDisplay)
                {
                    traitHtml += $"<p>{trait.TraitText}</p>";
                    displayCounter++;
                }
            });

            string answersHtml = string.Empty;
            sessionHelper.Session.RecordedAnswers.ForEach(answer =>
            {
                answersHtml += $"<p>{answer.QuestionId} {answer.SelectedOption}</p>";
            });

            // Replace placeholder text strings
            html = html.Replace("/assets/css/main", $"{sessionHelper.Config.StaticSiteDomain}/assets/css/main");
            html = html.Replace("/assets/js/site", $"{sessionHelper.Config.StaticSiteDomain}/assets/js/site");
            html = html.Replace("[session_id]", sessionHelper.Session.PrimaryKey);
            html = html.Replace("[job_families_li_html]", jobFamilyHtml);
            html = html.Replace("[traits_li_html]", traitHtml);
            html = html.Replace("[static_site_domain]", sessionHelper.Config.StaticSiteDomain);
            html = html.Replace("[job_family_count]", sessionHelper.Session.ResultData.JobFamilies.Count.ToString());
            html = html.Replace("[job_family_more_count]", (sessionHelper.Session.ResultData.JobFamilies.Count - 3).ToString());
            Html = html;
        }
    }
}
