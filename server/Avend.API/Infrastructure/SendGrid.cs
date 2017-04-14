using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Avend.API.Infrastructure.Logging;
using Avend.API.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qoden.Validation;

namespace Avend.API.Infrastructure
{
    public class SendgridConfiguration
    {
        public string ApiKey { get; set; }
        public string From { get; set; }
    }

    public interface ISendGrid
    {
        Task<bool> Send(string message, string subject, Func<string, string> linkGenerator,
            IEnumerable<SubscriptionInvite> invites);
    }

    public class SendGrid : ISendGrid
    {
        public static readonly Uri SendgridApiUrl = new Uri("https://api.sendgrid.com/v3/");

        private readonly IOptions<SendgridConfiguration> _sendgridConfig;
        private readonly ILogger _logger;

        public SendGrid(IOptions<SendgridConfiguration> sendgridConfig)
        {
            Assert.Argument(sendgridConfig, nameof(sendgridConfig)).NotNull();
            _sendgridConfig = sendgridConfig;
            _logger = AvendLog.CreateLogger(nameof(SendGrid));
        }

        public async Task<bool> Send(
            string message,
            string subject,
            Func<string, string> linkGenerator,
            IEnumerable<SubscriptionInvite> invites)
        {
            Assert.Argument(message, nameof(message)).NotEmpty();
            Assert.Argument(subject, nameof(subject)).NotEmpty();

            using (var http = new HttpClient())
            {
                http.BaseAddress = SendgridApiUrl;
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                    _sendgridConfig.Value.ApiKey);

                var personalizations = invites.Select(invite =>
                {
                    var link = linkGenerator(invite.InviteCode);
                    return new
                    {
                        to = new[] { new { email = invite.Email } },
                        subject,
                        substitutions = new JObject
                        {
                            {"%invite_link%", link}
                        }
                    };
                });

                var messageText = string.Format("{0}\n Please follow this link to signup - %invite_link%",
                    message);
                var sendGridMessage = new
                {
                    personalizations,
                    from = new
                    {
                        email = _sendgridConfig.Value.From
                    },
                    content = new[]
                    {
                        new
                        {
                            type = "text/plain",
                            value = messageText
                        }
                    }
                };

                var json = JsonConvert.SerializeObject(sendGridMessage);
                var response = await http.PostAsync("mail/send",
                    new StringContent(json, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogDebug("Response\n{header}\n\n{response:l}", response, responseBody);
                }

                return response.IsSuccessStatusCode;
            }
        }
    }
}