using System;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventAttendeeCategories
{
    [TestClass]
    public class CreateAttendeeCategoryTest : AttendeeCategoryCreateUpdateTestBase
    {
        public override HttpRequestMessage MakeRequest(Guid eventUid)
        {
            return new HttpRequestMessage(HttpMethod.Post, $"events/{eventUid}/attendee_categories");
        }
    }
}
