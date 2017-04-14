using System;
using System.Runtime.Serialization;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;

namespace Avend.API.Services.Dashboard.NetworkDTO
{
    [DataContract]
    public class DashboardEventDTO
    {
        [DataMember(Name = "event_uid")]
        public Guid Uid { get; set; }

        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; set; }

        [DataMember(Name = "website_url")]
        public string WebsiteUrl { get; set; }

        [DataMember(Name = "leads_goal")]
        public int LeadsGoal { get; set; }

        [DataMember(Name = "leads_count")]
        public int LeadsCount { get; set; }

        [DataMember(Name = "total_expenses")]
        public MoneyDto TotalExpenses { get; set; }

        public static DashboardEventDTO From(EventUserGoalsRecord record, Guid fallbackEventUid, string fallbackEventName, string fallbackWebsiteUrl, CurrencyCode currency, decimal expenses, int? count)
        {
            var dto = new DashboardEventDTO()
            {
                Uid = record.Event?.Uid ?? fallbackEventUid,
                Name = record.Event?.Name ?? fallbackEventName,
                WebsiteUrl = record?.Event?.WebsiteUrl ?? fallbackWebsiteUrl,

                LeadsGoal = record.LeadsGoal,
                LeadsCount = count ?? record.LeadsAcquired,

                TotalExpenses = new MoneyDto()
                {
                    Currency = currency != CurrencyCode.Unknown ? currency : CurrencyCode.USD,
                    Amount = expenses,
                },
            };

            return dto;
        }

        public static DashboardEventDTO From(EventRecord eventRecord, CurrencyCode currency, decimal expenses, int count)
        {
            var dto = new DashboardEventDTO()
            {
                Uid = eventRecord.Uid,
                Name = eventRecord.Name,
                WebsiteUrl = eventRecord.WebsiteUrl,

                LeadsGoal = 0,
                LeadsCount = count,

                TotalExpenses = new MoneyDto()
                {
                    Currency = currency != CurrencyCode.Unknown ? currency : CurrencyCode.USD,
                    Amount = expenses,
                },
            };

            return dto;
        }
    }
}