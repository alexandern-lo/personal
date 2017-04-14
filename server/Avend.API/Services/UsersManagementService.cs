using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Avend.API.Infrastructure.Logging;
using Avend.API.Model;
using Avend.API.Services.Crm;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Qoden.Validation;

namespace Avend.API.Services
{
    /// <summary>
    /// Implements functions required for Users management.
    /// <list type="bullet">
    /// <item>Manage user settings</item>
    /// <item>Manage user subscription data</item>
    /// </list>
    /// </summary>
    public class UsersManagementService
    {
        private ILogger Logger { get; }

        public UsersManagementService(AvendDbContext db, CrmConfigurationService crmService)
        {
            Assert.Argument(db, nameof(db)).NotNull();
            Logger = AvendLog.CreateLogger(nameof(UsersManagementService));
            Db = db;
            CrmService = crmService;
        }

        public CrmConfigurationService CrmService { get; }

        public AvendDbContext Db { get; }

        /// <summary>
        /// Validated the subscription and probably throws exceptions on problems.
        /// </summary>
        /// <param name="userUid">UID of the user for which to retrieve data</param>
        public void ValidateSubscription(Guid userUid)
        {
        }

        /// <summary>
        /// Returns user settings record for the given user.
        /// If no record is found, it creates a new one.
        /// </summary>
        /// <param name="userUid">UID of the user for which to retrieve data</param>
        /// <returns>SettingsRecord record for the given user</returns>
        [SuppressMessage("ReSharper", "ConvertIfStatementToNullCoalescingExpression")]
        public async Task<SettingsRecord> GetSettingsForUser(Guid userUid)
        {
            var settings = await GetUserSettingsQuery(userUid).FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = await SetupDefaultSettingsForUser(userUid);
            }
            return settings;
        }

        /// <summary>
        /// Sets up the default SettingsRecord record for the given user and saves it to database.
        /// </summary>
        /// <param name="userUid">UID of the user for which to create settings</param>
        /// <returns>The newly created SettingsRecord record.</returns>
        public async Task<SettingsRecord> SetupDefaultSettingsForUser(Guid userUid)
        {
            Logger.LogDebug("SetupDefaultSettingsForUser {0}", userUid);
            var settings = new SettingsRecord
            {
                UserUid = userUid,
                DefaultCrmId = null,
            };

            Db.Settings.Add(settings);

            await Db.SaveChangesAsync();

            return settings;
        }

        /// <summary>
        /// Returns query to retrieve user settings of the given user.
        /// </summary>
        /// <param name="userUid">UID of the user for which to retrieve data</param>
        /// <returns>Query that returns SettingsRecord record for the given user.</returns>
        public IQueryable<SettingsRecord> GetUserSettingsQuery(Guid? userUid)
        {
            var userSettingsTable = Db.Settings
                .Include(record => record.DefaultCrm);

            var settingsQuery = from settings in userSettingsTable
                where
                settings.UserUid == userUid
                select settings;

            return settingsQuery;
        }

        /// <summary>
        /// Returns terms record accepted by the current user or latest one otherwise.
        /// Also sets the acceptedAt field if the accepted terms are found.
        /// </summary>
        /// 
        /// <param name="userUid">Users UID</param>
        /// <param name="acceptedAt">Reference to DateTime that will receive the date on which user has accepted the terms</param>
        /// 
        /// <returns>Terms record accepted by the current user or latest one otherwise</returns>
        public Terms GetUserTermsStatus(Guid userUid, ref DateTime? acceptedAt)
        {
            Terms termsObj;

            var acceptedTerms = (
                    from t in Db.TermsAcceptancesTable
                    where
                    t.UserUid == userUid
                    orderby
                    t.AcceptedAt descending
                    select t)
                .FirstOrDefault();
            if (acceptedTerms != null)
            {
                termsObj = (
                        from terms in Db.TermsTable
                        where
                        terms.Id == acceptedTerms.TermsId
                        orderby
                        terms.ReleaseDate descending
                        select terms)
                    .FirstOrDefault();
                acceptedAt = acceptedTerms.AcceptedAt;
            }
            else
            {
                var termsQuery = from terms in Db.TermsTable
                    orderby
                    terms.ReleaseDate descending
                    select terms;
                termsObj = termsQuery.FirstOrDefault();
            }

            return termsObj;
        }
    }
}