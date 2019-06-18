using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dfc.UnitTests
{
    public class FilteringAssessmentCalculationTests
    {
        private IQuestionRepository _questionRepository;
        private IJobCategoryRepository _jobCategoryRepository;
        private IFilterAssessmentCalculationService _filterAssessmentCalculationService;

        public FilteringAssessmentCalculationTests()
        {
            _questionRepository = Substitute.For<IQuestionRepository>();
            _jobCategoryRepository = Substitute.For<IJobCategoryRepository>();

            _filterAssessmentCalculationService = new FilterAssessmentCalculationService(_questionRepository, _jobCategoryRepository);
        }


        [Fact]
        public async Task Test()
        {
            Assert.True(true);
        }

    }
}
