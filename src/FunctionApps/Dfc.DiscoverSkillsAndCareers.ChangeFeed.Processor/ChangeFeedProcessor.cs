using Dfc.DiscoverSkillsAndCareers.ChangeFeed.Common.Blob;
using Dfc.DiscoverSkillsAndCareers.ChangeFeed.Common.Queue;
using Dfc.DiscoverSkillsAndCareers.ChangeFeed.Data;
using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Linq;
using Dfc.DiscoverSkillsAndCareers.ChangeFeed.Common;

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Processor
{
    public static class ChangeFeedProcessor
    {
        private static IUnderstandMyselfDbContext DbContext;

        [FunctionName("ChangeFeedProcessor")]
        public static async Task RunAsync([ServiceBusTrigger("data-updates", Connection = "AzureServiceBusConnection")]string myQueueItem,
            ILogger log,
            [Inject]IBlobStorageService blobStorageService,
            [Inject]IUnderstandMyselfDbContext understandMyselfDbContext)
        {
            try
            {
                log.LogInformation($"Change feed received {myQueueItem}");

                DbContext = understandMyselfDbContext;

                // Get the blob from the store, myQueueItem contains the reference
                var changeFeedQueueItem = JsonConvert.DeserializeObject<ChangeFeedQueueItem>(myQueueItem);

                // Load the existing data item (eg. user session) check if we have a newer copy
                var blob = await blobStorageService.GetBlobText(changeFeedQueueItem.BlobName);
                if (blob == null)
                {
                    return;
                }

                // Update the sql database
                switch (changeFeedQueueItem.Type)
                {
                    case "UserSession":
                        {
                            var userSession = JsonConvert.DeserializeObject<Dfc.DiscoverSkillsAndCareers.Models.UserSession>(blob);
                            await UpdateUserSession(userSession);
                            await blobStorageService.DeleteBlob(changeFeedQueueItem.BlobName);
                            break;
                        }
                    case "Question":
                        {
                            var question = JsonConvert.DeserializeObject<Question>(blob);
                            await UpdateQuestion(question);
                            await blobStorageService.DeleteBlob(changeFeedQueueItem.BlobName);
                            break;
                        }
                    case "QuestionSet":
                        {
                            var questionSet = JsonConvert.DeserializeObject<Dfc.DiscoverSkillsAndCareers.Models.QuestionSet>(blob);
                            await UpdateQuestionSet(questionSet);
                            await blobStorageService.DeleteBlob(changeFeedQueueItem.BlobName);
                            break;
                        }
                    default:
                        {
                            throw new NotImplementedException(changeFeedQueueItem.Type);
                        }
                }
            }
            catch (Exception ex)
            {
                log.LogInformation($"Change feed exception {ex.ToString()}");
            }
        }

        private static async Task UpdateQuestion(Question question)
        {
            try
            {
                var entity = await DbContext.Questions.FirstOrDefaultAsync(x => x.Id == question.QuestionId);
                if (entity == null)
                {
                    entity = new Data.Entities.UmQuestion { Id = question.QuestionId };
                    UpdateQuestionEntityFromDto(entity, question);
                    DbContext.Questions.Add(entity);
                }
                else
                {
                    UpdateQuestionEntityFromDto(entity, question);
                    DbContext.Questions.Update(entity);
                }

                var toremoveExcludeJobProfiles = DbContext.QuestionJobProfiles.Where(x => x.QuestionId == question.QuestionId).ToList();
                DbContext.QuestionJobProfiles.RemoveRange(toremoveExcludeJobProfiles);
                if (question.JobProfiles != null)
                {
                    foreach (var jp in question.JobProfiles)
                    {
                        DbContext.QuestionJobProfiles.Add(new Data.Entities.UmQuestionJobProfile
                        {
                            Id = Guid.NewGuid().ToString(),
                            JobProfile = jp.JobProfile,
                            QuestionId = question.QuestionId,
                            Included = jp.Included
                        });
                    }
                }
   
                int changes = await DbContext.SaveChanges();
                Console.WriteLine($"Changes updated {changes}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        private static void UpdateQuestionEntityFromDto(Data.Entities.UmQuestion entity, Question dto)
        {
            entity.IsNegative = dto.IsNegative;
            entity.NegativeResultDisplayText = dto.NegativeResultDisplayText;
            entity.Order = dto.Order;
            entity.PositiveResultDisplayText = dto.PositiveResultDisplayText;
            entity.SfId = dto.SfId;
            entity.TraitCode = dto.TraitCode;
            entity.Text = dto.Texts.FirstOrDefault()?.Text;
            entity.LastUpdatedDt = dto.LastUpdatedDt.UtcDateTime;
        }

        private static async Task UpdateUserSession(Dfc.DiscoverSkillsAndCareers.Models.UserSession userSession)
        {
            try
            {
                var entity = DbContext.UserSessions.FirstOrDefault(x => x.Id == userSession.UserSessionId);
                if (entity == null)
                {
                    entity = new Data.Entities.UmUserSession { Id = userSession.UserSessionId };
                    UpdateUserSessionEntityFromDto(entity, userSession);
                    DbContext.UserSessions.Add(entity);
                }
                else
                {
                    UpdateUserSessionEntityFromDto(entity, userSession);
                    DbContext.UserSessions.Update(entity);
                }

                var toremoveAnswers = DbContext.Answers.Where(x => x.UserSessionId == userSession.UserSessionId).ToList();
                DbContext.Answers.RemoveRange(toremoveAnswers);

                foreach (var answer in userSession.AssessmentState?.RecordedAnswers)
                {
                    DbContext.Answers.Add(new Data.Entities.UmAnswer()
                    {
                        Id = userSession.UserSessionId + "-" + answer.QuestionId,
                        UserSessionId = userSession.UserSessionId,
                        AnsweredDt = answer.AnsweredDt,
                        IsNegative = answer.IsNegative,
                        QuestionId = answer.QuestionId,
                        QuestionNumber = answer.QuestionNumber,
                        QuestionSetVersion = answer.QuestionSetVersion,
                        QuestionText = answer.QuestionText,
                        SelectedOption = answer.SelectedOption.ToString(),
                        TraitCode = answer.TraitCode,
                        IsFiltered = false
                    });
                }

                foreach (var answer in (userSession.FilteredAssessmentState?.RecordedAnswers) ?? new Models.Answer[]{ })
                {
                    DbContext.Answers.Add(new Data.Entities.UmAnswer()
                    {
                        Id = userSession.UserSessionId + "-" + answer.QuestionId,
                        UserSessionId = userSession.UserSessionId,
                        AnsweredDt = answer.AnsweredDt,
                        IsNegative = answer.IsNegative,
                        QuestionId = answer.QuestionId,
                        QuestionNumber = answer.QuestionNumber,
                        QuestionSetVersion = answer.QuestionSetVersion,
                        QuestionText = answer.QuestionText,
                        SelectedOption = answer.SelectedOption.ToString(),
                        TraitCode = answer.TraitCode,
                        IsFiltered = true
                    });
                }

                var toremoveResultStatements = DbContext.ResultStatements.Where(x => x.UserSessionId == userSession.UserSessionId).ToList();
                DbContext.ResultStatements.RemoveRange(toremoveResultStatements);
                if (userSession?.ResultData?.Traits != null)
                {
                    foreach (var trait in userSession.ResultData.Traits)
                    {
                        DbContext.ResultStatements.Add(new Data.Entities.UmResultStatement()
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserSessionId = userSession.UserSessionId,
                            TextDisplayed = trait.TraitText,
                            IsTrait = true
                        });
                    }
                }
                if (userSession?.ResultData != null)
                {
                    foreach (var jobFamily in userSession.ResultData.JobFamilies)
                    {
                        if (jobFamily.FilterAssessment != null) {
                            foreach (var whatYouToldUs in jobFamily.FilterAssessment.WhatYouToldUs)
                            {
                                DbContext.ResultStatements.Add(new Data.Entities.UmResultStatement()
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    UserSessionId = userSession.UserSessionId,
                                    TextDisplayed = whatYouToldUs,
                                    IsFilter = true
                                });
                            }
                        }
                    }
                }

                var toremoveTraitScores = DbContext.TraitScores.Where(x => x.UserSessionId == userSession.UserSessionId).ToList();
                DbContext.TraitScores.RemoveRange(toremoveTraitScores);
                if (userSession?.ResultData?.Traits != null)
                {
                    foreach (var trait in userSession.ResultData.Traits)
                    {
                        DbContext.TraitScores.Add(new Data.Entities.UmTraitScore()
                        {
                            Id = userSession.UserSessionId + "-" + trait.TraitName,
                            UserSessionId = userSession.UserSessionId,
                            Trait = trait.TraitName,
                            Score = trait.TotalScore
                        });
                    }
                }

                var toremoveSuggestedJobCategories = DbContext.SuggestedJobCategories.Where(x => x.UserSessionId == userSession.UserSessionId).ToList();
                DbContext.SuggestedJobCategories.RemoveRange(toremoveSuggestedJobCategories);
                if (userSession.ResultData?.JobFamilies != null)
                {
                    foreach (var jobCategory in userSession?.ResultData?.JobFamilies)
                    {
                        DbContext.SuggestedJobCategories.Add(new Data.Entities.UmSuggestedJobCategory()
                        {
                            Id = userSession.UserSessionId + "-" + jobCategory.JobFamilyCode,
                            UserSessionId = userSession.UserSessionId,
                            JobCategoryCode = jobCategory.JobFamilyCode,
                            JobCategory = jobCategory.JobFamilyName,
                            TraitTotal = jobCategory.TraitsTotal,
                            JobCategoryScore = jobCategory.NormalizedTotal,
                            HasCompletedFilterAssessment = jobCategory.FilterAssessment != null
                        });

                        if (jobCategory.FilterAssessment != null)
                        {
                            foreach (var soccode in jobCategory.FilterAssessment?.SuggestedJobProfiles)
                            {
                                var toremoveSuggestedJobProfiles = DbContext.SuggestedJobProfiles.Where(x => x.UserSessionId == userSession.UserSessionId && x.JobCategoryCode == jobCategory.JobFamilyCode).ToList();
                                DbContext.SuggestedJobProfiles.RemoveRange(toremoveSuggestedJobProfiles);
                                DbContext.SuggestedJobProfiles.Add(new Data.Entities.UmSuggestedJobProfile()
                                {
                                    Id = userSession.UserSessionId + "-" + soccode.Key,
                                    UserSessionId = userSession.UserSessionId,
                                    JobCategoryCode = jobCategory.JobFamilyCode,
                                    SocCode = soccode.Key
                                });
                            }
                        }
                    }
                }

                int changes = await DbContext.SaveChanges();
                Console.WriteLine($"Changes updated {changes}");
            }
            catch (System.InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        private static void UpdateUserSessionEntityFromDto(Data.Entities.UmUserSession entity, Dfc.DiscoverSkillsAndCareers.Models.UserSession dto)
        {
            entity.AssessmentType = dto.AssessmentType;
            entity.CompleteDt = dto.CompleteDt;
            entity.CurrentFilterAssessmentCode = dto.CurrentQuestionSetVersion;
            entity.CurrentQuestion = dto.CurrentQuestion;
            entity.IsComplete = dto.IsComplete;
            entity.LanguageCode = dto.LanguageCode;
            entity.LastUpdatedDt = dto.LastUpdatedDt;
            entity.MaxQuestions = dto.MaxQuestions;
            entity.QuestionSetVersion = dto.CurrentQuestionSetVersion;
            entity.StartedDt = dto.StartedDt;

            if (entity.LastUpdatedDt == new DateTime())
            {
                entity.LastUpdatedDt = DateTime.UtcNow;
            }
        }

        private static async Task UpdateQuestionSet(Dfc.DiscoverSkillsAndCareers.Models.QuestionSet questionSet)
        {
            try
            {
                var entity = await DbContext.QuestionSets.FirstOrDefaultAsync(x => x.Id == questionSet.QuestionSetVersion);
                if (entity == null)
                {
                    entity = new Data.Entities.UmQuestionSet { Id = questionSet.QuestionSetVersion };
                    UpdateQuestionSetEntityFromDto(entity, questionSet);
                    DbContext.QuestionSets.Add(entity);
                }
                else
                {
                    UpdateQuestionSetEntityFromDto(entity, questionSet);
                    DbContext.QuestionSets.Update(entity);
                }

                int changes = await DbContext.SaveChanges();
                Console.WriteLine($"Changes updated {changes}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        private static void UpdateQuestionSetEntityFromDto(Data.Entities.UmQuestionSet entity, Dfc.DiscoverSkillsAndCareers.Models.QuestionSet dto)
        {
            entity.AssessmentType = dto.AssessmentType;
            entity.Id = dto.QuestionSetVersion;
            entity.MaxQuestions = dto.MaxQuestions;
            entity.Title = dto.Title;
            entity.Version = dto.Version;
            entity.LastUpdatedDt = dto.LastUpdated;
        }
    }
}
