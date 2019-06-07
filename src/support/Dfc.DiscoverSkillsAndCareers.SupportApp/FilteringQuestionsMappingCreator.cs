using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Web;
using CommandLine;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.SupportApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp
{

    public class ONetSkill : IEquatable<ONetSkill>
    {
        public string Title { get; set; }
        
        public string ONetAttributeType { get; set; }
        
        public int Rank { get; set; }
        
        public double ONetRank { get; set; }

        public string Skill => Title?.Split(new[] {'-'}, StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(1);

        public bool Equals(ONetSkill other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Skill, other.Skill);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ONetSkill) obj);
        }

        public override int GetHashCode()
        {
            return (Skill != null ? Skill.GetHashCode() : 0);
        }
    }

    public class SiteFinityJobProfile
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string WhatyouWillDo { get; set; }

        public string WYDDayToDayTasks { get; set; }
       // public string Skills { get; set; }
        public string Overview { get; set; }

        public string WorkingHoursPatternsAndEnvironment { get; set; }

        public Guid[] JobProfileCategories { get; set; }
        
        public ONetSkill[] RelatedSkills { get; set; }

        public IEnumerable<ONetSkill> Skills =>
            RelatedSkills.Where(o =>
                o.Skill != "Critical Thinking"
                && o.Skill != "Active Listening"
                && o.Skill != "Reading Comprehension"
                && o.Skill != "Idea Generation and Reasoning Abilities"
                && o.Skill != "Attentiveness"
                && o.Skill != "Attention to Detail"
                && o.ONetAttributeType != "Combination");
    }
    
    public class JobProfileCategory
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
    }
    
    public class Attribute
    {
        public string ONetAttribute { get; set; }
        public string ONetAttributeType { get; set; }
        public double CompositeRank { get; set; }
        public double ONetRank { get; set; }
        public double NcsRank { get; set; }
        public int TotalProfilesWithSkill { get; set; }
        public double PercentageProfileWithSkill { get; set; }
        public HashSet<string> ProfilesWithoutSkill { get; set; }
        
    }
        
    public class JobCategoryAttributes
    {
        public int JobCategoryProfileCount { get; set; }
            
        public Attribute[] Attributes { get; set; }
    }


    
    public class FilteringQuestionsMappingCreator
    {
        [Verb("create-filtering-questions-mapping", HelpText = "Creates the filtering question mappings for DYSAC.")]
        public class Options : AppSettings
        {
            public class JobProfileMappingSheet
            {
                public string Id = "1OrM2UH8eVClFiXH6L3_Geqs-q83yigHKHdBOhOftQuU";
                public string FilteringQuestionsRange = "Filtering Questions!A1:B";
            }

            public class ONetQuestionSheet
            {
                public string Id = "1OrM2UH8eVClFiXH6L3_Geqs-q83yigHKHdBOhOftQuU";
                public string OnetQuestionsRange = "Filtering Questions!A1:B";
            }

            
            [Option('o', "outputDir", Required = false, HelpText = "The output directory of any extracted data.")]
            public string OutputDirectory { get; set; }

            public string SiteFinityApiUrl => $"{SiteFinityApiUrlbase}/api/{SiteFinityApiWebService}";
            public double RemoveBottomPercentage { get; set; } = 0.0;

            public JobProfileMappingSheet JobProfileSheet = new JobProfileMappingSheet();
            
            public ONetQuestionSheet ONetSheet = new ONetQuestionSheet();


        }

        public static SuccessFailCode Execute(IServiceProvider services, Options opts)
        {
            var logger = services.GetService<ILogger<FilteringQuestionsMappingCreator>>();

            try
            {

                var configuration = services.GetService<IConfiguration>();
                configuration.GetSection("AppSettings").Bind(opts);
                var sitefinityService = services.GetService<ISiteFinityHttpService>();

//                var jobProfileCategories = SiteFinityWorkflowRunner
//                    .GetTaxonomyInstances(sitefinityService, opts.SiteFinityApiUrl, "Job Profile Category")
//                    .GetAwaiter()
//                    .GetResult()
//                    .Value
//                    .Select(c => c.ToObject<JobProfileCategory>()).ToDictionary(r => r.Id);
                
                //File.WriteAllText(Path.Combine(opts.OutputDirectory, "job_categories.json"), JsonConvert.SerializeObject(jobProfileCategories, Formatting.Indented));

//                var jobProfiles = SiteFinityWorkflowRunner.GetAll<SiteFinityJobProfile>(sitefinityService,
//                        opts.SiteFinityApiUrl,
//                        "jobprofiles?$select=Title,Overview,WhatYouWillDo,WYDDayToDayTasks,Skills,JobProfileCategories,WorkingHoursPatternsAndEnvironment&$expand=RelatedSkills&$orderby=Title")
//                    .GetAwaiter()
//                    .GetResult()
//                    .Value;

//                
//                File.WriteAllText(Path.Combine(opts.OutputDirectory, "job_profiles.json"),
//                    JsonConvert.SerializeObject(jobProfiles, Formatting.Indented));
//                

                var jobProfileCategories =
                    JsonConvert.DeserializeObject<Dictionary<Guid, JobProfileCategory>>(
                        File.ReadAllText(Path.Combine(opts.OutputDirectory, "job_categories.json")));

                var jobProfiles =
                    JsonConvert.DeserializeObject<List<SiteFinityJobProfile>>(
                        File.ReadAllText(Path.Combine(opts.OutputDirectory, "job_profiles.json")));
                
                var jobProfileCategoriesLookup =
                    jobProfiles.ToDictionary(p => p.Title,
                        p => p.JobProfileCategories.Select(c => jobProfileCategories[c]).ToArray(),
                        StringComparer.InvariantCultureIgnoreCase);

//                var jobProfileVocab = CreateJobProfileVocabulary(jobProfiles, opts);
//                
//                File.WriteAllText(Path.Combine(opts.OutputDirectory, "job_profile_vocab.json"),
//                    JsonConvert.SerializeObject(jobProfileVocab, Formatting.Indented));
//                
//                logger.LogInformation("Output: Job Profile Vocabulary");
//                
//                var questionVocab = ReadQuestionVocabulary(opts);
//                
//                File.WriteAllText(Path.Combine(opts.OutputDirectory, "question_vocab.json"),
//                    JsonConvert.SerializeObject(questionVocab, Formatting.Indented));
//                
//                logger.LogInformation("Output: Question Vocabulary");
//                
//                var termCounts = CalculateGlobalTermCounts(jobProfileVocab);
//                
//                File.WriteAllText(Path.Combine(opts.OutputDirectory, "job_profile_term_counts.json"),
//                    JsonConvert.SerializeObject(termCounts, Formatting.Indented));
//                
//                logger.LogInformation("Output: Job Profile Term Counts");
//                
//                var categoryTermCounts = CalculateCategoryTermCounts(jobProfileVocab, jobProfileCategoriesLookup);
//                
//                File.WriteAllText(Path.Combine(opts.OutputDirectory, "job_profile_category_term_counts.json"),
//                    JsonConvert.SerializeObject(categoryTermCounts, Formatting.Indented));
                
//                logger.LogInformation("Output: Job Profile -> Job Category term counts");


                var jobProfileSkills = CreateJobProfileSkillGrouping(jobProfiles);
                
                File.WriteAllText(Path.Combine(opts.OutputDirectory, $"job_profile_onet_complete.json"),
                    JsonConvert.SerializeObject(jobProfileSkills, Formatting.Indented));

                var mostCommon =
                    jobProfileSkills.Where(k => k.Value.ProfileCount / 852.0 >= 0.75)
                        .ToDictionary(r => r.Key, r => r.Value.ProfileCount);

                File.WriteAllText(Path.Combine(opts.OutputDirectory, $"job_profile_onet_in_75pc_of_profiles.json"),
                    JsonConvert.SerializeObject(mostCommon, Formatting.Indented));
                
                var jobCategorySkills = CreateJobCategorySkillGrouping(jobProfiles, jobProfileCategoriesLookup);

                File.WriteAllText(Path.Combine(opts.OutputDirectory, $"job_category_onet_complete.json"),
                    JsonConvert.SerializeObject(jobCategorySkills, Formatting.Indented));
                
                var candidateQuestions = jobCategorySkills.ToDictionary(r => r.Key, r => new JobCategoryAttributes
                {
                    JobCategoryProfileCount = r.Value.JobCategoryProfileCount,
                    Attributes = r.Value.Attributes.Take(4).ToArray()

                });
                
                File.WriteAllText(Path.Combine(opts.OutputDirectory, $"job_category_onet.json"),
                    JsonConvert.SerializeObject(candidateQuestions, Formatting.Indented));
                
                var onetAttributes =
                    jobCategorySkills.Values.SelectMany(v => v.Attributes.Select(r => (r.ONetAttribute, r.ONetAttributeType)))
                        .Select(r => new { ONetAttribute = r.Item1, ONetAttributeType = r.Item2 })
                        .Distinct();
                
                File.WriteAllText(Path.Combine(opts.OutputDirectory, $"active_onet_attributes.json"),
                    JsonConvert.SerializeObject(onetAttributes, Formatting.Indented));
                
                logger.LogInformation("Done");
                
                
//                GenerateProfileMappingMatrix(questionVocab, jobProfileVocab, jobProfileCategoriesLookup, opts);
//                
//                logger.LogInformation("Output: Job Profile Mapping Matrix");
                
                return SuccessFailCode.Succeed;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured loading cms content");
                return SuccessFailCode.Fail;
            }
        }
        
        private static IDictionary<string,JobCategoryAttributes> CreateJobCategorySkillGrouping(List<SiteFinityJobProfile> jobProfiles, 
            IDictionary<string,JobProfileCategory[]> categoryLookup)
        {
            var categorySkillGrouping = 
                jobProfiles
                    .SelectMany(p =>
                        categoryLookup[p.Title].Select(c => new
                        {
                            Category = c.Title,
                            Profile = p
                        }))
                    .GroupBy(x => x.Category)
                    .Select(x =>
                    {
                        var totalProfileCount = (double)x.Count();
                        return new
                        {
                            Category = x.Key,
                            Skills = new JobCategoryAttributes
                            {
                                JobCategoryProfileCount = (int)totalProfileCount,
                                Attributes = x.SelectMany(y => y.Profile.Skills.Select(s => new {Profile = y.Profile, Skill = s}))
                                    .GroupBy(s => s.Skill).Select(s =>
                                    {
                                        var onetRank = s.Average(r => r.Skill.ONetRank);
                                        var ncsRank = s.Average(r => 20 - r.Skill.Rank);
                                        var profileCount = (double) s.Count();
                                        var profilePercentage =
                                            (1.0 - ((totalProfileCount - profileCount) / totalProfileCount)) * 100.0;
                                        return new Attribute
                                        {
                                            ONetAttribute = s.Key.Skill,
                                            ONetAttributeType = s.Key.ONetAttributeType,
                                            CompositeRank =
                                                s.Average(r => ncsRank + (profilePercentage / 5.0)),
                                            ONetRank = onetRank,
                                            NcsRank = ncsRank,
                                            TotalProfilesWithSkill = (int) profileCount,
                                            PercentageProfileWithSkill = profilePercentage,
                                            ProfilesWithoutSkill = new HashSet<string>(
                                                x.Where(p => !p.Profile.Skills.Any(ps => ps.Skill == s.Key.Skill))
                                                    .Select(p => p.Profile.Title))
                                        };

                                    })
                                    .Where(a => a.ProfilesWithoutSkill.Count > 0)
                                    .OrderByDescending(a => a.CompositeRank)
                                    .ToArray()
                            }
                        };
                    })
                    .ToDictionary(x => x.Category, x => x.Skills);

            return categorySkillGrouping;

        }

        public class SkillProfileStats
        {
            public int ProfileCount => Profiles.Count; 
            public IList<string> Profiles { get; set; } = new List<string>();
        }
        
        private static IDictionary<string,SkillProfileStats> CreateJobProfileSkillGrouping(IList<SiteFinityJobProfile> jobProfiles)
        {
            var skills = jobProfiles.SelectMany(p => p.RelatedSkills.Select(s => s.Skill)).Distinct();
            var result = new Dictionary<string,SkillProfileStats>();
            foreach (var skill in skills)
            {
                if (!result.TryGetValue(skill, out var profiles))
                {
                    profiles = new SkillProfileStats();
                    result[skill] = profiles;
                }

                foreach (var jobProfile in jobProfiles)
                {
                    if (jobProfile.RelatedSkills.Any(s =>
                        skill.Equals(s.Skill, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        profiles.Profiles.Add(jobProfile.Title);
                    }
                }
            }

            return result;
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