﻿using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public interface IQuestionSetRepository
    {
        Task<QuestionSet> GetCurrentQuestionSet(string assessmentType, string title);
        Task<Document> CreateOrUpdateQuestionSet(QuestionSet questionSet);
        Task<QuestionSet> GetQuestionSetVersion(string assessmentType, string title, int version);
        Task<List<QuestionSet>> GetCurrentFilteredQuestionSets();
    }
}
