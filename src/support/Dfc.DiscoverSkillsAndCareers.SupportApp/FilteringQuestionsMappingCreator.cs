using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using CommandLine;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp
{

    public class SiteFinityJobProfile
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string WhatyouWillDo { get; set; }

        public string WYDDayToDayTasks { get; set; }
        public string Skills { get; set; }
        public string Overview { get; set; }

        public string WorkingHoursPatternsAndEnvironment { get; set; }

        public Guid[] JobProfileCategories { get; set; }
    }

    public class JobProfile
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid[] JobProfileCategories { get; set; }
        public string Mapping { get; set; }
    }

    public class JobProfileCategory
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
    }


    
    public class FilteringQuestionsMappingCreator
    {
        [Verb("create-filtering-questions-mapping", HelpText = "Validates the DYSAC content and relationships in Sitefinity.")]
        public class Options : AppSettings
        {
            public class JobProfileMappingSheet
            {
                public string Id = "1OrM2UH8eVClFiXH6L3_Geqs-q83yigHKHdBOhOftQuU";
                public string FilteringQuestionsRange = "Filtering Questions!A1:B";
            }

            
            [Option('o', "outputDir", Required = false, HelpText = "The output directory of any extracted data.")]
            public string OutputDirectory { get; set; }

            public string SiteFinityApiUrl => $"{SiteFinityApiUrlbase}/api/{SiteFinityApiWebService}";
            public double RemoveBottomPercentage { get; set; } = 0.0;

            public JobProfileMappingSheet JobProfileSheet = new JobProfileMappingSheet();


        }

        public static SuccessFailCode Execute(IServiceProvider services, Options opts)
        {
            var logger = services.GetService<ILogger<FilteringQuestionsMappingCreator>>();

            try
            {

                var configuration = services.GetService<IConfiguration>();
                configuration.GetSection("AppSettings").Bind(opts);
                var sitefinityService = services.GetService<ISiteFinityHttpService>();

                var jobProfileCategories = SiteFinityWorkflowRunner
                    .GetTaxonomyInstances(sitefinityService, opts.SiteFinityApiUrl, "Job Profile Category")
                    .GetAwaiter()
                    .GetResult()
                    .Value
                    .Select(c => c.ToObject<JobProfileCategory>()).ToDictionary(r => r.Id);

                var jobProfiles = SiteFinityWorkflowRunner.GetAll<SiteFinityJobProfile>(sitefinityService,
                        opts.SiteFinityApiUrl,
                        "jobprofiles?$select=Title,Overview,WhatYouWillDo,WYDDayToDayTasks,Skills,JobProfileCategories,WorkingHoursPatternsAndEnvironment&$orderby=Title")
                    .GetAwaiter()
                    .GetResult()
                    .Value;

                var jobProfileCategoriesLookup =
                    jobProfiles.ToDictionary(p => p.Title,
                        p => p.JobProfileCategories.Select(c => jobProfileCategories[c]).ToArray(),
                        StringComparer.InvariantCultureIgnoreCase);

                var jobProfileVocab = CreateJobProfileVocabulary(jobProfiles, opts);
                var questionVocab = ReadQuestionVocabulary(opts);


                var termCounts = CalculateGlobalTermCounts(jobProfileVocab);
                var categoryTermCounts = CalculateCategoryTermCounts(jobProfileVocab, jobProfileCategoriesLookup);

                File.WriteAllText(Path.Combine(opts.OutputDirectory, "job_profiles.json"),
                    JsonConvert.SerializeObject(jobProfiles, Formatting.Indented));
                File.WriteAllText(Path.Combine(opts.OutputDirectory, "job_profile_category_term_counts.json"),
                    JsonConvert.SerializeObject(categoryTermCounts, Formatting.Indented));
                File.WriteAllText(Path.Combine(opts.OutputDirectory, "job_profile_term_counts.json"),
                    JsonConvert.SerializeObject(termCounts, Formatting.Indented));
                File.WriteAllText(Path.Combine(opts.OutputDirectory, "job_profile_vocab.json"),
                    JsonConvert.SerializeObject(jobProfileVocab, Formatting.Indented));
                File.WriteAllText(Path.Combine(opts.OutputDirectory, "question_vocab.json"),
                    JsonConvert.SerializeObject(questionVocab, Formatting.Indented));

                GenerateProfileMappingMatrix(questionVocab, jobProfileVocab, jobProfileCategoriesLookup, opts);

                return SuccessFailCode.Succeed;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured loading cms content");
                return SuccessFailCode.Fail;
            }
        }

        private static Dictionary<string, Dictionary<string, int>> CalculateCategoryTermCounts(
            IDictionary<string, NLP.Vocabulary> jobProfileVocab,
            Dictionary<string, JobProfileCategory[]> jobProfileCategoriesLookup)
        {
            var categoryTermCounts = new Dictionary<string, Dictionary<string, int>>();

            foreach (var profile in jobProfileVocab)
            {
                var categories = jobProfileCategoriesLookup[profile.Key].Select(c => c.Title);

                foreach (var cat in categories)
                {
                    if (!categoryTermCounts.TryGetValue(cat, out var category))
                    {
                        category = new Dictionary<string, int>();
                        categoryTermCounts[cat] = category;
                    }

                    foreach (var term in profile.Value.Terms)
                    {
                        if (!category.TryGetValue(term.Text, out var count))
                        {
                            category[term.Text] = 1;
                        }

                        category[term.Text] = count + 1;
                    }
                }
            }

            return categoryTermCounts;
        }

        private static Dictionary<string, int> CalculateGlobalTermCounts(
            IDictionary<string, NLP.Vocabulary> jobProfileVocab)
        {
            var termCounts = new Dictionary<String, int>();

            foreach (var profile in jobProfileVocab)
            {
                foreach (var term in profile.Value.Terms)
                {
                    if (!termCounts.TryGetValue(term.Text, out var count))
                    {
                        termCounts[term.Text] = 1;
                        continue;
                    }

                    termCounts[term.Text] = count + 1;
                }
            }

            return termCounts;
        }

        private static IDictionary<string, NLP.Vocabulary> CreateJobProfileVocabulary(List<SiteFinityJobProfile> data,
            Options opts)
        {
            var jobProfiles =
                data
                    .Select(p =>
                    {
                        var text = String.Join(Environment.NewLine, p.Title, p.Overview, p.WhatyouWillDo, p.Skills,
                            p.WorkingHoursPatternsAndEnvironment);
                        return (p.Title, HttpUtility.HtmlDecode(text));
                    }).ToList();

            return NLP.Analyse(topTermPercentage: opts.RemoveBottomPercentage, texts: jobProfiles.ToArray());
        }

        private static NLP.SentenceDocument[] CreateJobProfileSentenceDocument(ISiteFinityHttpService service,
            Options opts)
        {
            return
                SiteFinityWorkflowRunner.GetAll<SiteFinityJobProfile>(service, opts.SiteFinityApiUrl, "jobprofiles")
                    .GetAwaiter()
                    .GetResult()
                    .Value
                    .Select(p =>
                    {
                        var text = $"{p.WhatyouWillDo}";
                        return NLP.GetSentences(p.Title, HttpUtility.HtmlDecode(text));
                    }).ToArray();
        }

        private static NLP.SentenceDocument[] CreateQuestionSentenceDocuments(Options opts)
        {
            return GoogleSheetsReader.ReadRange(opts.JobProfileSheet.Id,
                opts.JobProfileSheet.FilteringQuestionsRange,
                row =>
                {
                    if (row.Count < 2)
                        return null;

                    var question = (string) row[0];
                    var vocab = (string) row[1];

                    return NLP.GetSentences(question, question);
                }).ToArray();
        }

        private static IDictionary<string, NLP.Vocabulary> ReadQuestionVocabulary(Options opts)
        {
            var qs = GoogleSheetsReader.ReadRange(opts.JobProfileSheet.Id,
                opts.JobProfileSheet.FilteringQuestionsRange,
                row =>
                {
                    var question = (string) row[0];
                    var vocab = "";

                    if (row.Count() >= 2)
                        vocab = (string) row[1];

                    return Tuple.Create(question,
                        $"{question}{Environment.NewLine}{vocab.Replace(",", Environment.NewLine)}");
                }).Select(s => (s.Item1, s.Item2)).Distinct();

            return NLP.Analyse(null, topTermPercentage: 0, texts: qs.ToArray());
        }


        private static void GenerateProfileMappingMatrix(IDictionary<string, NLP.Vocabulary> questionVocabulary,
            IDictionary<string, NLP.Vocabulary> jobProfileVocabulary,
            IDictionary<string, JobProfileCategory[]> jobProfileCategoriesLookup, Options opts)
        {
            using (var fs = File.Open(Path.Combine(opts.OutputDirectory, "functional_competency_mapping.csv"),
                FileMode.Truncate, FileAccess.ReadWrite))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.WriteLine($"JobProfile,JobProfileCategories,{String.Join(",", questionVocabulary.Keys)}");

                    foreach (var jobProfileVocab in jobProfileVocabulary)
                    {
                        var values = new List<string>
                        {
                            $"\"{jobProfileVocab.Key}\"",
                            $"\"{String.Join("|", jobProfileCategoriesLookup[jobProfileVocab.Key].Select(c => c.Title))}\""
                        };
                        foreach (var questionVocab in questionVocabulary)
                        {
                            if (questionVocab.Value.IsMatch(jobProfileVocab.Value))
                            {
                                values.Add("y");
                            }
                            else
                            {
                                values.Add("n");
                            }
                        }

                        sw.WriteLine(String.Join(",", values));
                    }

                }
            }

        }
    }
}