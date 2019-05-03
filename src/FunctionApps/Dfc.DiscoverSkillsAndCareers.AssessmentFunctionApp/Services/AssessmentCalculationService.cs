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
        private IJobCategoryRepository _jobCategoryRepository;
        private IShortTraitRepository _traitRepository;
        private readonly IQuestionSetRepository _questionSetRepository;
        private ILogger _log;

        private static Dictionary<AnswerOption, int> AnswerOptions = new Dictionary<AnswerOption, int>()
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
            IQuestionSetRepository questionSetRepository,
            ILogger log)
        {
            _jobCategoryRepository = jobCategoryRepository;
            _traitRepository = traitRepository;
            _questionSetRepository = questionSetRepository;
            _log = log;
        }

        public async Task CalculateAssessment(UserSession userSession)
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

            RunShortAssessment(userSession, jobFamilies, answerOptions, traits, filteredQuestionSets);
        }

        public void RunShortAssessment(UserSession userSession, IEnumerable<JobFamily> jobFamilies, Dictionary<AnswerOption, int> answerOptions, IEnumerable<Trait> traits, IEnumerable<QuestionSet> filteredQuestionSet)
        {
            // User traits
            var userTraits = userSession.AssessmentState.RecordedAnswers
                .Select(x => new
                {
                    x.TraitCode,
                    Score = !x.IsNegative ? answerOptions.Where(a => a.Key == x.SelectedOption).First().Value
                        : answerOptions.Where(a => a.Key == x.SelectedOption).First().Value * -1
                })
                .GroupBy(x => x.TraitCode)
                .Select(g => new TraitResult()
                {
                    TraitCode = g.First().TraitCode,
                    TraitName = traits.Where(x => x.TraitCode == g.First().TraitCode).First().TraitName,
                    TraitText = traits.Where(x => x.TraitCode == g.First().TraitCode).First().Texts.Where(x => x.LanguageCode.ToLower() == userSession.LanguageCode.ToLower()).FirstOrDefault()?.Text,
                    TotalScore = g.Sum(x => x.Score)
                })
                .OrderByDescending(x => x.TotalScore)
                .ToList();

            var jobCategories = CalculateJobFamilyRelevance(jobFamilies, userTraits, userSession.LanguageCode).ToArray();
            var filteredQuestionsLookup = filteredQuestionSet.ToDictionary(r => r.Title, StringComparer.InvariantCultureIgnoreCase);

            foreach (var jobCat in jobCategories)
            {
                if (!filteredQuestionsLookup.TryGetValue(jobCat.JobFamilyName, out var qs))
                {
                    _log.LogError(
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
              .Select(x => new JobFamilyResult()
              {
                  JobFamilyCode = x.JobFamilyCode,
                  JobFamilyName = x.JobFamilyName,
                  JobFamilyText = x.Texts.Where(t => t.LanguageCode.ToLower() == languageCode?.ToLower()).FirstOrDefault()?.Text,
                  Url = x.Texts.Where(t => t.LanguageCode.ToLower() == languageCode?.ToLower()).FirstOrDefault()?.Url,
                  TraitsTotal = userTraits.Where(t => x.TraitCodes.Contains(t.TraitCode)).Sum(t => t.TotalScore),
                  TraitValues = userTraits
                      .Where(t => x.TraitCodes.Contains(t.TraitCode))
                      .Select(v => new TraitValue()
                      {
                          TraitCode = v.TraitCode,
                          Total = v.TotalScore,
                          NormalizedTotal = x.ResultMultiplier * v.TotalScore
                      }).ToArray(),
                  NormalizedTotal = x.ResultMultiplier
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