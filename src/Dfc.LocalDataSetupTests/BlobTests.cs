using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Dfc.DiscoverSkillsAndCareers.FunctionApp;

namespace Dfc.LocalDataSetupTests
{
    public class BlobTests
    {
        [Fact]
        public async Task GetBlob_WithStartHtml_ShouldHaveBlob()
        {
            string blobHtml;
            string name;

            name = "Question.html";
            blobHtml = await BlobStorageHelper.GetBlob(name);
            if (blobHtml == null)
            {
                string localFileName = @".\pages\QuestionPage.html";
                await BlobStorageHelper.CreateBlob(localFileName, name);
                blobHtml = await BlobStorageHelper.GetBlob(name);
            }

            Assert.NotNull(blobHtml);
            Assert.Contains("<html", blobHtml);
        }
    }
}
