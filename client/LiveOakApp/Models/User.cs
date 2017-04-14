using System;
using System.Collections.Generic;
using Microsoft.Identity.Client;
using SL4N;
using ServiceStack;

namespace LiveOakApp.Models
{
    public class User
    {
        static readonly ILogger LOG = LoggerFactory.GetLogger<User>();

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string DefaultName { get; private set; }

        public string FullName
        {
            get
            {
                var result = string.Join(" ", new List<string> { FirstName, LastName });
                if (result.Trim().IsNullOrEmpty())
                {
                    return DefaultName;
                }
                return result;
            }
        }

        public User(TokenCacheItem cacheItem)
        {
            if (!ParseNamesFromToken(cacheItem.Token))
            {
                DefaultName = DefaultName ?? cacheItem.Name;
            }
        }

        public User(AuthenticationResult authResult)
        {
            if (!ParseNamesFromToken(authResult.Token))
            {
                DefaultName = DefaultName ?? authResult.User?.Name;
            }
        }

        bool ParseNamesFromToken(string jwtToken)
        {
            try
            {
                var data = JwtToken.Parse(jwtToken);
                FirstName = data.GivenName;
                LastName = data.FamilyName;
                DefaultName = data.Name;
                return true;
            }
            catch (Exception error)
            {
                LOG.Error("failed to parse JWT", error);
            }
            return false;
        }
    }
}
