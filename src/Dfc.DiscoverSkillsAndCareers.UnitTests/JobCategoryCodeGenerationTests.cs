using Dfc.DiscoverSkillsAndCareers.Models;
using Xunit;

namespace Dfc.UnitTests
{
    public class JobCategoryCodeGenerationTests
    {
        [Theory]
        [InlineData("Administration", "ADMIN")]
        [InlineData("Animal Care", "AC")]
        [InlineData("Business and Finance", "BAF")]
        [InlineData("Computing, technology and digital", "CTAD")]
        [InlineData("Construction and trades", "CAT")]
        [InlineData("Creative and Media", "CAM")]
        [InlineData("Delivery and storage", "DAS")]
        [InlineData("Emergency and uniform services", "EAUS")]
        [InlineData("Engineering and maintenance", "EAM")]
        [InlineData("Environment and land", "EAL")]
        [InlineData("Government services", "GS")]
        [InlineData("Healthcare", "HEALT")]
        [InlineData("Home services", "HS")]
        [InlineData("Hospitality and food", "HAF")]
        [InlineData("Law and legal", "LAL")]
        [InlineData("Managerial", "MANAG")]
        [InlineData("Manufacturing", "MANUF")]
        [InlineData("Retail and sales", "RAS")]
        [InlineData("Science and research", "SAR")]
        [InlineData("Social Care", "SC")]
        [InlineData("Sports and leisure", "SAL")]
        [InlineData("Teaching and education", "TAE")]
        [InlineData("Transport", "TRANS")]
        [InlineData("Travel and Tourism", "TAT")]
        
        public void CanGenerateJobCategoryCode(string category, string expected)
        {
            var processor = JobCategoryHelper.GetCode(category);
            Assert.Equal(expected, processor);
        }
    }
}