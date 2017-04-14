using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ServiceStack.Text;
using SL4N;
using StudioMobile;
using LiveOakApp.Resources;
using LiveOakApp.Models.Data;
using LiveOakApp.Models.Data.NetworkDTO;

#if __ANDROID__
using Android.App;
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
#endif

namespace LiveOakApp.Models.Services
{
    public class ApiService
    {
        static readonly ILogger LOG = LoggerFactory.GetLogger<ApiService>();

        const string RootUrl = ApplicationConfig.ApiRootUrl;

        const string ServiceUrl = RootUrl + "api/v1/";

        readonly JsonService JsonService;
        readonly AuthService AuthService;

        public ApiService(JsonService jsonService, AuthService authService)
        {
            JsonService = jsonService;
            AuthService = authService;
            Debug.Assert(JsonService != null, "JsonService must not be null");
            Debug.Assert(AuthService != null, "AuthService must not be null");

            LOG.Debug(string.Format("ServiceUrl {0}", ServiceUrl));

            RemoteImage.HttpClientBuilder = CreateHttpClient;
        }

        public HttpClient CreateHttpClient()
        {
            return new HttpClient();
        }

        async Task<ApiResult<TResult>> ApiCall<TResult>(HttpRequestMessage request, string previousETag, CancellationToken? cancellationToken, Func<string, ResponseWithDataDTO<TResult>> converter)
        {
            if (AuthService.IsAuthTokenExpired)
            {
                LOG.Debug("Auth token expired, attempting SilentLogin");
                await AuthService.SilentLogin();
                LOG.Debug("SilentLogin successful");
            }
            var retryRequest = await CloneHttpRequestMessageAsync(request);
            try
            {
                return await internalApiCall(request, previousETag, cancellationToken, converter);
            }
            catch (AuthException authError)
            {
                LOG.Warn("== Failed to execure request, attemping SilentLogin ({0})", authError.Message);
                await AuthService.SilentLogin();
                LOG.Debug("SilentLogin successful");
                return await internalApiCall(retryRequest, previousETag, cancellationToken, converter);
            }
        }

        async Task<ApiResult<TResult>> internalApiCall<TResult>(HttpRequestMessage request, string previousETag, CancellationToken? cancellationToken, Func<string, ResponseWithDataDTO<TResult>> converter)
        {
            using (var client = CreateHttpClient())
            {
                var totalWatch = Stopwatch.StartNew();

                client.BaseAddress = new Uri(ServiceUrl);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Add("X-UserLocale", L10n.CurrentLocale());

                if (!string.IsNullOrEmpty(previousETag))
                {
                    EntityTagHeaderValue previousEtagEntity;
                    if (EntityTagHeaderValue.TryParse(previousETag, out previousEtagEntity))
                    {
                        client.DefaultRequestHeaders.IfNoneMatch.Add(previousEtagEntity);
                    }
                }

                if (AuthService.IsLoggedIn)
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthService.AuthToken);
                }

                var reqId = new Random().Next(100);
                LOG.Trace("---------------------------------");
                LOG.Trace(">> {0} ApiCall Started [{1} {2}] etag:'{3}' ({4})", reqId, request.Method, request.RequestUri, previousETag, request.Headers);
                LOG.Trace(">> {0} ApiCall Headers: ({1}), ({2})", reqId, client.DefaultRequestHeaders, request.Headers);
#if DEBUG
                var jsonContent = request.Content as JsonContent;
                if (jsonContent != null)
                {
                    var jsonContentString = await jsonContent.ReadAsStringAsync();
                    LOG.Trace(">> {0} ApiCall Content: {1}", reqId, jsonContentString?.Truncate(2000));
                }
#endif
                LOG.Trace("----------------");

                HttpResponseMessage response = null;
                string responseString;

                try
                {
                    response = await Task.Run(() => client.SendAsync(request, cancellationToken ?? CancellationToken.None));
                    if (response.StatusCode == HttpStatusCode.NotModified)
                    {
                        LOG.Trace("----------------");
                        LOG.Trace("<< {0} ApiResult {1} {2} ({3};{4})", reqId, (int)response.StatusCode, response.StatusCode, response.Headers, response.Content?.Headers);
                        totalWatch.Stop();
                        LOG.Trace("<< {0} Execution time: {1}", reqId, totalWatch.Elapsed.ToSpanString());
                        LOG.Trace("---------------------------------");
                        return new ApiResult<TResult>(default(ResponseWithDataDTO<TResult>), null, ApiResultStatus.NotModified);
                    }

                    responseString = await response.Content.ReadAsStringAsync();
#if DEBUG
                    LOG.Trace("<< {0} Api responseString: {1}", reqId, responseString.Truncate(2000));
#endif
                    CheckResponseForErrors(response, responseString);
                }
                catch (Exception e)
                {
                    LOG.Debug("---------------------------------");
                    LOG.Debug(">> {0} ApiCall Failed [{1} {2}] etag:{3} ({4})", reqId, request.Method, request.RequestUri, previousETag, request.Headers);
                    LOG.Debug("----------------");
                    if (response != null)
                        LOG.Debug("<< {0} ApiResult {1} {2} Error {3} ({4};{5})", reqId, (int)response.StatusCode, response.StatusCode, response.Headers, response.Content?.Headers, e);
                    else
                        LOG.Debug("<< {0} ApiResult Exception ({1})", reqId, e);
                    if (e is MultipleException)
                    {
                        var multipleException = (MultipleException)e;
                        foreach (var exception in multipleException.Exceptions)
                        {
                            LOG.Debug("<< {0} ApiResult Inner Error ({1})", reqId, exception);
                        }
                    }
                    totalWatch.Stop();
                    LOG.Debug("<< {0} Execution time: {1}", reqId, totalWatch.Elapsed.ToSpanString());
                    LOG.Debug("---------------------------------");
                    RaiseApiExceptionReceived(e);
                    throw e;
                }

                var jsonWatch = Stopwatch.StartNew();
                var dto = await Task.Run(() => converter(responseString));
                jsonWatch.Stop();
                LOG.Trace("<< json parsed with thread overhead in: {0}", jsonWatch.Elapsed.ToSpanString());

                var etag = response.Headers.ETag?.ToString();

                LOG.Trace("----------------");
                var dtoList = dto != null ? dto.Data as ICollection : null;
                var dtoListCount = dtoList != null ? string.Format("count: {0}", dtoList.Count) : "";
                var contentLenghtNumber = response.Content?.Headers?.ContentLength;
                var contentLenght = contentLenghtNumber.HasValue ? string.Format("length: {0}", contentLenghtNumber) : "";
                LOG.Trace("<< {0} ApiResult {1} {2} ({3};{4}) result:{5} {6} {7}", reqId, (int)response.StatusCode, response.StatusCode, response.Headers, response.Content?.Headers, dto, dtoListCount, contentLenght);
                totalWatch.Stop();
                LOG.Trace("<< {0} Execution time: {1}", reqId, totalWatch.Elapsed.ToSpanString());
                LOG.Trace("---------------------------------");
                return new ApiResult<TResult>(dto, etag, ApiResultStatus.Ok);
            }
        }

        public static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage req)
        {
            // TODO: wrap request creation in a closure instead?
            var clone = new HttpRequestMessage(req.Method, req.RequestUri);

            var stream = new MemoryStream();
            if (req.Content != null)
            {
                await req.Content.CopyToAsync(stream).ConfigureAwait(false);
                stream.Position = 0;
                clone.Content = new StreamContent(stream);

                // Copy the content headers
                if (req.Content.Headers != null)
                    foreach (var h in req.Content.Headers)
                        clone.Content.Headers.Add(h.Key, h.Value);
            }

            clone.Version = req.Version;

            foreach (KeyValuePair<string, object> prop in req.Properties)
                clone.Properties.Add(prop);

            foreach (KeyValuePair<string, IEnumerable<string>> header in req.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return clone;
        }

        async Task<ApiResult<T>> ApiCall<T>(HttpRequestMessage request, string previousETag, CancellationToken? cancellationToken)
        {
            return await ApiCall(request, previousETag, cancellationToken, DeserializeResult<ResponseWithDataDTO<T>>);
        }

        public TResult DeserializeResult<TResult>(string responseString)
        {
            var jsonWatch = Stopwatch.StartNew();
            try
            {

                if (responseString == null)
                    return default(TResult);
                return JsonService.Deserialize<TResult>(responseString);

            }
            finally
            {
                jsonWatch.Stop();
                LOG.Trace("<< json parsed in: {0}", jsonWatch.Elapsed.ToSpanString());
            }
        }

        void CheckResponseForErrors(HttpResponseMessage response, string content)
        {
            Exception exception = null;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new AuthException(response.StatusCode.ToString());
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                exception = new ApiRoutingError(response.StatusCode.ToString());
            }
            else if ((int)response.StatusCode >= 400)
            {
                exception = new ApiServerError(response.StatusCode.ToString());
            }

            bool contentIsJson =
                response.Content.Headers.ContentType?.MediaType == "application/json" &&
                !string.IsNullOrEmpty(content) &&
                content[0] == '{';
            if (contentIsJson)
            {
                var jsonObject = JsonService.Deserialize<JsonObject>(content);
                if (jsonObject != null)
                {
                    if (exception == null && !jsonObject.Get<bool>("success"))
                    {
                        exception = new ApiServerError();
                    }

                    var errors = jsonObject.Get<List<JsonObject>>("errors");
                    if (errors != null && errors.Count > 0)
                    {
                        var exceptionList = new List<Exception>();
                        foreach (var error in errors)
                        {
                            exceptionList.Add(errorToException(error));
                        }
                        exception = new MultipleException(exceptionList);
                    }
                }
            }

            if (exception != null)
            {
                throw exception;
            }
        }

        Exception errorToException(JsonObject error)
        {
            var errorCode = error.Get<string>("code");
            var errorMessage = error.Get<string>("message");
            var fields = error.ContainsKey("fields") ? error.Get<List<string>>("fields") : new List<string>();

            // TODO: handle other errors
            switch (errorCode)
            {
                case "access_denied":
                    return new AccessDeniedError(errorMessage);
                case "subscription_expired":
                    return new SubscriptionExpiredError(errorMessage);
                case "not_found":
                    if (fields.Contains("lead_uid"))
                        return new LeadNotFoundError(errorMessage);
                    if (fields.Contains("event_uid"))
                        return new EventNotFoundError(errorMessage);
                    return new EntityNotFoundError(errorMessage);
                case "rejected_old_data":
                    var editedDate = error.Get<DateTime?>("client_updated_at");
                    return new ServerHasNewerLeadError(editedDate);
                default:
                    return new ApiServerError(string.Format("{0} {1}", errorCode, errorMessage));
            }
        }

        #region Profile

        async public Task<ApiResult<UserProfileDTO>> GetProfile(string etag, CancellationToken? cancellationToken)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "users/profile");
            return await ApiCall<UserProfileDTO>(requestMessage, etag, cancellationToken);
        }

        #endregion

        #region TermsOfUse

        async public Task<ApiResult<TermsOfUseDTO>> GetTerms(string etag, CancellationToken? cancellationToken)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "users/terms/latest");
            return await ApiCall<TermsOfUseDTO>(requestMessage, etag, cancellationToken);
        }

        async public Task<ApiResult<string>> AcceptTerms(string termsUid, CancellationToken? cancellationToken)
        {
            var uri = string.Format("users/terms/{0}/accept", termsUid);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            return await ApiCall<string>(requestMessage, null, cancellationToken);
        }

        #endregion

        #region Events

        async public Task<ApiResult<List<EventDTO>>> GetEvents(int perPage, string etag, CancellationToken? cancellationToken)
        {
            // hack for events paginaton. Simulate one events request instead of several events page requests.
            return await SimulateOneRequestForPages<EventDTO>("events?", perPage, etag, cancellationToken);
            // end of hack.
        }

        async public Task<ApiResult<List<EventDTO>>> GetSelectableEvents(int perPage, string etag, CancellationToken? cancellationToken)
        {
            // hack for events paginaton. Simulate one events request instead of several events page requests.
            // scope = 1 => returns selectable events, events which may be used in lead creation.
            return await SimulateOneRequestForPages<EventDTO>("events?scope=1&", perPage, etag, cancellationToken);
            // end of hack.
        }

        async public Task<ApiResult<EventDTO>> GetEvent(string eventUid, string etag, CancellationToken? cancellationToken)
        {
            var uri = string.Format("jobs/{0}", eventUid);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            return await ApiCall<EventDTO>(requestMessage, etag, cancellationToken);
        }

        #endregion

        #region Resources

        async public Task<ApiResult<List<ResourceDTO>>> GetResources(string etag, CancellationToken? cancellationToken)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "users/resources");
            return await ApiCall<List<ResourceDTO>>(requestMessage, etag, cancellationToken);
        }

        #endregion

        #region Attendees

        async public Task<ApiResult<List<AttendeeDTO>>> SearchAttendees(string eventUid, AttendeeFilterDTO filter, string sortBy, int pageNumber, int pageSize, string etag, CancellationToken? cancellationToken)
        {
            var uri = string.Format("events/{0}/attendees/filter?sort_field={1}&page={2}&per_page={3}", eventUid, sortBy, pageNumber, pageSize);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new JsonContent(filter, JsonService)
            };
            return await ApiCall<List<AttendeeDTO>>(requestMessage, etag, cancellationToken);
        }

        #endregion

        #region AgendaItems

        async public Task<ApiResult<List<AgendaItemDTO>>> GetAgendaItems(string eventUid, string etag, CancellationToken? cancellationToken)
        {
            var uri = string.Format("events/{0}/agenda_items", eventUid);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            return await ApiCall<List<AgendaItemDTO>>(requestMessage, etag, cancellationToken);
        }

        #endregion

        #region Leads

        async public Task<ApiResult<List<LeadDTO>>> GetLeads(int page, int perPage, string etag, CancellationToken? cancellationToken)
        {
            // hack for leads paginaton. Simulate one leads request instead of several leads page requests.
            return await SimulateOneRequestForPages<LeadDTO>("leads/own?sort_field=first_name&", perPage, etag, cancellationToken);
            // end of hack.
        }

        async public Task<ApiResult<LeadDTO>> CreateLead(LeadDTO leadDTO, CancellationToken? cancellationToken)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "leads")
            {
                Content = new JsonContent(leadDTO, JsonService)
            };
            return await ApiCall<LeadDTO>(requestMessage, null, cancellationToken);
        }

        async public Task<ApiResult<LeadDTO>> UpdateLead(string leadUID, LeadDTO leadDTO, CancellationToken? cancellationToken)
        {
            var uri = string.Format("leads/{0}", leadUID);
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, uri)
            {
                Content = new JsonContent(leadDTO, JsonService)
            };
            return await ApiCall<LeadDTO>(requestMessage, null, cancellationToken);
        }

        async public Task<ApiResult<string>> DeleteLead(string leadUID, CancellationToken? cancellationToken)
        {
            var uri = string.Format("leads/{0}", leadUID);
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);
            return await ApiCall<string>(requestMessage, null, cancellationToken);
        }

        async public Task<ApiResult<LeadsExportReportDTO>> ExportLeadToCRM(string leadUID, CancellationToken? cancellationToken)
        {
            var exportDTO = new LeadsExportDTO
            {
                Uids = new List<string> { leadUID }
            };
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "leads/export/crm")
            {
                Content = new JsonContent(exportDTO, JsonService)
            };
            return await ApiCall<LeadsExportReportDTO>(requestMessage, null, cancellationToken);
        }

        async public Task<ApiResult<LeadsExportReportDTO>> ExportLeadsToCRM(List<string> leadsUIDs, CancellationToken? cancellationToken)
        {
            var exportDTO = new LeadsExportDTO
            {
                Uids = leadsUIDs
            };
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "leads/export/crm")
            {
                Content = new JsonContent(exportDTO, JsonService)
            };
            return await ApiCall<LeadsExportReportDTO>(requestMessage, null, cancellationToken);
        }

        #endregion

        #region LeadsRecentActivity

        async public Task<ApiResult<List<LeadRecentActivityDTO>>> GetLeadsRecentActivity(int limit, string etag, CancellationToken? cancellationToken)
        {
            var uri = string.Format("leads/recent_activity?limit={0}", limit);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            return await ApiCall<List<LeadRecentActivityDTO>>(requestMessage, etag, cancellationToken);
        }

        #endregion

        #region File Uploads

        async public Task<ApiResult<string>> CreateUploadTokenUri(string fileName, CancellationToken? cancellationToken)
        {
            var uri = string.Format("users/resources/upload_token/{0}", fileName);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            return await ApiCall<string>(requestMessage, null, cancellationToken);
        }

        async public Task<string> UploadFile(string tokenUri, string absoluteFilePath, CancellationToken? cancellationToken)
        {
            LOG.Trace("Started uploading file {0}", tokenUri);
            using (var client = CreateHttpClient())
            using (var streamContent = new StreamContent(File.OpenRead(absoluteFilePath)))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("x-ms-blob-type", "BlockBlob");
                var response = await client.PutAsync(tokenUri, streamContent, cancellationToken ?? CancellationToken.None);
                var responseString = await response.Content.ReadAsStringAsync();
                try
                {
                    CheckResponseForErrors(response, responseString);
                }
                catch (Exception error)
                {
                    LOG.Error("UploadFile failed", error);
                    throw;
                }
                LOG.Trace("Finished uploading file {0}", tokenUri);
                return tokenUri.Split('?').First();
            }
        }

        #endregion

        #region File Uploads

        async public Task<byte[]> DownloadSmallFileToMemory(string absoluteUriToFile, CancellationToken? cancellationToken)
        {
            LOG.Trace("Started downloading small file {0}", absoluteUriToFile);
            using (var client = CreateHttpClient())
            {
                byte[] responseByteArray;
                client.MaxResponseContentBufferSize = 1024 * 1000;
                try
                {
                    var response = await client.GetAsync(absoluteUriToFile, cancellationToken ?? CancellationToken.None);
                    responseByteArray = await response.Content.ReadAsByteArrayAsync();
                }
                catch (Exception error)
                {
                    LOG.Error("Download small file failed", error);
                    throw;
                }
                LOG.Trace("Finished downloading small file {0}", absoluteUriToFile);
                return responseByteArray;
            }
        }

        #endregion

        #region Dashboard

        async public Task<ApiResult<DashboardDTO>> GetDashboard(CancellationToken? cancellationToken, int resourcesLimit, int eventsLimit)
        {
            var uri = string.Format("dashboard?events_limit={0}&resources_limit={1}", eventsLimit, resourcesLimit);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            return await ApiCall<DashboardDTO>(requestMessage, null, cancellationToken);
        }

        async public Task<ApiResult<string>> SetNewEventGoal(EventUserGoalDTO eventUserGoal, CancellationToken? cancellationToken)
        {
            var uri = string.Format("events/{0}/goals", eventUserGoal.EventUid);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new JsonContent(eventUserGoal, JsonService)
            };
            return await ApiCall<string>(requestMessage, null, cancellationToken);
        }

        async public Task<ApiResult<string>> AddExpense(EventExpenseDTO eventExpenseDTO, CancellationToken? cancellationToken)
        {
            var uri = string.Format("events/{0}/expenses", eventExpenseDTO.EventUid);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new JsonContent(eventExpenseDTO, JsonService)
            };
            return await ApiCall<string>(requestMessage, null, cancellationToken);
        }
        #endregion

        #region GetEventTotalExpenses

        async public Task<ApiResult<MoneyDTO>> GetTotalExpenses(string eventUID, CancellationToken? cancellationToken)
        {
            var uri = string.Format("events/{0}/expenses_total", eventUID);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            return await ApiCall<MoneyDTO>(requestMessage, null, cancellationToken);
        }

        #endregion

        #region track resources

        async public Task<ApiResult<List<string>>> SendResourceSentTrackingRequest(List<Guid> resourceUIDs, CancellationToken? cancellationToken)
        {
            var uri = "users/resources/track_sending";
            var resourcesSentTrackingReportDTO = new ResourcesSentTrackingRequestDTO
            {
                Uids = resourceUIDs
            };
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new JsonContent(resourcesSentTrackingReportDTO, JsonService)
            };
            return await ApiCall<List<string>>(requestMessage, null, cancellationToken);
        }

        #endregion

        #region pagination hack

        async Task<ApiResult<List<T>>> SimulateOneRequestForPages<T>(string endpoint, int perPage, string etag, CancellationToken? cancellationToken)
        {
            var allData = new List<T>();
            List<T> responseData;
            var currentPage = 0;
            ApiResult<List<T>> lastResponse;
            do
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, string.Format("{0}page={1}&per_page={2}", endpoint, currentPage++, perPage));
                lastResponse = await ApiCall<List<T>>(requestMessage, etag, cancellationToken);
                responseData = lastResponse.Content;
                allData.AddRange(responseData);
            } while (responseData.Count == perPage);
            var simulatedResponse = new ApiResult<List<T>>(lastResponse.Response, etag, lastResponse.Status);
            simulatedResponse.Content = allData;
            return simulatedResponse;
        }

        #endregion

        #region Exception handling

        public event EventHandler<ExceptionEventArgs> ApiExceptionReceived;

        void RaiseApiExceptionReceived(Exception error)
        {
            var args = new ExceptionEventArgs(error);
            ApiExceptionReceived.Invoke(this, args);
        }

        public class ExceptionEventArgs : EventArgs
        {
            public Exception exception;

            public ExceptionEventArgs(Exception exception)
            {
                this.exception = exception;
            }
        }

        #endregion
    }

    enum ApiErrorCode
    {
        RoutingError = 100,
    }

    public enum ApiResultStatus
    {
        Ok,
        NotModified
    }

    public class JsonContent : StringContent
    {
        public JsonContent(string content) : this(content, Encoding.UTF8)
        {
        }

        public JsonContent(string content, Encoding encoding) : base(content, encoding, "application/json")
        {
        }

        public JsonContent(object jsonObject, JsonService jsonService) : this(jsonService.Serialize(jsonObject))
        {
        }
    }

    public class ApiResult<T>
    {
        public ApiResult(ResponseWithDataDTO<T> response, string eTag, ApiResultStatus status)
        {
            Response = response;
            Content = response.Data;
            ETag = eTag;
            Status = status;
        }

        public static ApiResult<T> Create(ResponseWithDataDTO<T> response, string etag, ApiResultStatus status)
        {
            return new ApiResult<T>(response, etag, status);
        }

        public ResponseWithDataDTO<T> Response { get; set; }

        public T Content { get; set; }

        public string ETag { get; set; }

        public ApiResultStatus Status { get; set; }
    }
}
