using Microsoft.Extensions.Configuration;

namespace Recurly.AspNetCore.Configuration
{
    /// <summary>
    /// Defines apiKey, privateKey, and subdomain properties for web.config/app.config files.
    /// </summary>
    public class RecurlySection
    {
        private IConfigurationSection recurlySection;

        public RecurlySection(IConfigurationSection recurlySection)
        {
            this.recurlySection = recurlySection;
        }

        public static RecurlySection Current
        {
            get; set;
        }

        #region Properties

        /// <summary>
        /// API Key
        /// </summary>
        public string ApiKey
        {
            get { return recurlySection["ApiKey"]; }
            set { recurlySection["ApiKey"] = value; }
        }

        /// <summary>
        /// API Private Key for Recurly.js and Transparent Post API
        /// </summary>
        public string PrivateKey
        {
            get { return recurlySection["PrivateKey"]; }
            set { recurlySection["PrivateKey"] = value; }
        }

        /// <summary>
        /// Recurly Subdomain
        /// </summary>
        public string Subdomain
        {
            get { return recurlySection["Subdomain"]; }
            set { recurlySection["Subdomain"] = value; }
        }

        /// <summary>
        /// Default Page Size or limit to the number of results returned at a time
        /// </summary>
        public int PageSize
        {
            get { return int.Parse(recurlySection["PageSize"]); }
            set { recurlySection["PageSize"] = value.ToString(); }
        }
      
        #endregion
    }
}
