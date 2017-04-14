using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avend.API.Infrastructure.Logging;
using Avend.API.Infrastructure.SearchExtensions;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Services.Dashboard;
using Avend.API.Services.Events;
using Avend.API.Services.Exceptions;
using Avend.API.Services.Leads.NetworkDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Qoden.Validation;

namespace Avend.API.Services.Leads
{
    public class LeadsCrudService
    {
        public readonly ILogger Logger;

        public DbContextOptions<AvendDbContext> DbOptions { get; }
        public UserContext UserContext { get; }

        private static readonly string[] AllowedSearchFields = new[]
        {
            "tenant_uid",
            "first_name",
            "last_name",
            "job_title",
            "company_name",
            "created_at",
        };

        /// <summary>
        /// Service constructor
        /// </summary>
        /// <param name="userContext">User context</param>
        /// <param name="dbOptions">Configuration for connection to AvendDb</param>
        public LeadsCrudService(
            UserContext userContext,
            DbContextOptions<AvendDbContext> dbOptions
        )
        {
            Assert.Argument(userContext, nameof(userContext)).NotNull();
            Assert.Argument(dbOptions, nameof(dbOptions)).NotNull();

            UserContext = userContext;

            DbOptions = dbOptions;

            Logger = AvendLog.CreateLogger(nameof(DashboardService));
        }

        /// <summary>
        /// Returns LeadDTO with given leadUid for given userUid
        /// </summary>
        /// 
        /// <param name="leadUid">Uid of the lead to retrieve</param>
        /// 
        /// <returns>LeadDTO object containing retrieved data</returns>
        public async Task<LeadDto> GetLeadByUid(Guid? leadUid)
        {
            Check.Value(UserContext.UserUid, nameof(UserContext.UserUid)).NotNull();
            Check.Value(leadUid, nameof(leadUid)).NotNull();

            using (var db = new AvendDbContext(DbOptions))
            {
                var leadsRepo = new LeadsRepository(db)
                {
                    Scope = UserContext.AvailableLeads()
                };

                // ReSharper disable once PossibleInvalidOperationException
                var lead = await leadsRepo.FindLeadByUid(leadUid.Value);

                Check.Value(lead, "lead_uid", AvendErrors.NotFound).NotNull();

                return LeadDto.From(lead, lead.Event);
            }
        }

        /// <summary>
        /// Returns Leads owned by current user
        /// </summary>
        /// 
        /// <param name="searchQuery">Prepared object with filtering, sorting and pagination parameters</param>
        /// 
        /// <returns>SearchResult object containing retrieved list with some metadata</returns>
        public SearchResult<LeadDto> FindOwnedBy(SearchQueryParams searchQuery)
        {
            Logger.LogInformation($@"Trying to get all leads with names starting with '{searchQuery.Filter}'");

            Check.Value(UserContext.UserUid, nameof(UserContext.UserUid)).NotNull();

            Assert.Argument(searchQuery, nameof(searchQuery)).NotNull();

            searchQuery.ApplyDefaultSortOrder<LeadRecord>();
            searchQuery.Validate().Throw();

            if (!string.IsNullOrWhiteSpace(searchQuery.SortField))
                Check.Value(searchQuery.SortField, "sort_field")
                    .In(AllowedSearchFields, "Invalid sort field: " + searchQuery.SortField);

            using (var db = new AvendDbContext(DbOptions))
            {
                var leadsRepo = new LeadsRepository(db)
                {
                    Scope = UserContext.OwnLeads()
                };

                var leadsSearch = leadsRepo.SearchBy(searchQuery, null);

                return leadsSearch.Paginate(LeadDto.From);
            }
        }

        /// <summary>
        /// Returns LeadDTO with given leadUid for given userUid
        /// </summary>
        /// 
        /// <param name="searchQuery">Prepared object with filtering, sorting and pagination parameters</param>
        /// <param name="subscriptionUid">Uid of the subscription to additinally filter by</param>
        /// 
        /// <returns>LeadDTO object containing retrieved data</returns>
        public SearchResult<LeadDto> FindBy(SearchQueryParams searchQuery, Guid? subscriptionUid)
        {
            Logger.LogInformation($@"Trying to get all leads with names starting with '{searchQuery.Filter}'");

            Check.Value(UserContext.UserUid, nameof(UserContext.UserUid)).NotNull();

            Assert.Argument(searchQuery, nameof(searchQuery)).NotNull();

            searchQuery.ApplyDefaultSortOrder<LeadRecord>();
            searchQuery.Validate().Throw();

            if (!string.IsNullOrWhiteSpace(searchQuery.SortField))
                Check.Value(searchQuery.SortField, "sort_field")
                    .In(AllowedSearchFields, "Invalid sort field: " + searchQuery.SortField);

            using (var db = new AvendDbContext(DbOptions))
            {
                var leadsRepo = new LeadsRepository(db)
                {
                    Scope = UserContext.AvailableLeads()
                };

                var leadsSearch = leadsRepo.SearchBy(searchQuery, null);

                return leadsSearch.Paginate(LeadDto.From)
                    .WithFilter("tenant", subscriptionUid);
            }
        }

        /// <summary>
        /// Adds new lead into database from DTO for given userUid
        /// </summary>
        /// 
        /// <param name="leadDto">Lead DTO to add into database</param>
        /// 
        /// <returns>LeadDTO object containing inserted data</returns>
        /// 
        /// <exception cref="DuplicateRecordCreationException">If clientside UID is not null and we have a record with that value for current user.</exception>
        public async Task<LeadDto> CreateLeadFromDto(LeadDto leadDto)
        {
            Check.Value(UserContext.UserUid, nameof(UserContext.UserUid)).NotNull();
            Check.Value(leadDto, "lead").NotNull();
            Check.Value(leadDto.EventUid, "lead.event_uid").NotNull();
            Check.Value(
                leadDto.Emails != null && leadDto.Emails.Count > 0 || !string.IsNullOrWhiteSpace(leadDto.FirstName),
                "lead.event_uid").IsTrue("Either lead.first_name or lead.emails should be not empty");

            using (var db = new AvendDbContext(DbOptions))
            {
                var leadsRepo = new LeadsRepository(db)
                {
                    Scope = UserContext.OwnLeads()
                };

                var eventRepo = new EventsRepository(db) {Scope = UserContext.SelectableEvents()};
                var @event = eventRepo.FindEventByUid(leadDto.EventUid.GetValueOrDefault());
                Check.Value(@event, "event_uid").NotNull("Lead event not found or not accessible");

                if (leadDto.ClientsideUid.HasValue)
                {
                    //  We should not need the restriction on status here to be prevent

                    var lead = await db.LeadsTable
                        .Where(x => x.UserUid == UserContext.UserUid && x.ClientsideUid == leadDto.ClientsideUid)
                        .Select(x => new {x.Id, x.Uid})
                        .FirstOrDefaultAsync();

                    if (lead != null)
                    {
                        throw new DuplicateRecordCreationException(leadDto.ClientsideUid.Value,
                            lead.Uid,
                            "Already has record with clientside UID=" + lead.Uid);
                    }
                }
                var leadObj = leadsRepo.CreateEmptyLead(UserContext.UserUid, UserContext.TenantId, @event.Id);
                leadDto.UpdateLead(leadObj);
                if (string.IsNullOrWhiteSpace(leadObj.FirstName))
                    leadObj.FirstName = leadDto.Emails?.FirstOrDefault()?.Email;
                leadsRepo.PopulateIdsInLeadQuestionAnswer(db, leadObj);
                Logger.LogInformation("Inserting lead object into database:\n" + leadObj.ToJson());
                db.LeadsTable.Add(leadObj);
                await db.SaveChangesAsync();
                var dto = LeadDto.From(leadObj);
                //Is this a good idea? (averbin)
                dto.Owner = UserContext.Member;
                return dto;
            }
        }

        public async Task<LeadDto> Update(Guid leadUid, LeadDto leadDto)
        {
            Check.Value(UserContext.UserUid, nameof(UserContext.UserUid)).NotNull();
            Check.Value(leadDto, "lead").NotNull();
            Check.Value(leadDto.EventUid, "lead.event_uid").NotNull();
            Check.Value(leadDto.Uid == leadUid, "lead.lead_uid")
                .IsTrue("lead.lead_uid doesn't match the uid passed in URL");

            Logger.LogInformation($"Trying to update lead with UID={leadUid}: {leadDto}");

            using (var db = new AvendDbContext(DbOptions))
            {
                var leadsRepo = new LeadsRepository(db)
                {
                    Scope = UserContext.AvailableLeads()
                };

                var leadObj = await leadsRepo.FindLeadByUid(leadUid);

                Check.Value(leadObj, "lead", AvendErrors.NotFound).NotNull("Lead not found");

                if (leadObj.ClientsideUpdatedAt.HasValue && leadDto.ClientsideUpdatedAt.HasValue)
                    Check.Value(leadObj.ClientsideUpdatedAt < leadDto.ClientsideUpdatedAt, "lead.clientside_updated_at",
                            AvendErrors.OldData)
                        .IsTrue("The record was not updated. Newer data were received at " + leadObj.ClientsideUpdatedAt);

                var queryEvent = from recEvent in db.EventsTable.Where(UserContext.SelectableEvents())
                    where recEvent.Uid == leadDto.EventUid
                    select new {EventId = recEvent.Id};

                var newEventObj = queryEvent.FirstOrDefault();

                Check.Value(newEventObj, "event_uid").NotNull("Event not found");

                leadObj.EventId = newEventObj?.EventId;

                Logger.LogInformation("The source lead record is:\n" +
                                      JsonConvert.SerializeObject(leadObj, new JsonSerializerSettings()
                                      {
                                          ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                          PreserveReferencesHandling = PreserveReferencesHandling.All,
                                          NullValueHandling = NullValueHandling.Ignore,
                                      }));

                var queryLeadPhones = db.LeadPhonesTable.Where(leadPhone => leadPhone.LeadId == leadObj.Id);

                foreach (var record in queryLeadPhones)
                {
                    db.LeadPhonesTable.Remove(record);
                }

                var queryLeadEmails = db.LeadEmailsTable.Where(leadEmail => leadEmail.LeadId == leadObj.Id);

                foreach (var record in queryLeadEmails)
                {
                    db.LeadEmailsTable.Remove(record);
                }

                var queryQuestionAnswers =
                    db.LeadQuestionAnswersTable.Where(leadQuestionAnswer => leadQuestionAnswer.LeadId == leadObj.Id);

                foreach (var record in queryQuestionAnswers)
                {
                    db.LeadQuestionAnswersTable.Remove(record);
                }

                leadDto.UpdateLead(leadObj);

                leadsRepo.PopulateIdsInLeadQuestionAnswer(db, leadObj);

                //Logger.LogTrace("The updated lead record is justified");

                Logger.LogInformation("The updated lead record is:\n" +
                                      JsonConvert.SerializeObject(leadObj, new JsonSerializerSettings()
                                      {
                                          ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                          PreserveReferencesHandling = PreserveReferencesHandling.All,
                                          NullValueHandling = NullValueHandling.Ignore,
                                      }));

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Logger.LogWarning("DbContext Error: " + ex.Message + ", entries count is: " + ex.Entries.Count +
                                      ", extra data:\n" + JsonConvert.SerializeObject(ex.Data));
                }

                return LeadDto.From(leadObj);
            }
        }

        public async Task DeleteLeadByUidString(string leadUidStr)
        {
            Check.Value(UserContext.UserUid, nameof(UserContext.UserUid)).NotNull();
            Check.Value(leadUidStr, "lead_uid").NotNull();

            Guid leadUid;

            var res = Guid.TryParse(leadUidStr, out leadUid);

            Check.Value(res, "lead_uid").IsTrue("{Key} is not a valid GUID");

            using (var db = new AvendDbContext(DbOptions))
            {
                var leadsRepo = new LeadsRepository(db)
                {
                    Scope = UserContext.AvailableLeads()
                };

                var lead = await leadsRepo.FindLeadByUid(leadUid);

                Check.Value(lead, "lead_uid", AvendErrors.NotFound).NotNull();

                leadsRepo.DeleteLead(lead);

                await db.SaveChangesAsync();
            }
        }

        public List<LeadRecentActivityDto> GetRecentActivity(int count)
        {
            Check.Value(UserContext.UserUid, nameof(UserContext.UserUid)).NotNull();

            using (var db = new AvendDbContext(DbOptions))
            {
                var leadsRepo = new LeadsRepository(db)
                {
                    Scope = UserContext.AvailableLeads()
                };

                var leads = leadsRepo.GetRecentLeads(UserContext.UserUid, count);

                return leads.Select(LeadRecentActivityDto.From).ToList();
            }
        }
    }
}