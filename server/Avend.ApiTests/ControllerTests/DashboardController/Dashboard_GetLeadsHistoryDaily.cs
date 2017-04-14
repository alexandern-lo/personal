using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.DashboardController
{
    [TestClass]
    [TestCategory("Integrational")]
    [TestCategory("DashboardController")]
    [TestCategory("DashboardController.GetLeadsHistoryDaily()")]
    public class Dashboard_GetLeadsHistoryDaily: BaseDashboardEndpointTest
    {
        public const string DailyLeadsUrl = "dashboard/leads_history/daily";
        public readonly Uri DailyLeadsUri = new Uri(DailyLeadsUrl, UriKind.Relative);

        const int HistoryDaysCount = 25;

        [TestMethod]
        [TestCategory("Integrational")]
        [TestCategory("DashboardController")]
        [TestCategory("DashboardController.GetLeadsHistoryDaily()")]
        public async Task NoLeadsTest()
        {
            var responseJson = BobTA.GetJsonAsync(DailyLeadsUrl + "?limit=" + HistoryDaysCount);

            var avendResponse = await responseJson.AvendResponse<List<DateIndexedTupleDto<decimal>>>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO")
                .And
                .HaveCount(HistoryDaysCount, "because we should get exactly the number of values we've asked for")
                .And
                .Match(obj => obj.Any(record => record.Value == 0M), "because all without any registered leads all elements of the list should have zero values")
                ;

            avendResponse[0].Date.Should()
                .BeSameDateAs(DateTime.UtcNow.Date.AddDays(-25), "because first element should be the oldest");

            avendResponse[avendResponse.Count-1].Date.Should()
                .BeSameDateAs(DateTime.UtcNow.Date.AddDays(-1), "because last element should always be yesterday");
        }
    }
}