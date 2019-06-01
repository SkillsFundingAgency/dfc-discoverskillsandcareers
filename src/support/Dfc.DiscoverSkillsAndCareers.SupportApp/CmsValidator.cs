using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using CommandLine;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Syn.WordNet;

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
    

    public class FilteringQuestion
    {
        private IDictionary<Guid, JobProfile> _jobProfilesLookup = null;
        private JobProfile[] _profiles = { };
        
        public Guid Id { get; set; }
        
        public string Title { get; set; }
        
        public string QuestionText { get; set; }
        
        [JsonProperty("IsYes")]
        public bool ExcludeIfYes { get; set; }
        
        public Guid[] JobProfileCategories { get; set; }

        [JsonProperty("ExcludedJobProfiles")]
        public JobProfile[] JobProfiles
        {
            get => _profiles;
            set
            {
                _profiles = value;
                _jobProfilesLookup = _profiles.ToDictionary(x => x.Id);
            }
        }
        
        

        public bool ExcludesJobProfile(JobProfile profile)
        {
            if (_jobProfilesLookup.TryGetValue(profile.Id, out var p))
                return p.Mapping == "n";

            return false;
        }

        public bool HasJobProfile(JobProfile profile)
        {
            return _jobProfilesLookup.ContainsKey(profile.Id);
        }
    }
    
    
    public class FunctionalCompetency : IEquatable<FunctionalCompetency>
    {
        public string JobCategory { get; set; }
        public string Competency { get; set; }
        public string QuestionText { get; set; }

        public bool Equals(FunctionalCompetency other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(JobCategory, other.JobCategory) && string.Equals(Competency, other.Competency) && string.Equals(QuestionText, other.QuestionText);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FunctionalCompetency) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (JobCategory != null ? JobCategory.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Competency != null ? Competency.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (QuestionText != null ? QuestionText.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    

    public class CmsValidator
    {
        [Verb("validate-cms", HelpText = "Validates the DYSAC content and relationships in Sitefinity.")]
        public class Options : AppSettings
        {
            public class JobProfileMappingSheet
            {
                public string Id = "1OrM2UH8eVClFiXH6L3_Geqs-q83yigHKHdBOhOftQuU";

                public string JobProfileRange = "Job Profiles!A1:M";
                public string FilteringQuestionsRange = "Filtering Questions!A1:B";
                
            }

            public class FunctionalCompetencyMappingSheet
            {
                public string Id = "17mcCFqWRF7y1xUdjPhtEmPLgZMdp01MgwqYmGN3Q0Qk";
                public string JobCategoryRange = "Functional Competency Mapping!A";
                public string JobProfileRange = "Job Profiles!A";
                public string FunctionalComptencyRange = "Functional Competency Mapping!A1:AA";
                
                public IDictionary<string, string> JobCategorySheetRanges = new Dictionary<string, string>
                {
                    {"Administration", "A1:D60"},
                    {"Animal Care", "A1:D32"},
                    {"Beauty and Wellbeing", "A1:C29"},
                    {"Business and Finance", "A1:C40"},
                    {"Computing, Technology and Digital", "A1:C42"},
                    {"Construction and trades", "A1:C33"},
                    {"Creative and media", "A1:D119"},
                    {"Delivery and storage", "A1:C21"},
                    {"Emergency and Uniform Services", "A1:E38"},
                    {"Engineering and Maintenance", "A1:C110"},
                    {"Environment and Land", "A1:D56"},
                    {"Government Services", "A1:D44"},
                    {"Healthcare", "A1:E102"},
                    {"Home Services", "A1:B35"},
                    {"Hospitality and food", "A1:C23"},
                    {"Law and legal", "A1:C32"},
                    {"Managerial", "A1:C84"},
                    {"Manufacturing", "A1:C62"},
                    {"Retail and Sales", "A1:D67"},
                    {"Science and Research", "A1:D69"},
                    {"Social Care", "A1:C57"},
                    {"Sports and Leisure", "A1:C45"},
                    {"Teaching and Education", "A1:C51"},
                    {"Transport", "A1:D39"},
                    {"Travel and Tourism", "A1:C23"}
                };
            }
            
            [Option('o', "outputDir", Required = false, HelpText = "The output directory of any extracted data.")]
            public string OutputDirectory { get; set; }
            
            public string SiteFinityApiUrl => $"{SiteFinityApiUrlbase}/api/{SiteFinityApiWebService}";
            public double RemoveBottomPercentage { get; set; } = 0.0;

            public FunctionalCompetencyMappingSheet FunctionalCompetencySheet = new FunctionalCompetencyMappingSheet();
            
            public JobProfileMappingSheet JobProfileSheet = new JobProfileMappingSheet();
            
            
        }
        
        public static SuccessFailCode Execute(IServiceProvider services, Options opts)
        {
            var logger = services.GetService<ILogger<CmsValidator>>();
            
            try
            {
              
                var configuration = services.GetService<IConfiguration>();
                configuration.GetSection("AppSettings").Bind(opts);
                var sitefinityService = services.GetService<ISiteFinityHttpService>();
                
                var jobProfileVocab = CreateJobProfileVocabulary(sitefinityService, opts);
                var questionVocab = ReadQuestionVocabulary(opts);
                
                File.WriteAllText(Path.Combine(opts.OutputDirectory, "job_profile_vocab.json"), JsonConvert.SerializeObject(jobProfileVocab, Formatting.Indented));
                File.WriteAllText(Path.Combine(opts.OutputDirectory, "question_vocab.json"), JsonConvert.SerializeObject(questionVocab, Formatting.Indented));
                
                GenerateProfileMappingMatrix(questionVocab, jobProfileVocab, opts);
                
                return SuccessFailCode.Succeed;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured loading cms content");
                return SuccessFailCode.Fail;
            }
        }
        
        private static IDictionary<string, NLP.Vocabulary> CreateJobProfileVocabulary(ISiteFinityHttpService service, Options opts)
        {
            string TransformNull(string s)
            {
                return s == "(null)" ? "" : s;
            }

            var jobProfiles =
                SiteFinityWorkflowRunner.GetAll<SiteFinityJobProfile>(service, opts.SiteFinityApiUrl, "jobprofiles")
                    .GetAwaiter()
                    .GetResult()
                    .Value
                    .Select(p =>
                    {
                        var text = $"{p.Title} {p.Overview} {p.WhatyouWillDo} {p.Skills}";
                        return (p.Title, HttpUtility.HtmlDecode(text));
                    }).ToList();

            return NLP.Analyse(topTermPercentage: opts.RemoveBottomPercentage, texts: jobProfiles.ToArray());
        }
        
        private static IDictionary<string, NLP.Vocabulary> ReadQuestionVocabulary(Options opts)
        {
            var qs = GoogleSheetsReader.ReadRange(opts.JobProfileSheet.Id,
                opts.JobProfileSheet.FilteringQuestionsRange,
                row =>
                {
                    if (row.Count < 2)
                        return null;
        
                    var question = (string) row[0];
                    var vocab = (string) row[1];
                    
                    return Tuple.Create(question, $"{question} {vocab.Replace(",", " ")}");
                }).Select(s => (s.Item1, s.Item2)).Distinct();

            return NLP.Analyse(null, topTermPercentage: 0, texts: qs.ToArray());
        }


        private static void GenerateProfileMappingMatrix(IDictionary<string,NLP.Vocabulary> questionVocabulary, IDictionary<string,NLP.Vocabulary> jobProfileVocabulary, Options opts)
        {
            using (var fs = File.Open(Path.Combine(opts.OutputDirectory, "functional_competency_mapping.csv"),
                FileMode.Truncate, FileAccess.ReadWrite))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.WriteLine($",{String.Join(",",  questionVocabulary.Keys)}");

                    foreach (var jobProfileVocab in jobProfileVocabulary)
                    {
                        var values = new List<string> { $"\"{jobProfileVocab.Key}\"" };
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

        //        private static IDictionary<string, IDictionary<string, FunctionalCompetency>> ReadFunctionalCompetencyMapping(string spreadsheetId, string range)
//        {
//            var data = GoogleSheetsReader.ReadRangeWithHeaders(spreadsheetId, range, (headers, row) =>
//            {
//                var category = row[0];
//                IList<FunctionalCompetency> fcs = new List<FunctionalCompetency>();
//                var index = 1;
//                foreach (var cell in row.Skip(1))
//                {
//                    if (cell is string)
//                    {
//                        var question = cell as String;
//
//                        if (!String.IsNullOrWhiteSpace(question))
//                        {
//                            fcs.Add(new FunctionalCompetency
//                            {
//                                Competency = headers[index],
//                                JobCategory = category.ToString(),
//                                QuestionText = cell.ToString()
//                            });
//                        }
//                    }
//
//                    index++;
//                }
//
//                return fcs;
//            });
//
//            return data
//                .SelectMany(f => f.GroupBy(r => r.JobCategory.Trim().Replace("&", "and")))
//                .ToDictionary(f => f.Key, f => (IDictionary<string,FunctionalCompetency>)f.ToDictionary(x => x.Competency, StringComparer.InvariantCultureIgnoreCase), StringComparer.InvariantCultureIgnoreCase);
//        }
//        
//        private static IList<FilteringQuestion> ReadFilteringQuestions(
//            string spreadsheetId, 
//            JobProfileCategory category,
//            string range,
//            IDictionary<string, JobProfile> jobProfileLookup,
//            IDictionary<string, FunctionalCompetency> functionalCompetencyMapping)
//        {
//            var sheet = category.Title;
//            return GoogleSheetsReader.ReadRangeWithHeaders(spreadsheetId, $"'{sheet}'!{range}", (headers, row) =>
//            {
//                var jobProfile = (string)row[0];
//                var data = new List<(FunctionalCompetency, JobProfile)>();
//                var index = 1;
//                foreach (var cell in row.Skip(1))
//                {
//                    var competency = functionalCompetencyMapping[headers[index]];
//                    var profile = jobProfileLookup[jobProfile.ToString()];
//                    profile.Mapping = cell.ToString();
//                    
//                    data.Add((competency, profile));
//
//                    index++;
//                }
//
//                return data;
//            }).SelectMany(r => r)
//                .GroupBy(r => r.Item1).Select((r,i) =>
//                {
//                    return new FilteringQuestion
//                    {
//                        Title = $"{category.Title}-{i + 1}",
//                        QuestionText = r.Key.QuestionText,
//                        JobProfileCategories = new[] {category.Id},
//                        JobProfiles = r.Select(p => p.Item2).ToArray()
//                    };
//
//                }).ToList();
//        }
        

//        private static Dataset GetGoogleSheetDataset(Options opts)
//        {
//            var dataset = new Dataset
//            {
//                JobProfileCategories = GoogleSheetsReader.ReadRangeWithHeaders(opts.FunctionalCompetencySheet.Id, opts.FunctionalCompetencySheet.JobCategoryRange, (_, row) => new JobProfileCategory {Title = (string) row[0]}),
//                JobProfiles = GoogleSheetsReader.ReadRangeWithHeaders(opts.FunctionalCompetencySheet.Id, opts.FunctionalCompetencySheet.JobProfileRange, (_,row) => new JobProfile { Title = (string)row[0] })
//            };
//            
//            var functionalCompetencies = ReadFunctionalCompetencyMapping(opts.FunctionalCompetencySheet.Id, opts.FunctionalCompetencySheet.FunctionalComptencyRange);
//
//            var questions = new List<FilteringQuestion>();
//            foreach (var jobCategorySheet in opts.FunctionalCompetencySheet.JobCategorySheetRanges)
//            {
//                var category = dataset.JobCategoryNameLookup[jobCategorySheet.Key];
//                var competencies = functionalCompetencies[jobCategorySheet.Key];
//                var googleSheetsFilteringQuestions = ReadFilteringQuestions(opts.FunctionalCompetencySheet.Id, category,jobCategorySheet.Value, dataset.JobProfileNameLookup, competencies);
//                    
//                questions.AddRange(googleSheetsFilteringQuestions);
//            }
//
//            dataset.FilteringQuestions = questions;
//            
//            return dataset;
//        }
//        
//        private static Dataset GetSitefinityDataset(ISiteFinityHttpService siteFinityService, Options opts)
//        {
//            var jobProfileCategories = SiteFinityWorkflowRunner
//                .GetTaxonomyInstances(siteFinityService, opts.SiteFinityApiUrl, "Job Profile Category")
//                .GetAwaiter()
//                .GetResult()
//                .Value
//                .Select(c => c.ToObject<JobProfileCategory>()).ToArray();
//
//            var sitefinityFilteringQuestions = SiteFinityWorkflowRunner
//                .GetAll<FilteringQuestion>(siteFinityService, opts.SiteFinityApiUrl,
//                    "filteringquestions?$expand=ExcludedJobProfiles").GetAwaiter().GetResult();
//            
//            var jobProfiles = SiteFinityWorkflowRunner
//                .GetAll<JobProfile>(siteFinityService, opts.SiteFinityApiUrl,
//                    "jobprofiles?$select=Id,Title,JobProfileCategories").GetAwaiter().GetResult();
//
//            return new Dataset
//            {
//                FilteringQuestions = sitefinityFilteringQuestions.Value,
//                JobProfiles = jobProfiles.Value,
//                JobProfileCategories = jobProfileCategories
//            };
//        }
//
//
//        private static void RunValidation(ILogger logger, string outputDirectory, Dataset sitefinityDataset, Dataset googleSheetData)
//        {
//            RunValidateUnmappedJobProfiles(outputDirectory, sitefinityDataset, logger);
//            
//        }
//
//        private static void RunValidateUnmappedJobProfiles(string outputDirectory, Dataset sitefinityDataset, ILogger logger)
//        {
//            logger.LogInformation($"Finding unmapped job profiles.");
//            
//            var unmappedProfiles = sitefinityDataset.UnmappedExcludedProfiles.ToList();
//            
//            if (unmappedProfiles.Count > 0)
//            {
//                using (var fs = File.OpenWrite(Path.Combine(outputDirectory, "unmapped_job_profiles.csv")))
//                {
//                    using (var sw = new StreamWriter(fs))
//                    {
//                        sw.WriteLine("JobProfileId,Title,JobProfileCategories");
//                        foreach (var unmappedProfile in unmappedProfiles)
//                        {
//                            sw.WriteLine($"{unmappedProfile.Id},\"{unmappedProfile.Title}\",\"{String.Join("|",unmappedProfile.JobProfileCategories.Select(c => sitefinityDataset.JobCategoryIdLookup[c]))}\"");
//                        }
//                    }
//                }
//
//                logger.LogInformation($"Found {unmappedProfiles.Count} unmapped job profiles.");
//            }
//        }
    }

//    public class Dataset
//    {
//        public IEnumerable<FilteringQuestion> FilteringQuestions { get; set; }
//        public IEnumerable<JobProfile> JobProfiles { get; set; }
//        public IEnumerable<JobProfileCategory> JobProfileCategories { get; set; }
//
//        public IDictionary<string, JobProfile> JobProfileNameLookup => 
//            JobProfiles.ToDictionary(r => r.Title, StringComparer.InvariantCultureIgnoreCase);
//
//        public IDictionary<Guid, JobProfileCategory> JobCategoryIdLookup =>
//            JobProfileCategories.ToDictionary(r => r.Id);
//
//        public IDictionary<string, JobProfileCategory> JobCategoryNameLookup =>
//            JobProfileCategories.ToDictionary(r => r.Title, StringComparer.InvariantCultureIgnoreCase);
//
//        public IEnumerable<JobProfile> UnmappedExcludedProfiles => 
//            JobProfiles.Where(jobProfile => !FilteringQuestions.Any(f => f.ExcludesJobProfile(jobProfile)));
//    }
}