using System;
using System.Linq.Expressions;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;

namespace Avend.API.Services.Events
{
    public static class EventAccessExtensions
    {
        public static Expression<Func<EventRecord, bool>> ConferenceEvents(this IAvendPrincipal user)
        {
            return e => e.Type == EventRecord.EventTypeConference;
        }

        public static Expression<Func<EventRecord, bool>> OwnEvents(this IAvendPrincipal user)
        {
            switch (user.Role)
            {
                case UserRole.SeatUser:
                    return e => e.OwnerId == user.UserId;
                case UserRole.Admin:
                    return e => e.SubscriptionId == user.TenantId;
                case UserRole.SuperAdmin:
                    return e => true;
                default:
                    return e => false;
            }
        }

        public static Expression<Func<EventRecord, bool>> SelectableEvents(this IAvendPrincipal user)
        {
            return e => e.Type == EventRecord.EventTypeConference || e.OwnerId == user.UserId;
        }

        public static Expression<Func<EventRecord, bool>> AvailableEvents(this IAvendPrincipal user)
        {
            switch (user.Role)
            {
                case UserRole.SeatUser:
                    return e => e.Type == EventRecord.EventTypeConference || e.OwnerId == user.UserId;
                case UserRole.Admin:
                    return e => e.Type == EventRecord.EventTypeConference || e.SubscriptionId == user.TenantId;
                case UserRole.SuperAdmin:
                    return e => true;
                default:
                    return e => false;
            }
        }
    }
}