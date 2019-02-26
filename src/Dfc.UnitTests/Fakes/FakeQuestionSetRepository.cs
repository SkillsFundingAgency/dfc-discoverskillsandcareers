﻿using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Azure.Documents;
using System;
using System.Threading.Tasks;

namespace Dfc.UnitTests.Fakes
{
    public class FakeQuestionSetRepository : IQuestionSetRepository
    {
        public Task<Document> CreateQuestionSet(QuestionSet questionSet)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionSet> GetCurrentQuestionSet(string assessmentType, string title)
        {
            var questionSet = new QuestionSet()
            {
                AssessmentType = assessmentType,
                Title = title
            };
            return Task.FromResult<QuestionSet>(questionSet);
        }

        public Task<QuestionSet> GetQuestionSetVersion(string assessmentType, string title, int version)
        {
            throw new NotImplementedException();
        }
    }
}