using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.Resources
{
    [TestClass]
    public class ResourcesCreateTest : ResourcesCreateUpdateTestBase
    {
        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            Resource = ResourceData.MakeSample();
        }

        protected override HttpRequestMessage MakeRequest()
        {
            return new HttpRequestMessage(HttpMethod.Post, Resources);
        }

        [TestMethod]
        public async Task CreateUploadToken()
        {
            const string testFileContent = "This is a test object";
            const string testFileName = "test-blah-blah.txt";
            var fileUrl =
                await BobTA.PostJsonAsync($"{Resources}/upload_token/{testFileName}", "")
                    .AvendResponse<string>();

            fileUrl.Should()
                .Contain(AzureCloudStorageUrl,
                    "because returned resource's cloud URL should contain Azure Cloud Storage site's as part of the URL");
            fileUrl.Should()
                .Contain(testFileName,
                    "because returned resource's cloud URL should contain passed file name as part of URL");

            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("x-ms-blob-type", "BlockBlob");
                http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var azureResponse = await http.PutAsync(fileUrl, new StringContent(testFileContent));

                azureResponse.IsSuccessStatusCode.Should()
                    .BeTrue("because we expect fresh upload token to work");

                var fileContentResponse = await http.GetAsync(fileUrl);
                fileContentResponse.IsSuccessStatusCode.Should()
                    .BeTrue("because we expect file to be created");

                var loadedFileContent = await fileContentResponse.Content.ReadAsStringAsync();
                loadedFileContent.Should()
                    .Be(testFileContent);
            }
        }
    }
}