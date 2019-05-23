using System;
using CommandLine;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp
{
    public class CmsValidator
    {
        [Verb("validate-cms", HelpText = "Validates the DYSAC content and relationships in Sitefinity.")]
        public class Options : AppSettings
        {
            [Option('f', "workflowFile", Required = true, HelpText = "The file containing the sitefinity workflow to be executed.")]
            public string WorkflowFile { get; set; }
            
            [Option('o', "outputDir", Required = false, HelpText = "The output directory of any extracted data.")]
            public string OutputDirectory { get; set; }
            
            public string SiteFinityApiUrl => $"{SiteFinityApiUrlbase}/api/{SiteFinityApiWebService}";

        }
        
        public static SuccessFailCode Execute(IServiceProvider services, Options opts)
        {
            var logger = services.GetService<ILogger<Cms>>();
            
            try
            {
                var configuration = services.GetService<IConfiguration>();
                configuration.GetSection("AppSettings").Bind(opts);
                
                var siteFinityService = services.GetService<ISiteFinityHttpService>();

                SiteFinityWorkflowRunner.RunWorkflowFromFile(siteFinityService, logger, opts.SiteFinityApiUrl, opts.WorkflowFile, opts.OutputDirectory).GetAwaiter().GetResult();
                
                return SuccessFailCode.Succeed;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured loading cms content");
                return SuccessFailCode.Fail;
            }
        }
    }
}