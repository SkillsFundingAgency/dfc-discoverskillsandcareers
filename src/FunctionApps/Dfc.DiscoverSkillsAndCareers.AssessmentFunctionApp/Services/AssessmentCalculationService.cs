using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Task = System.Threading.Tasks.Task;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services
{
    public class AssessmentCalculationService : IAssessmentCalculationService
    {
        private readonly IJobCategoryRepository _jobCategoryRepository;
        private readonly IShortTraitRepository _traitRepository;
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
            IQuestionSetRepository questionSetRepository)
        {
            _jobCategoryRepository = jobCategoryRepository;
            _traitRepository = traitRepository;
            _questionSetRepository = questionSetRepository;
        }

        public async Task CalculateAssessment(UserSession userSession, ILogger log)
        {
            var jobFamilies = await _jobCategoryRepository.GetJobCategories("jobfamily-cms");
            var answerOptions = AnswerOptions;
            var traits = await _traitRepository.GetTraits("shorttrait-cms");
            var filteredQuestionSets = await _questionSetRepository.GetCurrentFilteredQuestionSets();

            if (jobFamilies.Length == 0)
            {
                throw new Exception("No job families found!");
            }
            if (traits.Length == 0)
            {
                throw new Exception("No traits found!");
            }
            if (filteredQuestionSets.Count == 0)
            {
                throw new Exception("No filtering questions found!");
            }

            if (userSession.AssessmentState == null)
            {
                throw new Exception("AssessmentState is not set!");
            }

            RunShortAssessment(userSession, jobFamilies, answerOptions, traits, filteredQuestionSets, log);
        }

        public void RunShortAssessment(UserSession userSession, IEnumerable<JobFamily> jobFamilies, Dictionary<AnswerOption, int> answerOptions, IEnumerable<Trait> traits, IEnumerable<QuestionSet> filteredQuestionSet, ILogger log)
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

            var jobCategories = CalculateJobFamilyRelevance(jobFamilies, userTraits, userSession.LanguageCode).ToArray();
            var filteredQuestionsLookup = filteredQuestionSet.ToDictionary(r => r.QuestionSetKey, StringComparer.InvariantCultureIgnoreCase);

            foreach (var jobCat in jobCategories)
            {
                if (!filteredQuestionsLookup.TryGetValue(jobCat.JobFamilyName.Replace(" ", "-").ToLower(), out var qs))
                {
                    log.LogError(
                        new KeyNotFoundException($"Unable to find a question set for {jobCat.JobFamilyName}"),
                        $"Unable to find a question set for {jobCat.JobFamilyName} available sets include {Environment.NewLine}{String.Join(Environment.NewLine, filteredQuestionsLookup.Keys)}"
                    );
                    continue;
                }


                jobCat.TotalQuestions = qs.MaxQuestions;
            }

            var resultData = new ResultData()
            {
                Traits = userTraits.Where(x => x.TotalScore > 0).ToArray(),
                JobFamilies = jobCategories,
                TraitScores = userTraits.ToArray()
            };

            userSession.ResultData = resultData;
        }

        public List<JobFamilyResult> CalculateJobFamilyRelevance(IEnumerable<JobFamily> jobFamilies, IEnumerable<TraitResult> userTraits, string languageCode)
        {
            var userJobFamilies = jobFamilies
              .Select(x =>
              {
                  var uTraits = userTraits.Where(t => x.TraitCodes.Contains(t.TraitCode)).ToArray();
                  var jfText = x.Texts.FirstOrDefault(t => t.LanguageCode.ToLower() == languageCode?.ToLower());
                  return new JobFamilyResult()
                  {
                      JobFamilyCode = x.JobFamilyCode,
                      JobFamilyName = x.JobFamilyName,
                      JobFamilyText = jfText?.Text,
                      Url = jfText?.Url,
                      TraitsTotal = uTraits.Sum(t => t.TotalScore),
                      TraitValues =
                          uTraits.Select(v => new TraitValue()
                          {
                              TraitCode = v.TraitCode,
                              Total = v.TotalScore,
                              NormalizedTotal = x.ResultMultiplier * v.TotalScore
                          }).ToArray(),
                      NormalizedTotal = x.ResultMultiplier
                  };
              })
              .Where(x => x.TraitValues.Any(v => v.Total > 0))
              .ToList();
            userJobFamilies.ForEach(x =>
            {
                x.Total = x.TraitValues.Where(v => v.NormalizedTotal >= 1m).Sum(v => v.NormalizedTotal);
                x.NormalizedTotal = x.NormalizedTotal * x.TraitValues.Sum(v => v.Total);
            });
            var result = userJobFamilies
                .OrderByDescending(x => x.Total)
                .Take(10)
                .ToList();
            return result;
        }
    }
}