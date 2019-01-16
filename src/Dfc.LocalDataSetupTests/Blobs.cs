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

            Environment.SetEnvironmentVariable("ContainerName", containerName);
            Environment.SetEnvironmentVariable("BlobStorage:StorageConnectionString", storageConnectionString);

            string blobHtml;
            string name;
            string localFileName;

            name = "Question.html";
            localFileName = @".\pages\QuestionStyled.html";
            await BlobStorageHelper.CreateBlob(localFileName, name);
            blobHtml = await BlobStorageHelper.GetBlob(name);

            Assert.NotNull(blobHtml);
            Assert.Contains("<html", blobHtml);

            name = "Results.html";
            blobHtml = await BlobStorageHelper.GetBlob(name);
            localFileName = @".\pages\ResultsStyled.html";
            await BlobStorageHelper.CreateBlob(localFileName, name);
            blobHtml = await BlobStorageHelper.GetBlob(name);

            Assert.NotNull(blobHtml);
            Assert.Contains("<html", blobHtml);
        }
    }
}
