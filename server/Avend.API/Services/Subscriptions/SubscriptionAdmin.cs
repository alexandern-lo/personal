using System;
using System.Linq;
using Avend.API.Infrastructure.Responses;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Qoden.Validation;

namespace Avend.API.Services.Subscriptions
{
    public class SubscriptionAdmin
    {
        private readonly SubscriptionRepository _repo;
        private readonly UserContext _userContext;

        public SubscriptionAdmin(UserContext userContext, SubscriptionRepository repo)
        {
            Assert.Argument(repo, nameof(repo)).NotNull();
            Assert.Argument(userContext, nameof(userContext)).NotNull();
            _repo = repo;
            _userContext = userContext;

            Check.Value(_userContext.Role, "role", AvendErrors.Forbidden)
                .In(new[] {UserRole.SuperAdmin, UserRole.Admin});

            if (_userContext.Role == UserRole.Admin)
            {
                var admin = _repo.FindMember(_userContext.UserUid, _userContext.Subscription?.Uid);

                Check.Value(admin, onError: AvendErrors.Forbidden)
                    .NotNull();

                Check.Value(admin?.Role, onError: AvendErrors.Forbidden)
                    .EqualsTo(SubscriptionMemberRole.Admin);
            }
        }

        public Model.SubscriptionMember UpdateRole(Guid userUid, bool admin)
        {
            var targetMember = _repo.FindMember(userUid, _userContext.Subscription?.Uid);
            Check.Value(targetMember, onError: AvendErrors.NotFound)
                .NotNull("Target member not found");

            if (_userContext.Role == UserRole.Admin)
            {
                if (!admin)
                {
                    CheckNotLastAdmin(targetMember, "Cannot revoke admin rights from last admin");
                }
            }
            Check.Value(targetMember.Role, "role", AvendErrors.Forbidden)
                .NotEqualsTo(SubscriptionMemberRole.SuperAdmin, "cannot update super admin role");
            targetMember.Role = admin ? SubscriptionMemberRole.Admin : SubscriptionMemberRole.User;
            return targetMember;
        }

        public Model.SubscriptionMember UpdateStatus(Guid userUid, bool status)
        {
            var targetMember = _repo.FindMember(userUid, _userContext.Subscription?.Uid);

            Check.Value(targetMember, onError: AvendErrors.NotFound)
                .NotNull();

            if (_userContext.Role == UserRole.Admin)
            {
                if (!status && targetMember.UserUid == _userContext.UserUid)
                {
                    CheckNotLastAdmin(targetMember, "Cannot disable last admin");
                }
            }

            Check.Value(targetMember.Role, "status", AvendErrors.Forbidden)
                .NotEqualsTo(SubscriptionMemberRole.SuperAdmin, "cannot update super admin status");
            targetMember.Status = status ? SubscriptionMemberStatus.Enabled : SubscriptionMemberStatus.Disabled;
            return targetMember;            
        }

        public SubscriptionInvite DeleteInvite(Guid userUid)
        {
            var invite = _repo.FindInviteByUId(userUid, _userContext.Subscription?.Uid);

            Check.Value(invite, onError: AvendErrors.NotFound)
                .NotNull();

            _repo.Db.Remove(invite);
            return invite;
        }

        //TODO - don't invite users one by one, select all invites from DB at once (averbin)        
        public SubscriptionInvite Invite(Guid subscriptionUid, string email)
        {
            SubscriptionRecord subscription;
            //Admin can only invite to his own subscription
            if (_userContext.Role == UserRole.Admin)
            {
                subscription = _repo.FindSubscriptionForUser(_userContext.UserUid);
            }
            else
            {
                subscription = _repo.FindSubscriptionByUid(subscriptionUid);
            }

            Check.Value(subscription, onError: AvendErrors.NotFound)
                .NotNull("Subscription not found");

            var invite = _repo.FindInviteByEmail(email, false);
            if (invite == null)
            {
                invite = new SubscriptionInvite
                {
                    Uid = Guid.NewGuid(),
                    Email = email,
                    Subscription = subscription,
                    Accepted = false,
                    InviteCode = Guid.NewGuid().ToString()
                };
                _repo.Db.Add(invite);
            }

            invite.ValidTill = DateTime.Now + TimeSpan.FromDays(30);
            invite.Accepted = false;
            subscription.ActiveUsersCount = subscription.ActiveUsersCount + 1;
            Check.Value(subscription.ActiveUsersCount, onError: e => e.ApiErrorCode(ErrorCodes.SubscriptionMembersViolation))
                .LessOrEqualTo(subscription.MaximumUsersCount, "Subscription users limit ({Max}) reached");

            return invite;
        }

        private void CheckNotLastAdmin(Model.SubscriptionMember targetMember, string message)
        {
            //does it makes sense to use Count() here? will it be faster? (averbin)
            var otherAdmins = _repo.SubscriptionAdmins(targetMember.SubscriptionId)
                .Take(2);
            Check.Value(otherAdmins.Count(), onError:AvendErrors.Forbidden)
                .Greater(1, message);
        }
    }
}