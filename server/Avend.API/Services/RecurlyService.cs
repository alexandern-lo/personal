using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Avend.API.Model;
using Avend.API.Model.Recurly;
using Avend.API.Services.Exceptions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Qoden.Validation;

using RecurlySubscription = Avend.API.Model.Recurly.DataTypes.Subscription;

namespace Avend.API.Services
{
    public class RecurlyService
    {
        private ILogger Logger { get; }
        public AvendDbContext Db { get; }

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        /// 
        /// <param name="db">Database context to operate on</param>
        /// <param name="logger">Logger object to be injected</param>
        public RecurlyService(AvendDbContext db, ILogger<RecurlyService> logger)
        {
            Assert.Argument(db, nameof(db)).NotNull();
            Assert.Argument(logger, nameof(logger)).NotNull();

            Db = db;
            Logger = logger;
        }

        /// <summary>
        /// Generating a new user subscription based on recurly data received by webhook.
        /// </summary>
        /// 
        /// <param name="db">Database context to operate on</param>
        /// <param name="traceIdentifier">Request identifier to use when writing logs</param>
        /// <param name="subscrNotification">Data from Recurly notification</param>
        /// 
        /// <returns>Task to be awaited</returns>
        /// 
        /// <exception cref="RecurlyServiceException">If subscription data cannot be parsed or account is not found</exception>
        public async Task RegisterNewSubscription(AvendDbContext db, string traceIdentifier, SubscriptionCreatedNotification subscrNotification)
        {
            if (string.IsNullOrWhiteSpace(subscrNotification.Account.AccountCode))
            {
                throw new RecurlyServiceException("NewSubscription", "No user UID defined in account");
            }

            Guid userUid;
            var parsingResult = Guid.TryParse(subscrNotification.Account.AccountCode, out userUid);
            if (!parsingResult)
            {
                throw new RecurlyServiceException("NewSubscription", "User UID defined in account is not valid");
            }

            var subscription = subscrNotification.Subscription;

            Logger.LogInformation(traceIdentifier + ": Adding new subscription to database: " + subscription.Uid);
            var userSubscription = db.SubscriptionsTable.FirstOrDefault(record => record.Service == SubscriptionServiceType.Recurly && record.ExternalUid == subscription.Uid.ToString());
            if (userSubscription == null)
            {
                userSubscription = new SubscriptionRecord
                {
                    Uid = Guid.NewGuid(),
                    ExternalUid = subscription.Uid.ToString().Replace("-", ""),
                    RecurlyAccountUid = userUid,
                    AdditionalData = JsonConvert.SerializeObject(subscrNotification, Formatting.Indented),
                    CreatedAt = DateTime.UtcNow,
                };
                db.SubscriptionsTable.Add(userSubscription);
                UpdateUserSubscription(userSubscription, subscription);
                await db.SaveChangesAsync();
                Logger.LogInformation(traceIdentifier + ": Added new subscription to database: " + subscription.Uid);
            }
            else
            {
                Logger.LogInformation(traceIdentifier + ": Subscription already in database: " + subscription.Uid);
            }
        }

        /// <summary>
        /// Updates the user subscription based on recurly data received by webhook.
        /// </summary>
        /// <param name="db">Database context to operate on</param>
        /// <param name="traceIdentifier">Request identifier to use when writing logs</param>
        /// <param name="subscrNotification">Data from Recurly notification</param>
        /// 
        /// <returns>Task to be awaited</returns>
        /// 
        /// <exception cref="RecurlyServiceException">If subscription data cannot be parsed or account is not found</exception>
        public async Task RenewSubscription(AvendDbContext db, string traceIdentifier, SubscriptionRenewedNotification subscrNotification)
        {
            if (string.IsNullOrWhiteSpace(subscrNotification.Account.AccountCode))
            {
                throw new RecurlyServiceException("RenewSubscription", "No user UID defined in account");
            }

            Guid userUid;

            var parsingResult = Guid.TryParse(subscrNotification.Account.AccountCode, out userUid);

            if (!parsingResult)
            {
                throw new RecurlyServiceException("RenewSubscription", "User UID defined in account is not valid");
            }

            Logger.LogInformation(traceIdentifier + ": Adding new subscription to database: " + subscrNotification.Subscription.Uid);

            var subscriptionExternalUid = subscrNotification.Subscription.Uid.ToString().Replace("-", "");

            var userSubscription = db.SubscriptionsTable.FirstOrDefault(record =>
                    record.Service == SubscriptionServiceType.Recurly && record.ExternalUid == subscriptionExternalUid
                );

            if (userSubscription == null)
                throw new RecurlyServiceException("RenewSubscription", "Subscription with this UID should exists: " + subscrNotification.Subscription.Uid);

            if (userSubscription.RecurlyAccountUid != userUid)
                throw new RecurlyServiceException("RenewSubscription", "Subscription with this UID should belong to the user: " + userUid);

            UpdateUserSubscription(userSubscription, subscrNotification.Subscription);

            await db.SaveChangesAsync();
            Logger.LogInformation(traceIdentifier + ": Renewed subscription in database: " + subscrNotification.Subscription.Uid);
        }

        /// <summary>
        /// Updates the user subscription based on recurly data received by webhook.
        /// </summary>
        /// <param name="db">Database context to operate on</param>
        /// <param name="traceIdentifier">Request identifier to use when writing logs</param>
        /// <param name="subscrNotification">Data from Recurly notification</param>
        /// 
        /// <returns>Task to be awaited</returns>
        /// 
        /// <exception cref="RecurlyServiceException">If subscription data cannot be parsed or account is not found</exception>
        public async Task ExpireSubscription(AvendDbContext db, string traceIdentifier, SubscriptionExpiredNotification subscrNotification)
        {
            if (string.IsNullOrWhiteSpace(subscrNotification.Account.AccountCode))
            {
                throw new RecurlyServiceException("RenewSubscription", "No user UID defined in account");
            }

            Guid userUid;

            var parsingResult = Guid.TryParse(subscrNotification.Account.AccountCode, out userUid);

            if (!parsingResult)
            {
                throw new RecurlyServiceException("RenewSubscription", "User UID defined in account is not valid");
            }

            Logger.LogInformation(traceIdentifier + ": Adding new subscription to database: " + subscrNotification.Subscription.Uid);

            var subscriptionExternalUid = subscrNotification.Subscription.Uid.ToString().Replace("-", "");

            var userSubscription = db.SubscriptionsTable.FirstOrDefault(record =>
                    record.Service == SubscriptionServiceType.Recurly && record.ExternalUid == subscriptionExternalUid
                );

            if (userSubscription == null)
                throw new RecurlyServiceException("RenewSubscription", "Subscription with this UID should exists: " + subscrNotification.Subscription.Uid);

            if (userSubscription.RecurlyAccountUid != userUid)
                throw new RecurlyServiceException("RenewSubscription", "Subscription with this UID should belong to the user: " + userUid);

            UpdateUserSubscription(userSubscription, subscrNotification.Subscription);

            await db.SaveChangesAsync();
            Logger.LogInformation(traceIdentifier + ": Renewed subscription in database: " + subscrNotification.Subscription.Uid);
        }

        /// <summary>
        /// Stores payment record based on recurly data received by webhook.
        /// </summary>
        /// 
        /// <param name="db">Database context to operate on</param>
        /// <param name="traceIdentifier">Request identifier to use when writing logs</param>
        /// <param name="paymentNotification">Data from Recurly notification</param>
        /// 
        /// <returns>Task to be awaited</returns>
        /// 
        /// <exception cref="RecurlyServiceException">If payment data cannot be parsed or account is not found</exception>
        public async Task ProcessPayment(AvendDbContext db, string traceIdentifier, PaymentSuccessfulNotification paymentNotification)
        {
            if (string.IsNullOrWhiteSpace(paymentNotification.Account.AccountCode))
            {
                throw new RecurlyServiceException("Payment", "No user UID defined in account");
            }

            Guid userUid;

            var parsingResult = Guid.TryParse(paymentNotification.Account.AccountCode, out userUid);

            if (!parsingResult)
            {
                throw new RecurlyServiceException("Payment", "User UID defined in the account (account_code) is not valid: " + paymentNotification.Account.AccountCode);
            }

            Logger.LogInformation(traceIdentifier + ": Adding new transaction to database for subscription " + paymentNotification.Transaction.SubscriptionUid);

            var userSubscription = GetUserSubscriptionByUidWithRetries(db, paymentNotification);

            if (userSubscription == null)
            {
                throw new RecurlyServiceException("Payment", "Cannot find subscription for payment: " + paymentNotification.Transaction.SubscriptionUid);
            }

            var userTransaction = db.UserTransactionsTable.Include(transaction => transaction.Subscription)
                .FirstOrDefault(
                    record => record.Service == SubscriptionServiceType.Recurly
                              && record.ExternalUid == paymentNotification.Transaction.Uid.ToString()
                              && record.Subscription.ExternalUid == paymentNotification.Transaction.SubscriptionUid.ToString()
                );

            if (userTransaction != null)
            {
                Logger.LogCritical(traceIdentifier + ": Found existing transaction with external UID: " + paymentNotification.Transaction.Uid);

                //  throw new RecurlyServiceException("Payment", "Transaction with this UID already exists: " + paymentNotification.Transaction.SubscriptionUid);

                return;
            }

            db.UserTransactionsTable.Add(
                new UserTransaction
                {
                    Uid = Guid.NewGuid(),
                    UserUid = userUid,
                    Service = SubscriptionServiceType.Recurly,
                    Status = GetTransactionStatusByRecurlyState(paymentNotification.Transaction.Status),
                    ExternalUid = paymentNotification.Transaction.SubscriptionUid.ToString(),
                    AdditionalData = JsonConvert.SerializeObject(paymentNotification, Formatting.Indented),
                    Subscription = userSubscription,
                    CreatedAt = DateTime.UtcNow,
                });

            await db.SaveChangesAsync();

            Logger.LogInformation(traceIdentifier + ": Added new transaction to database for subscription " + paymentNotification.Transaction.SubscriptionUid);
        }

        #region Auxillary methods

        private static SubscriptionRecord GetUserSubscriptionByUidWithRetries(AvendDbContext db, PaymentSuccessfulNotification paymentNotification)
        {
            var externalSubscriptionUid = paymentNotification.Transaction.SubscriptionUid.ToString().Replace("-", "");

            for (var i = 0; i < 5; i++)
            {
                var userSubscription = db.SubscriptionsTable.FirstOrDefault(
                    record =>
                        record.Service == SubscriptionServiceType.Recurly &&
                        record.ExternalUid == externalSubscriptionUid
                );

                if (userSubscription != null)
                    return userSubscription;

                //  We wait for another webhook in case the order of webhooks was not right.
                Thread.Sleep(100);
            }

            throw new RecurlyServiceException("Payment", "Subscription UID defined in the transaction (subscription_id) is not found: " + paymentNotification.Transaction.SubscriptionUid);
        }

        public static void UpdateUserSubscription(SubscriptionRecord userSubscription, RecurlySubscription subscription)
        {
            var planObj = subscription.Plan;
            int seatsNumber;
            string planType;
            var parseRes = TryParsePlanType(planObj.Code, out seatsNumber, out planType);
            if (!parseRes)
            {
                seatsNumber = 1;
                planType = "individual";
            }

            userSubscription.Status = GetSubscriptionStatusByRecurlyState(subscription.State);
            userSubscription.Service = SubscriptionServiceType.Recurly;
            userSubscription.MaximumUsersCount = seatsNumber;
            userSubscription.Type = planType;
            userSubscription.ExpiresAt = subscription.ExpiresAt.HasValue
                ? subscription.ExpiresAt.Value
                : (
                    subscription.CurrentPeriodEndsAt.HasValue
                        ? subscription.CurrentPeriodEndsAt.Value
                        : DateTime.MinValue);
        }

        private static SubscriptionStatus GetSubscriptionStatusByRecurlyState(string recurlyState)
        {
            switch (recurlyState)
            {
                case "pending":
                    return SubscriptionStatus.Pending;

                case "active":
                    return SubscriptionStatus.Active;

                case "cancelled":
                    return SubscriptionStatus.Cancelled;

                case "expired":
                    return SubscriptionStatus.Expired;

                case "suspended":
                    return SubscriptionStatus.Suspended;
            }

            throw new RecurlyServiceException("SubscriptionState", "Unknown subscription state: " + recurlyState);
        }

        private static TransactionStatus GetTransactionStatusByRecurlyState(string recurlyState)
        {
            switch (recurlyState)
            {
                case "pending":
                    return TransactionStatus.Pending;

                case "success":
                    return TransactionStatus.Completed;

                case "cancelled":
                    return TransactionStatus.Cancelled;

                case "suspended":
                    return TransactionStatus.Suspended;
            }

            throw new RecurlyServiceException("PaymentState", "Unknown payment state: " + recurlyState);
        }

        /// <summary>
        /// Tries to parse the plan data from the plan code.
        /// At the moment can parse the maximum seats number and plan type.
        /// </summary>
        /// 
        /// <param name="planCode">The plan code as reported by Recurly WebHook / Payment Plan API object</param>
        /// <param name="seatsNumber">Maximum number of seats</param>
        /// <param name="planType">Plan type string</param>
        /// 
        /// <returns>True if successful</returns>
        public static bool TryParsePlanType(string planCode, out int seatsNumber, out string planType)
        {
            planType = "unknown";
            seatsNumber = 1;

            var codeParts = planCode.Split('_');

            if (codeParts.Length < 2)
                return false;

            planType = codeParts[1];

            if (planType == "individual")
                return true;

            if (codeParts.Length < 3)
                return false;

            seatsNumber = Convert.ToInt32(codeParts[3]);

            return true;
        }

        #endregion Auxillary methods
    }
}