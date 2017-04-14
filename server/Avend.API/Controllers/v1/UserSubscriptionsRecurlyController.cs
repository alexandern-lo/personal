using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Avend.API.Infrastructure;
using Avend.API.Infrastructure.Responses;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Model.Recurly;
using Avend.API.Services;
using Avend.API.Services.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Recurly.AspNetCore;
using Recurly.AspNetCore.Configuration;

using Swashbuckle.SwaggerGen.Annotations;
using Qoden.Validation;

namespace Avend.API.Controllers.v1
{
    /// <summary>
    /// Responsible for processing of Recurly Webhooks of all kinds.
    /// Backed by the RecurlyService class to perform actual changes in the database.
    /// </summary>
    [Route("api/v1/users/subscriptions/recurly")]
    [Authorize]
    public class UserSubscriptionsRecurlyController : BaseController
    {
        public UserSubscriptionsRecurlyController(
            DbContextOptions<AvendDbContext> options, 
            RecurlyService recurlyService)
            : base(options)
        {
            Assert.Argument(recurlyService, "recurlyService").NotNull();
            RecurlyService = recurlyService;
        }

        public RecurlyService RecurlyService { get; }

        /// <summary>
        /// Returns the list of recurly plans. Detects 
        /// - individual / corporate plans
        /// - limit on the number of users for corporate plans
        /// - billing period
        /// - price
        /// - recurly URL where to redirect user for payment
        /// </summary>
        /// <remarks>Returns the whole plans list.</remarks>
        /// <response code="200">Json message encapsulating the array of records.</response>
        [HttpGet("plans")]
        [AllowAnonymous]
        [SwaggerOperation("GetPlansList")]
        [ProducesResponseType(typeof(OkListResponse<string>), 200)]
        public async Task<IActionResult> GetPlansList()
        {
            Logger.LogInformation("Started retrieving the plans list, most current section data is: " + JsonConvert.SerializeObject(RecurlySection.Current));

            var planList = Plans.List();

            await planList.RetrievalTask;

            var planCodes = new List<object>();

            foreach (var plan in planList)
            {
                Logger.LogInformation("Discovered plan with AccountingCode: '{0}'", plan.AccountingCode);

                planCodes.Add(SubscriptionPlanDTO.From(plan));
            }

            return Ok(new OkListResponse<object>()
            {
                TotalFilteredRecords = planCodes.Count,
                Data = planCodes,
            });
        }

        /// <summary>
        /// Processes WebHook request from Recurly. This endpoint can:
        /// - add subscription for a known user
        /// - record transaction for known subscription
        /// </summary>
        /// <remarks>Adds a new subscription based on webhook data from Recurly..</remarks>
        /// <response code="204">Subscription was created successfully</response>
        [HttpPost]
        [AllowAnonymous]
        [Consumes("application/xml")]
        [SwaggerOperation("ProcessRecurlyWebHook")]
        [ProducesResponseType(typeof(OkResponse<string>), 204)]
        public async Task<IActionResult> ProcessRecurlyWebHook()
        {
            try
            {
                Logger.LogInformation(HttpContext.TraceIdentifier + ": Started processing WebHook");
                Logger.LogDebug(HttpContext.TraceIdentifier + ": Body stream info: " + Request.Body);
                Logger.LogDebug(HttpContext.TraceIdentifier + ": Body stream CanRead: " + Request.Body.CanRead);
                Logger.LogDebug(HttpContext.TraceIdentifier + ": Body stream CanSeek: " + Request.Body.CanSeek);
                Logger.LogDebug(HttpContext.TraceIdentifier + ": Using memory stream...");

                var memoryStream = new MemoryStream();
                await Request.Body.CopyToAsync(memoryStream);
                Logger.LogTrace(HttpContext.TraceIdentifier + ": Memory stream: " + System.Text.Encoding.UTF8.GetString(memoryStream.ToArray()));
                memoryStream.Seek(0, SeekOrigin.Begin);
                Logger.LogDebug(HttpContext.TraceIdentifier + ": Seeking succeeded");

                using (var stream = new StreamReader(memoryStream))
                {
                    var header = stream.ReadLine();
                    if (!header.Contains("<?xml"))
                        throw new InvalidDataException("Proper XML document with <?xml...?> header is expected");
                    Logger.LogDebug(HttpContext.TraceIdentifier + ": Header reading succeeded");
                    var rootTag = stream.ReadLine();
                    if (rootTag == null)
                        throw new InvalidDataException("Proper XML document with valid root tag is expected, received null");
                    var serializer = GetXmlSerializerByRootTag(rootTag);
                    Logger.LogDebug(HttpContext.TraceIdentifier + ": Deserializer initialization succeeded");
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    Logger.LogDebug(HttpContext.TraceIdentifier + ": Repeated seeking succeeded");
                    var notification = serializer.Deserialize(memoryStream);
                    Logger.LogDebug(HttpContext.TraceIdentifier + ": Deserialization succeeded");

                    using (var db = GetDatabaseService())
                    {
                        if (notification is AccountCreatedNotification)
                        {
                            return Ok("{\n  data: \"New account\",\n  object: " + JsonConvert.SerializeObject(notification, Formatting.Indented) + "\n}");
                        }
                        if (notification is AccountBillingUpdatedNotification)
                        {
                            return Ok("{\n  data: \"Billing info updated\",\n  object: " + JsonConvert.SerializeObject(notification, Formatting.Indented) + "\n}");
                        }
                        if (notification is SubscriptionCreatedNotification)
                        {
                            await RecurlyService.RegisterNewSubscription(db, HttpContext.TraceIdentifier, notification as SubscriptionCreatedNotification);
                            return Ok("{\n  data: \"New subscription\",\n  object: " + JsonConvert.SerializeObject(notification, Formatting.Indented) + "\n}");
                        }
                        if (notification is SubscriptionRenewedNotification)
                        {
                            await RecurlyService.RenewSubscription(db, HttpContext.TraceIdentifier, notification as SubscriptionRenewedNotification);
                            return Ok("{\n  data: \"Renewed subscription\",\n  object: " + JsonConvert.SerializeObject(notification, Formatting.Indented) + "\n}");
                        }
                        if (notification is SubscriptionExpiredNotification)
                        {
                            await RecurlyService.ExpireSubscription(db, HttpContext.TraceIdentifier, notification as SubscriptionExpiredNotification);
                            return Ok("{\n  data: \"Expired subscription\",\n  object: " + JsonConvert.SerializeObject(notification, Formatting.Indented) + "\n}");
                        }
                        if (notification is InvoiceCreatedNotification)
                        {
                            return Ok("{\n  data: \"New invoice\",\n  object: " + JsonConvert.SerializeObject(notification, Formatting.Indented) + "\n}");
                        }
                        if (notification is InvoiceClosedNotification)
                        {
                            return Ok("{\n  data: \"Closed invoice\",\n  object: " + JsonConvert.SerializeObject(notification, Formatting.Indented) + "\n}");
                        }
                        if (notification is PaymentSuccessfulNotification)
                        {
                            await RecurlyService.ProcessPayment(db, HttpContext.TraceIdentifier, notification as PaymentSuccessfulNotification);
                            return Ok("{\n  data: \"Payment\",\n  object: " + JsonConvert.SerializeObject(notification, Formatting.Indented) + "\n}");
                        }
                        return BadRequest("{\n  data: \"Unknown object\",\n  object: " + JsonConvert.SerializeObject(notification, Formatting.Indented) + "\n}");
                    }
                }
            }
            catch (RecurlyServiceDuplicateRequestException ex)
            {
                Logger.LogError(HttpContext.TraceIdentifier + ": Duplicate RECURLY Request [" + ex.ObjectName + "]: " + ex.Message);

                return NoContent();
            }
            catch (RecurlyServiceException ex)
            {
                Logger.LogCritical(HttpContext.TraceIdentifier + ": FATAL RECURLY EXCEPTION [" + ex.ObjectName + "]: " + ex.Message);

                return BadRequest(ex.Message + " for " + ex.ObjectName);
            }
            catch (DbUpdateException ex)
            {
                Logger.LogCritical(HttpContext.TraceIdentifier + ": FATAL DATABASE EXCEPTION [" + ex.Message + "]: " + ex.InnerException.Message);

                if (ex.InnerException is SqlException)
                {
                    Logger.LogCritical(HttpContext.TraceIdentifier + ": FATAL SQL ERROR: " + ex.InnerException.Message);
                    Logger.LogCritical(HttpContext.TraceIdentifier + ": FATAL SQL ERROR Details: " + (ex.InnerException as SqlException).Errors);
                    Logger.LogCritical(HttpContext.TraceIdentifier + ": SQL STACKTRACE: " + ex.InnerException.StackTrace);
                }

                return BadRequest(ex.Message + "\n" + ex.InnerException.StackTrace);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(HttpContext.TraceIdentifier + ": FATAL EXCEPTION: " + ex.Message);
                Logger.LogCritical(HttpContext.TraceIdentifier + ": STACKTRACE: " + ex.StackTrace);

                if (ex.InnerException != null)
                {
                    Logger.LogCritical(HttpContext.TraceIdentifier + ": FATAL EXCEPTION Type: " + ex.GetType().Name);
                    Logger.LogCritical(HttpContext.TraceIdentifier + ": FATAL EXCEPTION [INNER] Type: " + ex.InnerException.GetType().Name);
                    Logger.LogCritical(HttpContext.TraceIdentifier + ": FATAL EXCEPTION [INNER]: " + ex.Message);
                    Logger.LogCritical(HttpContext.TraceIdentifier + ": STACKTRACE [INNER]: " + ex.StackTrace);
                }

                return BadRequest(ex.Message + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Auxillary method. Returns proper typed XMLSerializer for the given root tag.
        /// </summary>
        /// <param name="rootTag">Full root tag encluding the angle brackets.</param>
        /// <returns>Instantiated and properly typed XmlSerializer object.</returns>
        private static XmlSerializer GetXmlSerializerByRootTag(string rootTag)
        {
            XmlSerializer serializer;
            switch (rootTag)
            {
                case "<new_account_notification>":
                    serializer = new XmlSerializer(typeof(AccountCreatedNotification));
                    break;

                case "<billing_info_updated_notification>":
                    serializer = new XmlSerializer(typeof(AccountBillingUpdatedNotification));
                    break;

                case "<new_invoice_notification>":
                    serializer = new XmlSerializer(typeof(InvoiceCreatedNotification));
                    break;

                case "<closed_invoice_notification>":
                    serializer = new XmlSerializer(typeof(InvoiceClosedNotification));
                    break;

                case "<successful_payment_notification>":
                    serializer = new XmlSerializer(typeof(PaymentSuccessfulNotification));
                    break;

                case "<new_subscription_notification>":
                    serializer = new XmlSerializer(typeof(SubscriptionCreatedNotification));
                    break;

                case "<renew_subscription_notification>":
                    serializer = new XmlSerializer(typeof(SubscriptionRenewedNotification));
                    break;

                case "<expired_subscription_notification>":
                    serializer = new XmlSerializer(typeof(SubscriptionExpiredNotification));
                    break;

                default:
                    throw new InvalidDataException("Unknown XML document root: " + rootTag);
            }

            return serializer;
        }
    }
}