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
            var html = BlobStorageHelper.GetBlob("Results.html").Result;
          
            var jobFamilyHtml = "";
            sessionHelper.Session.ResultData.JobFamilies.ForEach(jobFamily =>
            {
                jobFamilyHtml += $"<li class=\"app-result\"><h3 class=\"govuk-heading-m app-result__header\">{jobFamily.JobFamilyName}</h3><div class=\"app-result__body\"><p>{jobFamily.JobFamilyText}</p><a href=\"{jobFamily.Url}\" role=\"button\" draggable=\"false\" class=\"govuk-button govuk-button--start app-button--alt\">See job category</a></div></li>";
            });

            var traitHtml = "";
            sessionHelper.Session.ResultData.Traits.ForEach(trait =>
            {
                traitHtml += $"<li>{trait.TraitName} {trait.TraitText}</li>";
            });

            string answersHtml = string.Empty;
            sessionHelper.Session.RecordedAnswers.ForEach(answer =>
            {
                answersHtml += $"<p>{answer.QuestionId} {answer.SelectedOption}</p>";
            });

            // Replace placeholder text strings
            html = html.Replace("[session_id]", sessionHelper.Session.PrimaryKey);
            html = html.Replace("[job_families_li_html]", jobFamilyHtml);
            html = html.Replace("[traits_li_html]", traitHtml);
            Html = html;
        }
    }
}
