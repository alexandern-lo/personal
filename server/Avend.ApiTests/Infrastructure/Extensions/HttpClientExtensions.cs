using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Avend.API.Infrastructure.Responses;

using FluentAssertions;

using HtmlAgilityPack;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

namespace Avend.ApiTests.Infrastructure.Extensions
{
    public static class HttpClientExtensions
    {
        public static HttpJsonOperation GetJsonAsync(this HttpClient client, string uri)
        {
            return new HttpJsonOperation
            {
                Task = client.GetAsync(uri)
            };
        }

        public static HttpJsonOperation PostJsonAsync<TReq>(this HttpClient client, string uri, TReq request)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            return SendJsonAsync(client, requestMessage, request);
        }

        public static HttpJsonOperation PutJsonAsync<TReq>(this HttpClient client, string uri, TReq request)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, uri);
            return SendJsonAsync(client, requestMessage, request);
        }

        public static HttpJsonOperation PatchJsonAsync<TReq>(this HttpClient client, string uri, TReq request)
        {
            var requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), uri);
            return SendJsonAsync(client, requestMessage, request);
        }

        public static HttpJsonOperation DeleteJsonAsync<TReq>(this HttpClient client, string uri, TReq request)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);
            return SendJsonAsync(client, requestMessage, request);
        }

        public static HttpJsonOperation DeleteJsonAsync(this HttpClient client, string uri)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);
            return SendJsonAsync(client, requestMessage, "");
        }

        public static HttpJsonOperation SendJsonAsync<TReq>(this HttpClient client, HttpRequestMessage requestMessage, TReq request)
        {
            var jsonStr = JsonConvert.SerializeObject(request);
            requestMessage.Content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
            return client.SendDataAsync(requestMessage);
        }

        public static HttpJsonOperation SendDataAsync(this HttpClient client, HttpRequestMessage requestMessage)
        {
            return new HttpJsonOperation
            {
                Task = client.SendAsync(requestMessage)
            };
        }
    }

    public class HttpJsonOperation
    {
        public Task<HttpResponseMessage> Task { get; set; }
        private const string ResponseMessage = "\nrequest failed with '{0}: {1}'\n{2}";
        public async Task<HttpResponseMessage> Response(HttpStatusCode status = HttpStatusCode.OK, string message = null, params object[] parameters)
        {
            var response = await Task;
            var body = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                var document = new HtmlDocument();
                document.LoadHtml(body);
                var bodyElement = document.DocumentNode.Element("html")?.Element("body");                
                var title = bodyElement?.Element("h1")?.InnerText ?? "";
                var stackTrace = document.GetElementbyId("stackpage")?.InnerText;
                stackTrace = Regex.Replace(stackTrace, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
                body = $"{title}\n{stackTrace}";
            }

            var requestDetails = string.Format(ResponseMessage, (int)response.StatusCode, response.ReasonPhrase, body);
            message = string.Format(message ?? "", parameters);
            Assert.AreEqual(status, response.StatusCode, message + requestDetails);
            return response;
        }

        public async Task<T> Response<T>(HttpStatusCode status = HttpStatusCode.OK, string message = null, params object[] parameters)
        {
            var response = await Response(status, message, parameters);
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<bool> AvendEmptyResponse(HttpStatusCode code = HttpStatusCode.OK, string format = null, params object[] formatParams)
        {
            var response = await Response<OkResponseEmpty>(code, format, formatParams);

            response.Should()
                .NotBeNull("Unexpected response received from test server");

            return response.Success;
        }

        public async Task<List<Error>> AvendErrorResponse(HttpStatusCode code = HttpStatusCode.BadRequest, string format = null, params object[] formatParams)
        {
            var response = await Response<ErrorResponse>(code, format, formatParams);

            response.Should()
                .NotBeNull("Unexpected response received from test server");

            response.Success.Should()
                .BeFalse("Expected error response from test server but received success");

            return response.Errors;
        }

        public async Task<T> AvendResponse<T>(HttpStatusCode code = HttpStatusCode.OK, string format = null, params object[] formatParams)
        {
            var response = await Response<OkResponse<T>>(code, format, formatParams);

            response.Should()
                .NotBeNull("Unexpected response received from test server");

            response.Success.Should()
                .BeTrue("Expected success response from test server but received error");

            return response.Data;
        }

        public async Task<List<T>> AvendListResponse<T>(int expectedCount = -1, HttpStatusCode code = HttpStatusCode.OK, string format = null, params object[] formatParams)
        {
            var response = await Response<OkListResponse<T>>(code, format, formatParams);
            if (expectedCount >= 0)
            {
                var message = format ?? $"response expected to have exactly {expectedCount} total records";

                response.TotalFilteredRecords.Should()
                    .Be(expectedCount, message, formatParams);
            }

            return response.Data;
        }        
    }
}