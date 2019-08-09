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
                            await UpdateUserSession(userSession, log);
                            await blobStorageService.DeleteBlob(changeFeedQueueItem.BlobName);
                            break;
                        }
                    case "Question":
                        {
                            var question = JsonConvert.DeserializeObject<Question>(blob);
                            await UpdateQuestion(question, log);
                            await blobStorageService.DeleteBlob(changeFeedQueueItem.BlobName);
                            break;
                        }
                    case "QuestionSet":
                        {
                            var questionSet = JsonConvert.DeserializeObject<Dfc.DiscoverSkillsAndCareers.Models.QuestionSet>(blob);
                            await UpdateQuestionSet(questionSet, log);
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

        private static async Task UpdateQuestion(Question question, ILogger log)
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
                
                int changes = await DbContext.SaveChanges();
                log.LogTrace($"Changes updated {changes}");
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Unable to update question {question.QuestionId}");
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

        private static async Task UpdateUserSession(Dfc.DiscoverSkillsAndCareers.Models.UserSession userSession, ILogger log)
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
                    foreach (var jobFamily in userSession.ResultData.JobCategories)
                    {
                        if (jobFamily.FilterAssessmentResult != null) {
                            foreach (var whatYouToldUs in jobFamily.FilterAssessmentResult.WhatYouToldUs)
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
                if (userSession.ResultData?.JobCategories != null)
                {
                    foreach (var jobCategory in userSession?.ResultData?.JobCategories)
                    {
                        DbContext.SuggestedJobCategories.Add(new Data.Entities.UmSuggestedJobCategory()
                        {
                            Id = userSession.UserSessionId + "-" + jobCategory.JobCategoryCode,
                            UserSessionId = userSession.UserSessionId,
                            JobCategoryCode = jobCategory.JobCategoryCode,
                            JobCategory = jobCategory.JobCategoryName,
                            TraitTotal = jobCategory.TraitsTotal,
                            JobCategoryScore = jobCategory.NormalizedTotal,
                            HasCompletedFilterAssessment = jobCategory.FilterAssessmentResult != null
                        });

                        if (jobCategory.FilterAssessmentResult != null)
                        {
                            var existingProfiles = 
                                DbContext.SuggestedJobProfiles.Where(x => x.UserSessionId == userSession.UserSessionId && x.JobCategoryCode == jobCategory.JobCategoryCode)
                                    .ToDictionary(p => p.Id);
                            
                            DbContext.SuggestedJobProfiles.RemoveRange(existingProfiles.Values);
                            
                            foreach (var jobProfile in jobCategory.FilterAssessmentResult.SuggestedJobProfiles)
                            {
                                var id = userSession.UserSessionId + "-" + jobCategory.JobCategoryCode + "-" + jobProfile;
                                try
                                {
                                    if (!existingProfiles.TryGetValue(id, out var p) && !DbContext.SuggestedJobProfiles.Local.Any(q => q.Id == id))
                                    {
                                        var suggestedJobProfile = new Data.Entities.UmSuggestedJobProfile()
                                        {
                                            Id = id,
                                            UserSessionId = userSession.UserSessionId,
                                            JobCategoryCode = jobCategory.JobCategoryCode,
                                            JobProfile = jobProfile
                                        };
                                        DbContext.SuggestedJobProfiles.Add(suggestedJobProfile);
                                        existingProfiles.Add(suggestedJobProfile.Id, suggestedJobProfile);
                                    }
                                }
                                catch (Exception e)
                                {
                                    log.LogWarning(e, $"Failed to add suggested job profile - {id}, typically this warning can be ignored, " +
                                                      $"as it is due to a partial session being captured on the change feed. " +
                                                      $"Following updates will correct the data.");
                                }
                                
                            }
                        }
                    }
                }

                int changes = await DbContext.SaveChanges();
                log.LogTrace($"Applied {changes} changes");
            }
            catch (System.InvalidOperationException ex)
            {
                log.LogWarning(ex, $"Error updating user session {userSession?.UserSessionId}");
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error updating user session {userSession?.UserSessionId}");
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

        private static async Task UpdateQuestionSet(Dfc.DiscoverSkillsAndCareers.Models.QuestionSet questionSet, ILogger log)
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
                log.LogTrace($"Changes updated {changes}");
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Failed to update question set");
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
            entity.LastUpdatedDt = dto.LastUpdated.UtcDateTime;
        }
    }
}
