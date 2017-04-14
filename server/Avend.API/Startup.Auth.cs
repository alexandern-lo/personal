using System;
using Avend.API.Middleware;
using Avend.API.Model.NetworkDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Avend.API
{
    public partial class Startup
    {
        public const string SubscriptionAdminPolicy = "SubscriptionAdmin";
        public const string AdmininsPolicy = "AdminsPolicy";

        protected virtual void AddAuthServices(IServiceCollection services, IMvcCoreBuilder mvcBuilder)
        {
            mvcBuilder.AddAuthorization(options =>
            {
                var basic = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes("Bearer")
                    .RequireAuthenticatedUser()
                    .Build();
                options.AddPolicy(SubscriptionAdminPolicy, policy =>
                {
                    policy
                        .Combine(basic)
                        .AddRequirements(new SubscriptionRequirement(UserRole.Admin));
                    policy
                        .Combine(basic)
                        .AddRequirements(new SubscriptionRequirement(UserRole.Admin, UserRole.SuperAdmin));
                });
                options.AddPolicy(AdmininsPolicy, policy =>
                {
                    policy
                        .Combine(basic)
                        .AddRequirements(new SubscriptionRequirement(UserRole.SuperAdmin));
                });

                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .Combine(basic)
                    .AddRequirements(new SubscriptionRequirement(UserRole.SeatUser, UserRole.Admin, UserRole.SuperAdmin))
                    .Build();
            });

            services.AddSingleton<IAuthorizationHandler, SubscriptionRequirementHandler>();
        }

        protected virtual void ConfigureAuthentication(IApplicationBuilder appBuilder)
        {
            var conf = Configuration.GetSection("Authentication").GetSection("AzureAD");
            var tenantUid = conf.GetSection("TenantUid").Value;
            var clientId = conf.GetSection("ClientId").Value;
            var policy = conf.GetSection("B2CPolicy").Value;
            var validateLifeTime = conf.GetSection("ValidateLifeTime").Value;
            //Note about Azure B2C policy
            //OpenID discovery URL returns different metadata  with and without p=xyz parameter.
            //Also for valid policies it always return same metadata. 
            //Below code relies on this behavior and assumes that no matter which policy is used it always get same OpenID metadata.
            //With such assumption it uses only one policy instead of configuring more complicated setup with multiple metadata URLs.
            //(averbin)
            var jwtOptions = new JwtBearerOptions
            {
                IncludeErrorDetails = true,
                RefreshOnIssuerKeyNotFound = true,
                AutomaticAuthenticate = true,
                AutomaticChallenge = false,
                Audience = clientId,
                MetadataAddress =
                    $"https://login.microsoftonline.com/{tenantUid}/.well-known/openid-configuration?p={policy}",
                TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateLifetime =
                        string.Equals(validateLifeTime, "false", StringComparison.OrdinalIgnoreCase),
                    ValidIssuers = new[] {$"https://login.microsoftonline.com/{tenantUid}/v2.0/"}
                }
            };

            appBuilder.UseJwtBearerAuthentication(jwtOptions);
        }
    }
}