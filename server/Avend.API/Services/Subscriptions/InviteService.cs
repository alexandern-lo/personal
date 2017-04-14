using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Avend.API.Infrastructure;
using Avend.API.Model;
using Avend.API.Validation.Util;
using Microsoft.EntityFrameworkCore;
using Qoden.Validation;

namespace Avend.API.Services.Subscriptions
{
    [DataContract]
    public class InviteRequestDto
    {
        [DataMember(Name = "emails")]
        public string[] Emails { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "subject")]
        public string Subject { get; set; }
    }

    public class InviteService
    {
        private readonly DbContextOptions<AvendDbContext> _dbOptions;
        private readonly ISendGrid _sendgrid;
        private readonly UserContext _userContext;

        public InviteService(DbContextOptions<AvendDbContext> dbOptions, UserContext userContext, ISendGrid sendgrid)
        {
            Assert.Argument(dbOptions, nameof(dbOptions)).NotNull();
            Assert.Argument(userContext, nameof(userContext)).NotNull();
            Assert.Argument(sendgrid, nameof(sendgrid)).NotNull();

            _dbOptions = dbOptions;
            _sendgrid = sendgrid;
            _userContext = userContext;
        }

        public async Task<List<string>> Invite(Guid adminUid, InviteRequestDto inviteRequest,
            Func<string, string> linkGenerator)
        {
            var validator = new Validator();
            validator.CheckValue(inviteRequest.Emails, "emails")
                .MaxLength(100, "Can send only {Max} invites at a time")
                .MinLength(1, "Must have at least {Min} emails");
            validator.CheckValue(inviteRequest.Subject, "subject")
                .NotEmpty();
            validator.Throw();

            using (var db = new AvendDbContext(_dbOptions))
            {
                var admin = new SubscriptionAdmin(_userContext, new SubscriptionRepository(db));

                var skippedEmails = new List<string>();

                var emails = inviteRequest.Emails.Where(EmailValidations.IsEmail);
                var invites = emails.Select(e => admin.Invite(_userContext.Subscription.Uid.GetValueOrDefault(), e)).ToList();
                var sendResult = await _sendgrid.Send(inviteRequest.Subject, inviteRequest.Message, linkGenerator, invites);
                Check.Value(sendResult).IsTrue("Invite email send failed");
                await db.SaveChangesAsync();

                return skippedEmails;
            }
        }

        public async Task DeleteInvite(Guid adminUid, Guid userUid)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var admin = new SubscriptionAdmin(_userContext, new SubscriptionRepository(db));
                admin.DeleteInvite(userUid);

                await db.SaveChangesAsync();
            }
        }

        public async Task AcceptInvite(Guid userUid, string inviteCode)
        {
            Assert.Argument(inviteCode, nameof(inviteCode)).NotNull();

            using (var member = new SubscriptionMember(_dbOptions))
            {
                if (member.Find(userUid))
                {
                    member.AcceptInvite(inviteCode);
                }
                member.Validator.Throw();
                await member.SaveChangesAsync();
            }
        }
    }
}