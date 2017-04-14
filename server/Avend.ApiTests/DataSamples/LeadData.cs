using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avend.ApiTests.Infrastructure;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model;
using Avend.API.Services.Leads.NetworkDTO;

using Microsoft.EntityFrameworkCore;

namespace Avend.ApiTests.DataSamples
{
    public class LeadData
    {
        public static LeadData Init(TestUser user, Guid eventUid, TestSystem system)
        {
            return new LeadData()
            {
                User = user,
                EventUid = eventUid,
                System = system,
                Leads = new List<LeadDto>()
            };
        }

        public static async Task<LeadData> Init(TestUser user, TestSystem system)
        {
            var eventData = await EventData.InitWithSampleEvent(user, system);
            return new LeadData()
            {
                User = user,
                EventUid = eventData.Event.Uid.GetValueOrDefault(),
                System = system,
                Leads = new List<LeadDto>()
            };
        }

        public Guid EventUid { get; private set; }
        public List<LeadDto> Leads { get; private set; }
        public TestSystem System { get; private set; }
        public TestUser User { get; private set; }

        public async Task<LeadDto> Add(Action<LeadDto> postProcessor = null)
        {
            var lead = CreateSample();
            postProcessor?.Invoke(lead);
            return await Send(lead);
        }

        public async Task<LeadDto> Add(Guid eventUid)
        {
            var lead = CreateSample(eventUid);
            
            return await Send(lead);
        }

        private async Task<LeadDto> Send(LeadDto lead)
        {
            using (var http = System.CreateClient(User.Token))
            {
                var leadDto = await http.PostJsonAsync("leads", lead).AvendResponse<LeadDto>();
                Leads.Add(leadDto);
                return leadDto;
            }
        }

        public LeadRecord GetDbRecord(int idx)
        {
            using (var services = System.GetServices())
            {
                var db = services.GetService<AvendDbContext>();
                var lead = Leads[idx];

                return (from l in db.LeadsTable where l.Uid == lead.Uid.GetValueOrDefault() select l)
                    .Include(x => x.ExportStatuses)
                    .Include(x => x.Emails)
                    .Include(x => x.Event)
                    .Include(x => x.Phones)                    
                    .Include(x => x.QuestionAnswers)
                    .First();
            }
        }

        public LeadRecord UpdateDbRecordCreationTime(int idx, DateTime newDateTime)
        {
            var lead = Leads[idx];

            return UpdateDbRecordCreationTime(lead, newDateTime);
        }

        public LeadRecord UpdateDbRecordCreationTime(LeadDto lead, DateTime newDateTime)
        {
            LeadRecord leadRecord;

            using (var services = System.GetServices())
            {
                var db = services.GetService<AvendDbContext>();

                leadRecord = db.LeadsTable.First(l => l.Uid == lead.Uid.GetValueOrDefault());

                leadRecord.CreatedAt = newDateTime;

                db.SaveChanges();
            }

            return leadRecord;
        }

        public LeadDto CreateSample(Guid? eventUid = null)
        {
            return CreateSample(User.Uid, eventUid ?? EventUid);
        }

        public static LeadDto CreateSample(Guid userUid, Guid eventUid)
        {
            return new LeadDto()
            {
                FirstName = $"Lead FirstName {DateTime.Now}",
                LastName = "Lead LastName",
                OwnerUid = userUid,
                EventUid = eventUid,
                Address = "Test Address",
                City = "Test City",
                State = "Test State",
                Country = "Test Country",
                ZipCode = "123456",
                CompanyName = "Test Company",
                CompanyUrl = "http://www.company.com",
                Notes = "Sample notes\nBla-Bla",
                Emails = new List<LeadEmailDto>()
                {
                    new LeadEmailDto()
                    {
                        Designation = "work",
                        Email = "workemail1@email.com",
                    },
                    new LeadEmailDto()
                    {
                        Designation = "work",
                        Email = "workemail2@email.com",
                    },
                    new LeadEmailDto()
                    {
                        Designation = "home",
                        Email = "homeemail@email.com",
                    },
                },
                Phones = new List<LeadPhoneDto>()
                {
                    new LeadPhoneDto()
                    {
                        Designation = "work",
                        Phone = "+123455555",
                    },
                    new LeadPhoneDto()
                    {
                        Designation = "mobile",
                        Phone = "+123456666",
                    },
                    new LeadPhoneDto()
                    {
                        Designation = "home",
                        Phone = "+123456777",
                    },
                }
            };
        }
    }
}