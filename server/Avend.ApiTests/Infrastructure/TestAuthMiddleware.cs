using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.Logging;

using Qoden.Validation;

namespace Avend.ApiTests.Infrastructure
{
    public class TestAuthMiddleware : AuthenticationMiddleware<FakeActiveDirectory>
    {
        public TestAuthMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            FakeActiveDirectory options)
            : base(next, Microsoft.Extensions.Options.Options.Create(options), loggerFactory, encoder)
        {
            Assert.Argument(next, nameof(next)).NotNull();
            Assert.Argument(loggerFactory, nameof(loggerFactory)).NotNull();
            Assert.Argument(encoder, nameof(encoder)).NotNull();
            Assert.Argument(options, nameof(options)).NotNull();
            
            AuthenticationScheme = "Bearer";
        }

        /// <summary>
        /// Called by the AuthenticationMiddleware base class to create a per-request handler. 
        /// </summary>
        /// <returns>A new instance of the request handler</returns>
        protected override AuthenticationHandler<FakeActiveDirectory> CreateHandler()
        {
            return new TestAuthHandler();
        }
    }

    public class FakeActiveDirectory : AuthenticationOptions
    {
        public FakeActiveDirectory()
        {
            AuthenticationScheme = "Bearer";
            AutomaticAuthenticate = true;
            AutomaticChallenge = true;
            Principals = new Dictionary<string, IEnumerable<Claim>>();
        }

        public Dictionary<string, IEnumerable<Claim>> Principals { get; }

        public void AddUser(string token, IEnumerable<Claim> claims)
        {
            Principals.Add(token, claims);
        }


        public void AddUser(string token, params Claim[] claims)
        {
            if (!Principals.ContainsKey(token))
            {
                Principals.Add(token, claims);
            }
        }

        public void Clear()
        {
            Principals.Clear();
        }
    }

    public class TestAuthHandler : AuthenticationHandler<FakeActiveDirectory>
    {
#pragma warning disable 1998
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
#pragma warning restore 1998
        {
            string token = null;
            string authorization = Request.Headers["Authorization"];
            // If no authorization header found, nothing to process further
            if (string.IsNullOrEmpty(authorization))
            {
                return AuthenticateResult.Skip();
            }
            if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = authorization.Substring("Bearer ".Length).Trim();
            }
            // If no token found, no further work possible
            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.Skip();
            }

            var principals = Options.Principals;
            if (!principals.ContainsKey(token))
            {
                return AuthenticateResult.Fail("No principal registered for token: " + token);
            }
            var claims = principals[token];
            //Specify any value as second ClaimsIdentity parameter
            //See here - http://stackoverflow.com/questions/20254796/why-is-my-claimsidentity-isauthenticated-always-false-for-web-api-authorize-fil
            var identity = new ClaimsIdentity(claims, "test auth type")
            {
                Label = token
            };
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(
                principal,
                new AuthenticationProperties(),
                Options.AuthenticationScheme);
            return AuthenticateResult.Success(ticket);
        }
    }

    public static class TestAuthAppBuilderExtensions
    {
        public static IApplicationBuilder UseTestAuthentication(this IApplicationBuilder app)
        {
            Assert.Argument(app, nameof(app)).NotNull();
            return app.UseMiddleware<TestAuthMiddleware>();
        }
    }
}