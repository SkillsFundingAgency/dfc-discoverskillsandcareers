using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Dfc.DiscoverSkillsAndCareers.FunctionApp;

namespace Dfc.LocalDataSetupTests
{
    public class Blobs
    {
        [Fact]
        public async Task CreateLocalHtmlBlobs()
        {
            string containerName = "mycontainer";
            string storageConnectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;";


            var settings = new Dfc.DiscoverSkillsAndCareers.FunctionApp.BlobStorageSettings()
            {
                ContainerName = containerName,
                StorageConnectionString = storageConnectionString
            };
            string blobHtml;
            string name;
            string localFileName;

            name = "questions.html";
            localFileName = @".\pages\QuestionStyled.html";
            await BlobStorageHelper.CreateBlob(settings, localFileName, name);
            blobHtml = await BlobStorageHelper.GetBlob(settings, name);

            Assert.NotNull(blobHtml);
            Assert.Contains("<html", blobHtml);

            name = "results.html";
            blobHtml = await BlobStorageHelper.GetBlob(settings, name);
            localFileName = @".\pages\ResultsStyled.html";
            await BlobStorageHelper.CreateBlob(settings, localFileName, name);
            blobHtml = await BlobStorageHelper.GetBlob(settings, name);

            Assert.NotNull(blobHtml);
            Assert.Contains("<html", blobHtml);
        }
    }
}
