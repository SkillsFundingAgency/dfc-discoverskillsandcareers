using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models.Extensions;
using Task = System.Threading.Tasks.Task;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services
{
    public class AssessmentCalculationService : IAssessmentCalculationService
    {
        private readonly IJobCategoryRepository _jobCategoryRepository;
        private readonly IShortTraitRepository _traitRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuestionSetRepository _questionSetRepository;

        private static readonly Dictionary<AnswerOption, int> AnswerOptions = new Dictionary<AnswerOption, int>()
        {
            { AnswerOption.StronglyDisagree, -2 },
            { AnswerOption.Disagree, -1 },
            { AnswerOption.Neutral, 0 },
            { AnswerOption.Agree, 1 },
            { AnswerOption.StronglyAgree, 2 },
        };

        public AssessmentCalculationService(
            IJobCategoryRepository jobCategoryRepository, 
            IShortTraitRepository traitRepository,
            IQuestionRepository questionRepository,
            IQuestionSetRepository questionSetRepository)
        {
            _jobCategoryRepository = jobCategoryRepository;
            _traitRepository = traitRepository;
            _questionRepository = questionRepository;
            _questionSetRepository = questionSetRepository;
        }

        public async Task CalculateAssessment(UserSession userSession, ILogger log)
        {
            var jobFamilies = await _jobCategoryRepository.GetJobCategories();
            var answerOptions = AnswerOptions;
            
            if (jobFamilies.Length == 0)
            {
                throw new Exception("No job families found!");
            }
            
            var traits = await _traitRepository.GetTraits();
            if (traits.Length == 0)
            {
                throw new Exception("No traits found!");
            }
            
            var questionSet = await _questionSetRepository.GetCurrentQuestionSet("filtered");
            var questions = await _questionRepository.GetQuestions(questionSet.QuestionSetVersion);
            if (questions.Length == 0)
            {
                throw new Exception("No filtering questions found!");
            }

            if (userSession.AssessmentState == null)
            {
                throw new Exception("AssessmentState is not set!");
            }

            RunShortAssessment(userSession, jobFamilies, answerOptions, traits);
 
            PrepareFilterAssessmentState(questionSet.QuestionSetVersion, userSession, jobFamilies, questions);
            
        }

        public void PrepareFilterAssessmentState(string questionSetVersion, UserSession userSession, JobCategory[] jobFamilies,
            Question[] questions)
        {
            foreach (var jobCategory in userSession.ResultData.JobCategories)
            {
                var jobFamily = jobFamilies.First(jf => jf.Code.EqualsIgnoreCase(jobCategory.JobCategoryCode));
                userSession.FilteredAssessmentState = userSession.FilteredAssessmentState ?? new FilteredAssessmentState();
                userSession.FilteredAssessmentState.CreateOrResetCategoryState(questionSetVersion, questions,
                    jobFamily);
            }
        }

        public void RunShortAssessment(UserSession userSession, IEnumerable<JobCategory> jobFamilies, Dictionary<AnswerOption, int> answerOptions, IEnumerable<Trait> traits)
        {
            // User traits
            var userTraits = userSession.AssessmentState.RecordedAnswers
                .Select(x => new
                {
                    x.TraitCode,
                    Score = !x.IsNegative ? answerOptions.First(a => a.Key == x.SelectedOption).Value
                        : answerOptions.First(a => a.Key == x.SelectedOption).Value * -1
                })
                .GroupBy(x => x.TraitCode)
                .Select(g =>
                {
                    var trait = traits.First(x => x.TraitCode == g.First().TraitCode);
                    return new TraitResult()
                    {

                        TraitCode = g.Key,
                        TraitName = trait.TraitName,
                        TraitText = trait.Texts.FirstOrDefault(x => x.LanguageCode.ToLower() == userSession.LanguageCode.ToLower())?.Text,
                        TotalScore = g.Sum(x => x.Score)
                    };
                })
                .OrderByDescending(x => x.TotalScore)
                .ToList();

            var jobCategories = 
                    CalculateJobFamilyRelevance(jobFamilies, userTraits, userSession.LanguageCode)
                        .OrderByDescending(x => x.Total)
                        .Take(10)
                        .ToArray();
            
 
            var resultData = new ResultData()
            {
                Traits = userTraits.Where(x => x.TotalScore > 0).ToArray(),
                JobCategories = jobCategories,
                TraitScores = userTraits.ToArray()
            };

            userSession.ResultData = resultData;
        }

        public IEnumerable<JobCategoryResult> CalculateJobFamilyRelevance(IEnumerable<JobCategory> jobFamilies, IEnumerable<TraitResult> userTraits, string languageCode)
        {
            var traitLookup = userTraits.Where(r => r.TotalScore > 0)
                                        .ToDictionary(r => r.TraitCode, StringComparer.InvariantCultureIgnoreCase);

            var results = new List<JobCategoryResult>();
            foreach (var jobFamily in jobFamilies)
            {
                if (jobFamily.Traits.All(tc => traitLookup.ContainsKey(tc)))
                {
                    var jfText = jobFamily.Texts.FirstOrDefault(t => t.LanguageCode.ToLower() == languageCode?.ToLower());

                    var uTraits = 
                        jobFamily.Traits.Select(tc =>
                        {
                            var trait = traitLookup[tc];
                            return new TraitValue()
                            {
                                TraitCode = trait.TraitCode,
                                Total = trait.TotalScore,
                                NormalizedTotal = jobFamily.ResultMultiplier * Convert.ToDecimal(trait.TotalScore)
                            };

                        }).ToArray();
                    
                    var traitsTotal = uTraits.Sum(r => r.Total);
                    
                   
                    
                    var jcResult = new JobCategoryResult()
                    {
                        JobCategoryName = jobFamily.Name,
                        JobCategoryText = jfText?.Text,
                        Url = jfText?.Url,
                        TraitsTotal = traitsTotal,
                        TraitValues = uTraits,
                        NormalizedTotal = uTraits.Sum(r => r.NormalizedTotal),
                        Total = traitsTotal,
                        TotalQuestions = jobFamily.Skills.Count,
                    };
                    
                    results.Add(jcResult);
                }
            }

            return results.OrderByDescending(t => t.Total).Take(10);
        }
    }
}