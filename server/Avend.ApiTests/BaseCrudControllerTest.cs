using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.API.Infrastructure.Responses;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Avend.ApiTests
{
    public class BaseCrudControllerTest<TDto> : BaseControllerTest
    {
        public readonly Guid BobTesterUid = TestUser.BobTester.Uid;

        public string ApiResourceUrl { get; set; }

        protected virtual string EntityName => typeof(TDto).Name.ToLowerInvariant();

        protected virtual string EntityPluralName => typeof(TDto).Name.ToLowerInvariant() + "s";

        public BaseCrudControllerTest(string apiResourceUrl)
        {
            ApiResourceUrl = apiResourceUrl;
        }
        
        [TestInitialize]
        public override async Task Init()
        {
            var baseInit = base.Init();
            var conferenceEvent = EventData.MakeSample(eventDto => eventDto.Name = "Conference Event By Alex");
            var eventDataTask = ConferenceEventData.AddFromDto(conferenceEvent);
            await Task.WhenAll(baseInit, eventDataTask);
        }

        public virtual void SendGetEntityByUidRequestExtraValidations(
            Guid entityUid,
            TDto testEntityObject
        )
        {
        }

        public async Task<TDto> SendGetEntityByUidRequest(
            Guid entityUid,
            TDto testEntityObject,
            Dictionary<Expression, string> validationRules
        )
        {
            var responseMessage = await BobTA.GetAsync($"{ApiResourceUrl}/{entityUid}");

            var responseBody = await responseMessage.Content.ReadAsStringAsync();

            responseMessage.IsSuccessStatusCode
                .Should()
                .BeTrue(
                    $"because HTTP request for retrieving {EntityName} by UID should succeed (got status {responseMessage.StatusCode})");

            var responseJson = JsonConvert.DeserializeObject<OkResponse<TDto>>(responseBody);

            responseJson.Success
                .Should()
                .BeTrue($"because data request for retrieving {EntityName} by UID should indicate success");

            SendGetEntityByUidRequestExtraValidations(entityUid, testEntityObject);

            return responseJson.Data;
        }

        public async Task<List<TDto>> SendGetEntitiesListRequest(int expectedTotalCount, int expectedCount)
        {
            var responseMessage = await BobTA.GetAsync(ApiResourceUrl);

            responseMessage.IsSuccessStatusCode
                .Should()
                .BeTrue($"because HTTP request for {EntityPluralName} list should succeed");

            var responseBody = await responseMessage.Content.ReadAsStringAsync();

            var responseJson = JsonConvert.DeserializeObject<OkListResponse<TDto>>(responseBody);

            responseJson.Success
                .Should()
                .BeTrue($"because data request for {EntityPluralName} list should indicate success");

            responseJson.TotalFilteredRecords
                .Should()
                .Be(expectedTotalCount,
                    $"because in current database we should have exactly {expectedTotalCount} of {EntityPluralName} ");

            responseJson.Data.Count
                .Should()
                .Be(expectedCount,
                    $"because in current database total records count should be {expectedCount}");

            Console.Out.WriteLine(responseBody);
            Console.Error.WriteLine(responseBody);

            return responseJson.Data;
        }

        public async Task<List<TDto>> SendPostEntitiesListFilterRequest(string body, int expectedTotalCount,
            int expectedCount)
        {
            var newEntityContent = new StringContent(body);

            newEntityContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var responseMessage = await BobTA.PostAsync($"{ApiResourceUrl}_filter", newEntityContent);

            responseMessage.IsSuccessStatusCode
                .Should()
                .BeTrue($"because HTTP request for {EntityPluralName} list should succeed");

            var responseBody = await responseMessage.Content.ReadAsStringAsync();

            var responseJson = JsonConvert.DeserializeObject<OkListResponse<TDto>>(responseBody);

            responseJson.Success
                .Should()
                .BeTrue($"because data request for {EntityPluralName} list should indicate success");

            responseJson.TotalFilteredRecords
                .Should()
                .Be(expectedTotalCount,
                    $"because in current database we should have exactly {expectedTotalCount} of {EntityPluralName} ");

            responseJson.Data.Count
                .Should()
                .Be(expectedCount,
                    $"because in current database total records count should be {expectedCount}");

            Console.Out.WriteLine(responseBody);
            Console.Error.WriteLine(responseBody);

            return responseJson.Data;
        }

        public async Task<Guid> SendNewEntityRequest(TDto testEntity)
        {
            var newEntityContent = new StringContent(JsonConvert.SerializeObject(testEntity));

            newEntityContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var responseMessage = await BobTA.PostAsync(ApiResourceUrl, newEntityContent);

            var responseBody = await responseMessage.Content.ReadAsStringAsync();

            responseMessage.IsSuccessStatusCode
                .Should()
                .BeTrue(
                    $"because HTTP request for new {EntityName} should succeed (got status {responseMessage.StatusCode})");

            var responseJson = JsonConvert.DeserializeObject<OkResponse<Guid>>(responseBody);

            responseJson.Success
                .Should()
                .BeTrue($"because data request for new {EntityName} should indicate success");

            responseJson.Data
                .Should()
                .NotBeEmpty($"because returned {EntityName}\'s Uid must be valid");

            return responseJson.Data;
        }
    }
}