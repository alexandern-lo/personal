using System;
using System.Net.Http;

namespace Avend.Admin
{
    public class AdminAppException : Exception
    {
        public string Body { get; }
        public HttpResponseMessage Response { get; }

        public AdminAppException(HttpResponseMessage response, string body)
        {
            Response = response;
            Body = body;
        }
    }
}