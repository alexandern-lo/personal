using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Services.Leads.NetworkDTO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.LeadsController
{
    [TestClass]
    [TestCategory("Integrational")]
    [TestCategory("LeadsController")]
    [TestCategory("LeadsController.CreateLead()")]
    // ReSharper disable once InconsistentNaming
    public class Leads_CreateLead : BaseLeadsEndpointTest
    {
        protected LeadDto LeadRequest;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
        }

        [TestMethod]
        public async Task Create()
        {
            var leadDto = new LeadDto()
            {
                EventUid = ConferenceEventData.Event.Uid,
                FirstName = "Konstantin",
                Notes = "Test",
                Emails = new List<LeadEmailDto>()
                {
                    new LeadEmailDto()
                    {
                        Designation = "work",
                        Email = "test_user@test.com"
                    }
                }
            };

            var newLeadDto = await BobTA.PostJsonAsync(UrlApiV1Leads, leadDto).AvendResponse<LeadDto>();

            newLeadDto.Should().NotBeNull("because we have just added a single Lead DTO");
            newLeadDto.Owner.Uid.Should().Be(TestUser.BobTester.Uid, "because we have used Bob account to add new lead");
            newLeadDto.Tenant.Uid.Should().Be(BobSubscriptionUid,"because we have used Bob account to add new lead");
            newLeadDto.Event.Uid.Should().Be(leadDto.EventUid, "because we have explicitly set event uid for the new lead");
            newLeadDto.FirstName.Should().Be(leadDto.FirstName, "because we have explicitly set first name for the new lead");
            newLeadDto.Notes.Should().Be(leadDto.Notes, "because we have explicitly set notes for the new lead");
            newLeadDto.Emails.Count.Should().Be(1, "because we have explicitly set single email for the new lead");
            newLeadDto.Emails[0].Email.Should()
                .Be(leadDto.Emails[0].Email, "because we have explicitly set the email value for the new lead");
        }

        [TestMethod]
        public async Task GenerateFirstName()
        {
            var leadDto = new LeadDto()
            {
                EventUid = ConferenceEventData.Event.Uid,
                Emails = new List<LeadEmailDto>()
                {
                    new LeadEmailDto()
                    {
                        Designation = "work",
                        Email = "test_user@test.com"
                    }
                }
            };
            var newLeadDto = await BobTA.PostJsonAsync(UrlApiV1Leads, leadDto).AvendResponse<LeadDto>();

            newLeadDto.FirstName.Should().Be(leadDto.Emails[0].Email);
        }

        [TestMethod]
        public async Task NoEmail()
        {
            var leadDto = new LeadDto()
            {
                EventUid = ConferenceEventData.Event.Uid,
                FirstName = "Konstantin",
            };
            await BobTA.PostJsonAsync(UrlApiV1Leads, leadDto).Response();
        }

        [TestMethod]
        public async Task ShouldReturnProperUidWhenDuplicateClientsideUidIsPassed()
        {
            var leadDto = new LeadDto()
            {
                EventUid = ConferenceEventData.Event.Uid,
                FirstName = "Konstantin",
                Notes = "Test",
                ClientsideUid = Guid.NewGuid(),
            };
            await BobTA.PostJsonAsync(UrlApiV1Leads, leadDto).Response();
            await BobTA.PostJsonAsync(UrlApiV1Leads, leadDto).Response(HttpStatusCode.SeeOther);
        }

        [TestMethod]
        public async Task EventUidDoesNotExist()
        {
            var leadDto = new LeadDto()
            {
                EventUid = Guid.Empty,
                FirstName = "Konstantin",
                Notes = "Test",
                Emails = new List<LeadEmailDto>()
                {
                    new LeadEmailDto()
                    {
                        Designation = "work",
                        Email = "test_user@test.com"
                    }
                }
            };

            await BobTA.PostJsonAsync(UrlApiV1Leads, leadDto)
                .Response(HttpStatusCode.BadRequest, "lead.event_uid is not found");
        }

        [TestMethod]
        public async Task EventUidIsNotSelectableByUser()
        {
            await CecileEventData.AddFromSample();

            var leadDto = new LeadDto()
            {
                EventUid = CecileEventData.Event.Uid,
                FirstName = "Konstantin",
                Notes = "Test",
                Emails = new List<LeadEmailDto>()
                {
                    new LeadEmailDto()
                    {
                        Designation = "work",
                        Email = "test_user@test.com"
                    }
                }
            };

            await BobTA.PostJsonAsync(UrlApiV1Leads, leadDto)
                .Response(HttpStatusCode.BadRequest, "lead.event_uid is not found");
        }

        [TestMethod]
        public async Task ShouldReturnBadRequestWhenBothFirstNameAndEmailPresentButNoEvent()
        {
            var leadDto = new LeadDto()
            {
                EventUid = null,
                FirstName = "Konstantin",
                Notes = "Test",
                Emails = new List<LeadEmailDto>()
                {
                    new LeadEmailDto()
                    {
                        Designation = "work",
                        Email = "test_user@test.com"
                    }
                }
            };

            await BobTA.PostJsonAsync(UrlApiV1Leads, leadDto)
                .Response(HttpStatusCode.BadRequest, "Either lead.first_name or lead.emails should be not empty");
        }
    }
}