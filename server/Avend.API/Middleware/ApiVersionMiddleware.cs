using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Qoden.Validation;

namespace Avend.API.Middleware
{
    public class ApiVersionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _version = "development";

        public ApiVersionMiddleware(RequestDelegate next)
        {
            Assert.Argument(next, nameof(next)).NotNull();
            _next = next;

            var assembly = GetType().GetTypeInfo().Assembly;
            var info = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (info != null)
                _version = info.InformationalVersion;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add("X-Avend-Api-Version", _version);
            await _next(context);
        }
    }
}
