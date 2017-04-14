using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Avend.API.Infrastructure.Logging;
using Avend.API.Infrastructure.SearchExtensions;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Qoden.Validation;

namespace Avend.API.Services.Leads
{
    public class LeadsRepository
    {
        public readonly ILogger Logger;

        public AvendDbContext Db { get; }

        public Expression<Func<LeadRecord, bool>> Scope { get; set; } = e => true;

        public LeadsRepository(AvendDbContext db)
        {
            Assert.Argument(db, nameof(db)).NotNull();
            Db = db;
            Logger = AvendLog.CreateLogger(nameof(LeadsRepository));
        }

        public DefaultSearch<LeadRecord> SearchBy(SearchQueryParams searchQuery, Guid? subscriptionUid)
        {
            searchQuery.ApplyDefaultSortOrder<LeadRecord>();
            searchQuery.Validate().Throw();

            var dbSet = Db.LeadsTable
                    .Include(x => x.Subscription)
                    .Include(x => x.Event)
                    .ThenInclude(e => e.Questions)
                    .ThenInclude(q => q.Choices)
                    .Include(x => x.Phones)
                    .Include(x => x.Emails)
                    .Include(x => x.QuestionAnswers)
                    .Include(x => x.ExportStatuses)
                    .Include(x => x.User)
                    .NotDeleted()
                    .Where(Scope)
                ;

            var search = DefaultSearch.Start(searchQuery, dbSet);

            search.Filter(collection =>
            {
                collection = collection.Where(Scope);

                if (subscriptionUid.HasValue)
                {
                    var subscriptionRecord = Db.SubscriptionsTable.FirstOrDefault(x => x.Uid == subscriptionUid.Value);

                    collection = subscriptionRecord != null
                        ? collection.Where(x => x.SubscriptionId == subscriptionRecord.Id)
                        : Enumerable.Empty<LeadRecord>().AsQueryable();
                }

                return collection;
            });

            return search;
        }

        public async Task<LeadRecord> FindLeadByUid(Guid leadUid)
        {
            return await Db.LeadsTable
                .Include(x => x.Subscription)
                .Include(x => x.Event)
                .ThenInclude(e => e.Questions)
                .ThenInclude(q => q.Choices)
                .Include(x => x.Phones)
                .Include(x => x.Emails)
                .Include(x => x.ExportStatuses)
                .Include(x => x.QuestionAnswers)
                .Include(x => x.User)
                .NotDeleted()
                .Where(Scope)
                .FirstOrDefaultAsync(record => record.Uid == leadUid);
        }

        public LeadRecord CreateEmptyLead(Guid userUid, long? subscriptionId, long? eventId = null)
        {
            var lead = new LeadRecord
            {
                Uid = Guid.NewGuid(),
                UserUid = userUid,
                SubscriptionId = subscriptionId,
                EventId = eventId                
            };

            Db.LeadsTable.Add(lead);
            //I don't like it, this needs to be refactored in a way that related records available 
            //and does not have to be loaded 
            Db.Entry(lead).Reference(x => x.Subscription).Load();
            Db.Entry(lead).Reference(x => x.User).Load();
            Db.Entry(lead).Reference(x => x.Event).Load();
            Db.Entry(lead.Event)?.Collection(x => x.Questions).Load();
            Db.Entry(lead.Event)?.Collection(x => x.Questions).Query().Include(x => x.Choices).Load();

            return lead;
        }

        public void DeleteLead(LeadRecord leadRecord, bool soft = true)
        {
            Assert.Argument(leadRecord, nameof(leadRecord)).NotNull();
            if (soft)
            {
                leadRecord.Deleted = true;
            }
            else
            {
                var emails = Db.LeadEmailsTable
                    .Where(x => x.LeadId == leadRecord.Id);
                Db.RemoveRange(emails);

                var phones = Db.LeadPhonesTable
                    .Where(x => x.LeadId == leadRecord.Id);
                Db.RemoveRange(phones);

                var exportStatuses = Db.LeadExportStatusesTable
                    .Where(x => x.LeadId == leadRecord.Id);
                Db.RemoveRange(exportStatuses);

                var questionAnswers = Db.LeadQuestionAnswersTable
                    .Where(x => x.LeadId == leadRecord.Id);
                Db.RemoveRange(questionAnswers);

                Db.LeadsTable.Remove(leadRecord);
            }
        }

        /// <summary>
        /// Returns given number of recent leads for the given userUid.
        /// 
        /// Includes deleted leads too - for recent activity leads.
        /// 
        /// No access/validity checks are made.
        /// </summary>
        /// 
        /// <param name="userUid">Uid of the user to get recent leads for</param>
        /// <param name="count">Count of records to return</param>
        /// 
        /// <returns>Enumerable of leads, including deleted</returns>
        public IEnumerable<LeadRecord> GetRecentLeads(Guid userUid, int count)
        {
            Assert.Argument(count, nameof(count)).Greater(0);

            return Db.LeadsTable
                .Include(x => x.Event)
                .ThenInclude(e => e.Questions)
                .ThenInclude(q => q.Choices)
                .Include(x => x.Phones)
                .Include(x => x.Emails)
                .Include(x => x.ExportStatuses)
                .Include(x => x.QuestionAnswers)
                .Include(x => x.User)
                .Where(record => record.UserUid == userUid)
                .OrderByDescending(record => record.UpdatedAt)
                .Take(count).ToList().Select(
                    record =>
                    {
                        record.Subscription = Db.SubscriptionsTable
                            .FirstOrDefault(
                                recSubscription => recSubscription.Id == record.SubscriptionId
                            );

                        return record;
                    }
                );
        }

        public void PopulateIdsInLeadQuestionAnswer(AvendDbContext db, LeadRecord leadObj)
        {
            foreach (var questionAnswer in leadObj.QuestionAnswers)
            {
                Check.Value(questionAnswer.EventQuestionUid, onError: AvendErrors.NotFound).HasValue();
                Check.Value(questionAnswer.EventAnswerUid, onError: AvendErrors.NotFound).HasValue();

                var eventQuestion =
                    db.Questions.FirstOrDefault(record => record.Uid == questionAnswer.EventQuestionUid);

                Check.Value(eventQuestion, onError: AvendErrors.NotFound).NotNull();

                var eventAnswer =
                    db.AnswerChoices.FirstOrDefault(record => record.Uid == questionAnswer.EventAnswerUid);

                Check.Value(eventAnswer, onError: AvendErrors.NotFound).NotNull();

                Logger.LogInformation("The question record found is:\n" + eventQuestion.ToJson());
                Logger.LogInformation("The answer record found is:\n" + eventAnswer.ToJson());

                questionAnswer.EventQuestionId = eventQuestion.Id;
                questionAnswer.QuestionText = eventQuestion.Text;

                questionAnswer.EventAnswerId = eventAnswer.Id;
                questionAnswer.AnswerText = eventAnswer.Text;
            }
        }
    }
}