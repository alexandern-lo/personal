using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.API.Services.Leads.NetworkDTO;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

namespace Avend.ApiTests.ControllerTests.LeadsController
{
    [TestClass]
    [TestCategory("Integrational")]
    [TestCategory("LeadsController")]
    [TestCategory("LeadsController.ExportLeadsToFile()")]
    // ReSharper disable once InconsistentNaming
    public class Leads_ExportLeadsToFile : BaseLeadsEndpointTest
    {
        string UrlApiV1LeadsExportToFile = "leads/export/file";

        [TestMethod]
        public async Task ShouldGenerateProperFileNameWhenPassedNullForUidsList()
        {
            await BobLeadsData.Add();

            var postData = JsonConvert.SerializeObject(new LeadsExportRequestDto
            {
                Uids = null,
            });

            HttpContent postContent = new StringContent(postData, Encoding.UTF8, "application/json");
            var postResponseMessage = await BobTA.PostAsync(UrlApiV1LeadsExportToFile, postContent);

            postResponseMessage.IsSuccessStatusCode
                .Should()
                .BeTrue("because leads export file generation should be successful");

            IEnumerable<string> disposition;
            var dispositionPresents = postResponseMessage.Content.Headers.TryGetValues("Content-Disposition", out disposition);

            dispositionPresents.Should()
                .BeTrue("because export to file should produce a valid Content-Disposition response header");

            disposition.Should()
                .HaveCount(1, "because export to file should produce a single Content-Disposition response header");

            disposition.First().Should()
                .Contain("filename=Avend", "because filename should start with Avend")
                .And
                .Contain(TestUser.BobTester.FirstName + "_" + TestUser.BobTester.LastName, "because filename should contain the user's fullname divided by underscore")
                .And
                .Contain(DateTime.UtcNow.ToString("dd.MM.yyyy"), "because filename should contain the current date in UTC formatted as dd.MM.yyyy")
                ;
        }

        [TestMethod]
        public async Task ShouldExportAllUserLeadsWhenPassedNullForUidsList()
        {
            await BobLeadsData.Add();
            await BobLeadsData.Add();

            var leadsDtos = BobLeadsData.Leads;
            var leadUids = leadsDtos.Where(record => record.Uid.HasValue).Select(record => record.Uid.Value).ToList();

            var postData = JsonConvert.SerializeObject(new LeadsExportRequestDto
            {
                Uids = null,
            });

            HttpContent postContent = new StringContent(postData, Encoding.UTF8, "application/json");
            var postResponseMessage = await BobTA.PostAsync(UrlApiV1LeadsExportToFile, postContent);
            var postResponseBody = await postResponseMessage.Content.ReadAsStringAsync();

            postResponseMessage.IsSuccessStatusCode
                .Should()
                .BeTrue("because leads file generation should be successful");

            postResponseBody.Split('\n').Count()
                .Should()
                .Be(leadUids.Count + 1, "because export file should have all leads and a header too");
        }

        [TestMethod]
        public async Task ShouldExportAllUserLeadsWhenPassedEmptyArrayForUidsList()
        {
            await BobLeadsData.Add();
            await BobLeadsData.Add();

            var leadsDtos = BobLeadsData.Leads;
            var leadUids = leadsDtos.Where(record => record.Uid.HasValue).Select(record => record.Uid.Value).ToList();

            var postData = JsonConvert.SerializeObject(new LeadsExportRequestDto
            {
                Uids = new List<Guid?>(),
            });

            HttpContent postContent = new StringContent(postData, Encoding.UTF8, "application/json");
            var postResponseMessage = await BobTA.PostAsync(UrlApiV1LeadsExportToFile, postContent);
            var postResponseBody = await postResponseMessage.Content.ReadAsStringAsync();

            postResponseMessage.IsSuccessStatusCode
                .Should()
                .BeTrue("because leads file generation should be successful");

            postResponseBody.Split('\n').Count()
                .Should()
                .Be(leadUids.Count + 1, "because export file should have all leads and a header too");
        }

        [TestMethod]
        public async Task ShouldExportSingleLeadWhenPassedSingleValidLeadUidInUidsList()
        {
            await BobLeadsData.Add();

            var leadsDtos = BobLeadsData.Leads;
            var leadUids = leadsDtos.Where(record => record.Uid.HasValue).Select(record => record.Uid).Take(1).ToList();

            var postData = JsonConvert.SerializeObject(new LeadsExportRequestDto
            {
                Uids = leadUids,
            });

            HttpContent postContent = new StringContent(postData, Encoding.UTF8, "application/json");
            var postResponseMessage = await BobTA.PostAsync(UrlApiV1LeadsExportToFile, postContent);
            var postResponseBody = await postResponseMessage.Content.ReadAsStringAsync();

            postResponseMessage.IsSuccessStatusCode
                .Should()
                .BeTrue("because leads file generation should be successful");

            postResponseBody.Split('\n').Count()
                .Should()
                .Be(2, "because export file should have single lead and a header too");
        }

        [TestMethod]
        public async Task ShouldReturnEmptyFileWhenPassedJustASingleNullValueInUidsList()
        {
            var postData = JsonConvert.SerializeObject(new LeadsExportRequestDto
            {
                Uids = new List<Guid?>()
                {
                    null,
                },
            });

            HttpContent postContent = new StringContent(postData, Encoding.UTF8, "application/json");
            var postResponseMessage = await BobTA.PostAsync(UrlApiV1LeadsExportToFile, postContent);
            var postResponseBody = await postResponseMessage.Content.ReadAsStringAsync();

            postResponseMessage.IsSuccessStatusCode
                .Should()
                .BeTrue("because leads file generation should be successful");

            postResponseBody.Split('\n').Count()
                .Should()
                .Be(1, "because éxport file should have only header");
        }
    }
}