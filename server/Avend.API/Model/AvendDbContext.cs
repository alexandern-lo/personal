using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avend.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Avend.API.Model
{
    public class AvendDbContext : DbContext
    {
        public DbSet<EventRecord> EventsTable { get; set; }
        public DbSet<EventAgendaItem> EventAgendaItemsTable { get; set; }
        public DbSet<EventUserExpenseRecord> EventUserExpensesTable { get; set; }
        public DbSet<EventUserGoalsRecord> EventUserGoalsTable { get; set; }

        public DbSet<AttendeeCategoryRecord> AttendeeCategories { get; set; }
        public DbSet<AttendeeCategoryOption> AttendeeCategoryOptions { get; set; }

        public DbSet<EventQuestionRecord> Questions { get; set; }
        public DbSet<AnswerChoiceRecord> AnswerChoices { get; set; }

        public DbSet<AttendeeRecord> Attendees { get; set; }
        public DbSet<AttendeeCategoryValue> AttendeeCategoryValues { get; set; }

        public DbSet<LeadRecord> LeadsTable { get; set; }
        public DbSet<LeadPhone> LeadPhonesTable { get; set; }
        public DbSet<LeadEmail> LeadEmailsTable { get; set; }
        public DbSet<LeadQuestionAnswer> LeadQuestionAnswersTable { get; set; }
        public DbSet<LeadExportStatus> LeadExportStatusesTable { get; set; }

        public DbSet<Terms> TermsTable { get; set; }
        public DbSet<TermsAcceptance> TermsAcceptancesTable { get; set; }

        public DbSet<CrmSystem> CrmSystemsTable { get; set; }

        public DbSet<SettingsRecord> Settings { get; set; }
        public DbSet<CrmRecord> Crms { get; set; }

        public DbSet<SubscriptionRecord> SubscriptionsTable { get; set; }
        public DbSet<SubscriptionMember> SubscriptionMembers { get; set; }
        public DbSet<SubscriptionInvite> SubscriptionInvitesTable { get; set; }
        public DbSet<UserTransaction> UserTransactionsTable { get; set; }

        public DbSet<Resource> ResourcesTable { get; set; }

        //see https://github.com/aspnet/EntityFramework/issues/7237
        //this constructor has to come first
        //(averbin)
        public AvendDbContext(DbContextOptions<AvendDbContext> options)
            : base(options)
        {
        }

        public AvendDbContext()
        {
        }

        public AvendDbContext(string connection)
            : base(new DbContextOptionsBuilder().UseSqlServer(
                connection
                ).Options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.EnableSensitiveDataLogging();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ApplyUpdatedAtValue();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            ApplyUpdatedAtValue();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void ApplyUpdatedAtValue()
        {
            var selectedEntityList = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseRecord
                            && x.State == EntityState.Modified);

            foreach (var entity in selectedEntityList)
            {
                ((BaseRecord) entity.Entity).UpdatedAt = DateTime.UtcNow;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //            modelBuilder.Ignore<MeetingEventShortView>();

            OnModelCreatingCrmSystems(modelBuilder);

            OnModelCreatingResources(modelBuilder);

            OnModelCreatingSubscriptions(modelBuilder);

            modelBuilder.Entity<SettingsRecord>()
                .HasIndex(b => b.UserUid);
            modelBuilder.Entity<SettingsRecord>()
                .HasOne(s => s.DefaultCrm)
                .WithOne(c => c.Settings);

            OnModelCreatingUserCrmConfigurations(modelBuilder);

            base.OnModelCreating(modelBuilder);

            OnModelCreatingRecord<EventRecord>(modelBuilder);
            modelBuilder.Entity<EventRecord>()
                .SoftDeletable();

            OnModelCreatingEventAgendaItems(modelBuilder);

            OnModelCreatingEventUserExpenses(modelBuilder);

            OnModelCreatingEventUserGoals(modelBuilder);

            OnModelCreatingRecord<EventQuestionRecord>(modelBuilder);
            modelBuilder.Entity<EventQuestionRecord>()
                .HasOne(x => x.Event).WithMany(x => x.Questions);
            modelBuilder.Entity<EventQuestionRecord>()
                .HasIndex(x => new {x.UserId, x.EventId});

            OnModelCreatingRecord<AnswerChoiceRecord>(modelBuilder);
            modelBuilder.Entity<AnswerChoiceRecord>()
                .HasOne(x => x.Question).WithMany(x => x.Choices);

            OnModelCreatingAttendees(modelBuilder);

            modelBuilder.Entity<AttendeeCategoryRecord>()
                .HasIndex(rec => rec.EventId);
            modelBuilder.Entity<AttendeeCategoryRecord>()
                .HasIndex(rec => rec.Uid);
            modelBuilder.Entity<AttendeeCategoryRecord>()
                .HasOne(x => x.EventRecord)
                .WithMany(x => x.AttendeeCategories)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<AttendeeCategoryRecord>()
                .SoftDeletable();

            modelBuilder.Entity<AttendeeCategoryOption>()
                .HasIndex(rec => rec.CategoryId);
            modelBuilder.Entity<AttendeeCategoryOption>()
                .HasOne(x => x.AttendeeCategory)
                .WithMany(x => x.Options)
                .OnDelete(DeleteBehavior.Cascade);

            OnModelCreatingAttendeeCategoryValues(modelBuilder);

            OnModelCreatingLeads(modelBuilder);

            OnModelCreatingLeadQuestionAnswers(modelBuilder);

            OnModelCreatingLeadExportStatuses(modelBuilder);
        }

        private void OnModelCreatingLeadExportStatuses(ModelBuilder modelBuilder)
        {
            OnModelCreatingRecord<LeadExportStatus>(modelBuilder);
/*

            modelBuilder.Entity<LeadExportStatus>()
                .HasOne(leadQuestionAnswer => leadQuestionAnswer.Lead)
                .WithMany()
                .HasForeignKey(leadQuestionAnswer => leadQuestionAnswer.LeadId)
                .OnDelete(DeleteBehavior.Restrict); // no ON DELETE        
*/
        }

        private static void OnModelCreatingLeadQuestionAnswers(ModelBuilder modelBuilder)
        {
            OnModelCreatingRecord<LeadQuestionAnswer>(modelBuilder);

            modelBuilder.Entity<LeadQuestionAnswer>()
                .HasOne(leadQuestionAnswer => leadQuestionAnswer.EventQuestion)
                .WithMany()
                .HasForeignKey(leadQuestionAnswer => leadQuestionAnswer.EventQuestionId)
                .OnDelete(DeleteBehavior.Restrict); // no ON DELETE        

            modelBuilder.Entity<LeadQuestionAnswer>()
                .HasOne(leadQuestionAnswer => leadQuestionAnswer.Answer)
                .WithMany()
                .HasForeignKey(leadQuestionAnswer => leadQuestionAnswer.EventAnswerId)
                .OnDelete(DeleteBehavior.Restrict); // no ON DELETE        
        }

        private static void OnModelCreatingAttendeeCategoryValues(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttendeeCategoryValue>()
                .HasIndex(rec => rec.AttendeeId);

            modelBuilder.Entity<AttendeeCategoryValue>()
                .HasIndex(rec => new {rec.CategoryId, rec.CategoryOptionId });

            modelBuilder.Entity<AttendeeCategoryValue>()
                .HasIndex(rec => new {rec.AttendeeId, rec.CategoryId, rec.CategoryOptionId })
                .IsUnique();

            modelBuilder.Entity<AttendeeCategoryValue>()
                .HasOne(catValue => catValue.Category)
                .WithMany()
                .HasForeignKey(catValue => catValue.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // no ON DELETE        

            modelBuilder.Entity<AttendeeCategoryValue>()
                .HasOne(catValue => catValue.AttendeeCategoryOption)
                .WithMany()
                .HasForeignKey(catValue => catValue.CategoryOptionId)
                .OnDelete(DeleteBehavior.Restrict); // no ON DELETE        
        }

        private static void OnModelCreatingLeads(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeadRecord>()
                .HasIndex(lead => lead.EventId);

            modelBuilder.Entity<LeadRecord>()
                .HasIndex(rec => new { rec.SubscriptionId, rec.Qualification });

            modelBuilder.Entity<LeadRecord>()
                .HasIndex(rec => new { rec.UserUid, rec.Qualification });

            modelBuilder.Entity<LeadRecord>()
                .HasIndex(rec => new {rec.UserUid, rec.FirstName});

            modelBuilder.Entity<LeadRecord>()
                .HasIndex(rec => new {rec.UserUid, rec.LastName});

            modelBuilder.Entity<LeadRecord>()
                .HasIndex(rec => new {rec.UserUid, rec.JobTitle});

            modelBuilder.Entity<LeadRecord>()
                .HasIndex(rec => new {rec.UserUid, rec.CompanyName});

            modelBuilder.Entity<LeadRecord>()
                .HasIndex(rec => new { rec.UserUid, rec.CreatedAt });

            modelBuilder.Entity<LeadRecord>()
                .HasOne(l => l.User)
                .WithMany(u => u.Leads)
                .HasForeignKey(l => l.UserUid)
                .HasPrincipalKey(u => u.UserUid)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeadRecord>().SoftDeletable();

            OnModelCreatingRecord<LeadRecord>(modelBuilder);
        }

        private static void OnModelCreatingEventAgendaItems(ModelBuilder modelBuilder)
        {
            OnModelCreatingRecord<EventAgendaItem>(modelBuilder);

            modelBuilder.Entity<EventAgendaItem>()
                .HasIndex(rec => new { rec.EventId, rec.Date, rec.StartTimeTicks });

            modelBuilder.Entity<EventAgendaItem>()
                .HasIndex(rec => new { rec.EventId, rec.Location, rec.Date });
        }

        private static void OnModelCreatingEventUserExpenses(ModelBuilder modelBuilder)
        {
            OnModelCreatingUserDependentRecord<EventUserExpenseRecord>(modelBuilder);

            modelBuilder.Entity<EventUserExpenseRecord>()
                .HasIndex(rec => new { rec.UserUid, rec.SpentAt });

            modelBuilder.Entity<EventUserExpenseRecord>()
                .HasIndex(rec => new { rec.EventId, rec.UserUid, rec.SpentAt });

            modelBuilder.Entity<EventUserExpenseRecord>()
                .HasIndex(rec => new { rec.EventId, rec.SpentAt });
        }

        private static void OnModelCreatingEventUserGoals(ModelBuilder modelBuilder)
        {
            OnModelCreatingUserDependentRecord<EventUserGoalsRecord>(modelBuilder);

            modelBuilder.Entity<EventUserGoalsRecord>()
                .HasIndex(rec => new { rec.EventId, rec.UserUid});

            modelBuilder.Entity<EventUserGoalsRecord>()
                .HasIndex(rec => new { rec.EventId, rec.TenantUid });
        }

        private static void OnModelCreatingAttendees(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttendeeRecord>()
                .HasIndex(rec => rec.EventId);

            modelBuilder.Entity<AttendeeRecord>()
                .HasIndex(rec => new {rec.EventId, rec.FirstName, rec.Id});

            modelBuilder.Entity<AttendeeRecord>()
                .HasIndex(rec => new {rec.EventId, rec.LastName, rec.Id });

            modelBuilder.Entity<AttendeeRecord>()
                .HasIndex(rec => new {rec.EventId, rec.Company, rec.Id });

            modelBuilder.Entity<AttendeeRecord>()
                .HasIndex(rec => new {rec.EventId, rec.Email, rec.Id });
        }

        private static void OnModelCreatingUserCrmConfigurations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CrmRecord>()
                .HasIndex(b => b.Uid);

            modelBuilder.Entity<CrmRecord>()
                .HasIndex(b => b.UserUid);

            modelBuilder.Entity<CrmRecord>()
                .HasIndex(b => new { b.UserUid, b.Name });

            modelBuilder.Entity<CrmRecord>()
                .HasIndex(b => new { b.UserUid, b.Uid });
        }

        private static void OnModelCreatingCrmSystems(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CrmSystem>()
                .HasIndex(b => b.Name);

            OnModelCreatingRecord<CrmSystem>(modelBuilder);
        }

        private static void OnModelCreatingResources(ModelBuilder modelBuilder)
        {
            OnModelCreatingUserDependentRecord<Resource>(modelBuilder);

            modelBuilder.Entity<Resource>().SoftDeletable();

            modelBuilder.Entity<Resource>()
                .HasIndex(b => new { b.UserUid, b.Status });

            modelBuilder.Entity<Resource>()
                .HasIndex(b => b.EventId);

            modelBuilder.Entity<Resource>()
                .HasIndex(b => new { b.EventId, b.Status });

            modelBuilder.Entity<Resource>()
                .HasIndex(b => b.Url);
        }

        private static void OnModelCreatingSubscriptions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubscriptionRecord>()
                .HasIndex(b => b.Uid)
                .IsUnique();
            modelBuilder.Entity<SubscriptionRecord>()
                .HasIndex(b => b.ExternalUid)
                .IsUnique();
            modelBuilder.Entity<SubscriptionRecord>()
                .HasIndex(b => new { UserUid = b.RecurlyAccountUid, b.Status, b.ExpiresAt });
            OnModelCreatingRecord<SubscriptionRecord>(modelBuilder);

            modelBuilder.Entity<SubscriptionMember>()
                .HasAlternateKey(x => x.UserUid);
            modelBuilder.Entity<SubscriptionMember>()
                .HasIndex(b => b.SubscriptionId);
            modelBuilder.Entity<SubscriptionMember>()
                .CreatedAt(b => b.CreatedAt, false);
            modelBuilder.Entity<SubscriptionMember>()
                .UpdatedAt(b => b.UpdatedAt, false);
            modelBuilder.Entity<SubscriptionMember>()
                .Property(b => b.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .ForSqlServerHasDefaultValueSql("GETUTCDATE()")
                .Metadata.IsReadOnlyAfterSave = false;

            modelBuilder.Entity<SubscriptionInvite>()
                .CreatedAt(b => b.CreatedAt, false);
            modelBuilder.Entity<SubscriptionInvite>()
                .UpdatedAt(b => b.CreatedAt, false);
            modelBuilder.Entity<SubscriptionInvite>()
                .HasIndex(b => new { InviteId = b.InviteCode })
                .IsUnique();
            modelBuilder.Entity<SubscriptionInvite>()
                .HasIndex(b => b.Uid)
                .IsUnique();
            modelBuilder.Entity<SubscriptionInvite>()
                .HasIndex(b => b.Email);
        }

        protected static void OnModelCreatingRecord<TRecord>(ModelBuilder modelBuilder) where TRecord : BaseRecord
        {
            modelBuilder.Entity<TRecord>()
                .HasIndex(b => b.Uid);

            modelBuilder.Entity<TRecord>()
                .HasIndex(b => b.CreatedAt);

            modelBuilder.Entity<TRecord>()
                .Property(b => b.CreatedAt)
                .ForSqlServerHasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<TRecord>()
                .HasIndex(b => b.UpdatedAt);

            modelBuilder.Entity<TRecord>()
                .Property(b => b.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .ForSqlServerHasDefaultValueSql("GETUTCDATE()")
                .Metadata.IsReadOnlyAfterSave = false;
        }

        protected static void OnModelCreatingUserDependentRecord<TRecord>(ModelBuilder modelBuilder)
            where TRecord : BaseUserDependentRecord
        {
            OnModelCreatingRecord<TRecord>(modelBuilder);

            modelBuilder.Entity<TRecord>()
                .HasIndex(b => b.UserUid);

            modelBuilder.Entity<TRecord>()
                .HasIndex(b => b.TenantUid);
        }
    }
}