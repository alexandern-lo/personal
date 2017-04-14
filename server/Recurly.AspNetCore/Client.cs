using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Recurly.AspNetCore.Configuration;
using Recurly.AspNetCore.Extensions;

[assembly: InternalsVisibleTo("Recurly.Test")]

namespace Recurly.AspNetCore
{
    /// <summary>
    /// Class for the Recurly client library.
    /// </summary>
    internal class Client
    {
        // refactored all these settings for increased testability
        public Settings Settings { get; protected set; }

        private static Client instance;
        internal static Client Instance => instance ?? (instance = new Client(Settings.Instance));

        protected Client(Settings settings)
        {
            Settings = settings;
        }

        internal static void ChangeInstance(Client client)
        {
            instance = client;
        }

        internal void ApplySettings(Settings settings)
        {
            Settings = settings;
        }

        public enum HttpRequestMethod
        {
            /// <summary>
            /// Lookup information about an object
            /// </summary>
            Get,
            /// <summary>
            /// Create a new object
            /// </summary>
            Post,
            /// <summary>
            /// Update an existing object
            /// </summary>
            Put,
            /// <summary>
            /// Delete an object
            /// </summary>
            Delete
        }

        /// <summary>
        /// Delegate to read a raw HTTP response from the server.
        /// </summary>
        public delegate void ReadResponseDelegate(HttpWebResponse response);

        /// <summary>
        /// Delegate to read the XML response from the server.
        /// </summary>
        /// <param name="xmlReader"></param>
        public delegate void ReadXmlDelegate(XmlReader xmlReader);

        /// <summary>
        /// Reads paged XML responses from the server
        /// </summary>
        /// <param name="xmlReader"></param>
        /// <param name="records"></param>
        public delegate void ReadXmlListDelegate(XmlReader xmlReader, int records, string start, string next, string prev);

        /// <summary>
        /// Delegate to write the XML request to the server.
        /// </summary>
        /// <param name="xmlWriter"></param>
        public delegate void WriteXmlDelegate(XmlWriter xmlWriter);

        public Task<HttpStatusCode> PerformRequest(HttpRequestMethod method, string urlPath)
        {
            return PerformRequest(method, urlPath, null, null, null, null);
        }

        public Task<HttpStatusCode> PerformRequest(HttpRequestMethod method, string urlPath,
            ReadXmlDelegate readXmlDelegate)
        {
            return PerformRequest(method, urlPath, null, readXmlDelegate, null, null);
        }

        public Task<HttpStatusCode> PerformRequest(HttpRequestMethod method, string urlPath,
            WriteXmlDelegate writeXmlDelegate)
        {
            return PerformRequest(method, urlPath, writeXmlDelegate, null, null, null);
        }

        public Task<HttpStatusCode> PerformRequest(HttpRequestMethod method, string urlPath,
            WriteXmlDelegate writeXmlDelegate, ReadXmlDelegate readXmlDelegate)
        {
            return PerformRequest(method, urlPath, writeXmlDelegate, readXmlDelegate, null, null);
        }

        public Task<HttpStatusCode> PerformRequest(HttpRequestMethod method, string urlPath,
            ReadXmlListDelegate readXmlListDelegate)
        {
            return PerformRequest(method, urlPath, null, null, readXmlListDelegate, null);
        }

        public Task<HttpStatusCode> PerformRequest(HttpRequestMethod method, string urlPath,
            WriteXmlDelegate writeXmlDelegate, ReadResponseDelegate responseDelegate)
        {
            return PerformRequest(method, urlPath, writeXmlDelegate, null, null, responseDelegate);
        }

        protected virtual async Task<HttpStatusCode> PerformRequest(HttpRequestMethod method, string urlPath,
            WriteXmlDelegate writeXmlDelegate, ReadXmlDelegate readXmlDelegate, ReadXmlListDelegate readXmlListDelegate, ReadResponseDelegate reseponseDelegate)
        {
            var url = Settings.GetServerUri(urlPath);
#if (DEBUG)
            Console.WriteLine("Requesting " + method + " " + url);
#endif
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Accept = "application/xml";      // Tells the server to return XML instead of HTML
            request.ContentType = "application/xml; charset=utf-8"; // The request is an XML document
            //  request.Headers[HttpRequestHeader.TransferEncoding] = "identity";             // Send it all as one request
            request.Headers[HttpRequestHeader.UserAgent] = Settings.UserAgent;
            request.Headers[HttpRequestHeader.Authorization] = Settings.AuthorizationHeaderValue;
            request.Headers["X-Api-Version"] = Settings.RecurlyApiVersion;
            request.Method = method.ToString().ToUpper();

            if ((method == HttpRequestMethod.Post || method == HttpRequestMethod.Put) && (writeXmlDelegate != null))
            {
                // 60 second timeout -- some payment gateways (e.g. PayPal) can take a while to respond
                var cts = new CancellationTokenSource();
                cts.CancelAfter(60000);

                // Write POST/PUT body
                using (var requestStream = await Task.Run(() => request.GetRequestStreamAsync(), cts.Token))
                {
                    WritePostParameters(requestStream, writeXmlDelegate);
                }
            }
            else
            {
                request.Headers[HttpRequestHeader.ContentLength] = 0.ToString();
            }

            try
            {
                using (var response = (HttpWebResponse) await request.GetResponseAsync())
                {

                    ReadWebResponse(response, readXmlDelegate, readXmlListDelegate, reseponseDelegate);
                    return response.StatusCode;

                }
            }
            catch (WebException ex)
            {
                if (ex.Response == null) throw;

                var response = (HttpWebResponse)ex.Response;
                var statusCode = response.StatusCode;
                Error[] errors;

                Debug.WriteLine(String.Format("Recurly Library Received: {0} - {1}", (int)statusCode, statusCode));

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.Accepted:
                    case HttpStatusCode.Created:
                    case HttpStatusCode.NoContent:
                        ReadWebResponse(response, readXmlDelegate, readXmlListDelegate, reseponseDelegate);

                        return HttpStatusCode.NoContent;

                    case HttpStatusCode.NotFound:
                        errors = Error.ReadResponseAndParseErrors(response);
                        if (errors.Length > 0)
                            throw new NotFoundException(errors[0].Message, errors);
                        throw new NotFoundException("The requested object was not found.", errors);

                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden:
                        errors = Error.ReadResponseAndParseErrors(response);
                        throw new InvalidCredentialsException(errors);

                    case HttpStatusCode.PreconditionFailed:
                        errors = Error.ReadResponseAndParseErrors(response);
                        throw new ValidationException(errors);

                    case HttpStatusCode.ServiceUnavailable:
                        throw new TemporarilyUnavailableException();

                    case HttpStatusCode.InternalServerError:
                        errors = Error.ReadResponseAndParseErrors(response);
                        throw new ServerException(errors);
                }

                if ((int)statusCode == ValidationException.HttpStatusCode) // Unprocessable Entity
                {
                    errors = Error.ReadResponseAndParseErrors(response);
                    if (errors.Length > 0) Debug.WriteLine(errors[0].ToString());
                    else Debug.WriteLine("Client Error: " + response.ToString());
                    throw new ValidationException(errors);
                }

                throw;
            }
        }

        /// <summary>
        /// Used for downloading PDFs
        /// </summary>
        /// <param name="urlPath"></param>
        /// <param name="acceptType"></param>
        /// <param name="acceptLanguage"></param>
        /// <returns></returns>
        public virtual async Task<byte[]> PerformDownloadRequest(string urlPath, string acceptType, string acceptLanguage)
        {
            var url = Settings.GetServerUri(urlPath);

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = acceptType;
            request.ContentType = "application/xml; charset=utf-8"; // The request is an XML document
            //request.SendChunked = false;             // Send it all as one request
            request.Headers[HttpRequestHeader.UserAgent] = Settings.UserAgent;
            request.Headers[HttpRequestHeader.Authorization] = Settings.AuthorizationHeaderValue;
            request.Method = "GET";
            request.Headers[HttpRequestHeader.AcceptLanguage] = acceptLanguage;

            try
            {
                var r = (HttpWebResponse) await request.GetResponseAsync();
                byte[] pdf;
                var buffer = new byte[2048];
                if (!request.HaveResponse || r.StatusCode != HttpStatusCode.OK) return null;
                using (var ms = new MemoryStream())
                {
                    using (var reader = new BinaryReader(r.GetResponseStream(), Encoding.UTF8))
                    {
                        int bytesRead;
                        while ((bytesRead = reader.Read(buffer, 0, 2048)) > 0)
                        {
                            ms.Write(buffer, 0, bytesRead);
                        }
                    }
                    pdf = ms.ToArray();
                }
                return pdf;

            }
            catch (WebException ex)
            {
                if (ex.Response == null) throw;
                var response = (HttpWebResponse)ex.Response;
                var statusCode = response.StatusCode;
                Error[] errors;

                Debug.WriteLine(String.Format("Recurly Library Received: {0} - {1}", (int)statusCode, statusCode));

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.Accepted:
                    case HttpStatusCode.Created:
                    case HttpStatusCode.NoContent:

                        return null;

                    case HttpStatusCode.NotFound:
                        errors = Error.ReadResponseAndParseErrors(response);
                        if (errors.Length > 0)
                            throw new NotFoundException(errors[0].Message, errors);
                        throw new NotFoundException("The requested object was not found.", errors);

                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden:
                        errors = Error.ReadResponseAndParseErrors(response);
                        throw new InvalidCredentialsException(errors);

                    case HttpStatusCode.PreconditionFailed:
                        errors = Error.ReadResponseAndParseErrors(response);
                        throw new ValidationException(errors);

                    case HttpStatusCode.ServiceUnavailable:
                        throw new TemporarilyUnavailableException();

                    case HttpStatusCode.InternalServerError:
                        errors = Error.ReadResponseAndParseErrors(response);
                        throw new ServerException(errors);
                }

                if ((int)statusCode == ValidationException.HttpStatusCode) // Unprocessable Entity
                {
                    errors = Error.ReadResponseAndParseErrors(response);
                    throw new ValidationException(errors);
                }

                throw;
            }
        }

        protected virtual void ReadWebResponse(HttpWebResponse response, ReadXmlDelegate readXmlDelegate, ReadXmlListDelegate readXmlListDelegate, ReadResponseDelegate responseDelegate)
        {
            if (readXmlDelegate == null && readXmlListDelegate == null && responseDelegate == null) return;
            var responseStream = CopyAndClose(response.GetResponseStream());

            if (responseDelegate != null)
            {
                responseDelegate(response);
                return;
            }

            responseStream.Position = 0;
            using (var xmlReader = XmlReader.Create(responseStream))
            {
                // Check for pagination
                var records = -1;
                var cursor = string.Empty;
                string start = null;
                string next = null;
                string prev = null;

                if (null != response.Headers["X-Records"])
                {
                    Int32.TryParse(response.Headers["X-Records"], out records);
                }

                var link = response.Headers["Link"];

                if (!link.IsNullOrEmpty())
                {
                    start = link.GetUrlFromLinkHeader("start");
                    next = link.GetUrlFromLinkHeader("next");
                    prev = link.GetUrlFromLinkHeader("prev");
                }

                if (records >= 0) {
                    readXmlListDelegate(xmlReader, records, start, next, prev);
                }
                else if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    readXmlDelegate(xmlReader);
                }
                    
            }

        }

        protected virtual void WritePostParameters(Stream outputStream, WriteXmlDelegate writeXmlDelegate)
        {
            using (var xmlWriter = XmlWriter.Create(outputStream, new XmlWriterSettings() { Encoding = Encoding.UTF8, Indent = true, IndentChars = "  "}))
            {
                xmlWriter.WriteStartDocument();

                writeXmlDelegate(xmlWriter);

                xmlWriter.WriteEndDocument();
            }
#if (DEBUG)
            // Also copy XML to debug output
            Console.WriteLine("Sending Data:");
            var s = new MemoryStream();
            using (var xmlWriter = XmlWriter.Create(outputStream, new XmlWriterSettings() { Encoding = Encoding.UTF8, Indent = true, IndentChars = "  " }))
            {
                xmlWriter.WriteStartDocument();

                writeXmlDelegate(xmlWriter);

                xmlWriter.WriteEndDocument();
            }
            Console.WriteLine(Encoding.UTF8.GetString(s.ToArray()));
#endif

        }

        protected virtual MemoryStream CopyAndClose(Stream inputStream)
        {
            const int readSize = 256;
            var buffer = new byte[readSize];
            var ms = new MemoryStream();

            var count = inputStream.Read(buffer, 0, readSize);
            while (count > 0)
            {
                ms.Write(buffer, 0, count);
                count = inputStream.Read(buffer, 0, readSize);
            }

            ms.Position = 0;

            inputStream.Dispose();

            return ms;
        }

    }
}
