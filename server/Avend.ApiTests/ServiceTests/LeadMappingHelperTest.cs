using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure;
using Avend.API.Model;
using Avend.API.Services.Helpers;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ServiceTests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class LeadMappingHelperTest
    {
        /// <summary>
        /// Auxiliary method to construct a sample lead record with given 
        /// user UID based on DTO sample from LeadData helper class.
        /// <br /><br />
        /// Doesn't actually adds that Lead record to the database.
        /// </summary>
        /// 
        /// <param name="userUid">User's UID to assign to this lead</param>
        /// 
        /// <returns>Valid sample Lead record, not actually existing in database</returns>
        private static LeadRecord ConstructSampleLead(Guid userUid)
        {
            var eventUid = Guid.NewGuid();
            var sampleLeadDto = LeadData.CreateSample(userUid, eventUid);

            var lead = new LeadRecord();
            sampleLeadDto.UpdateLead(lead);

            return lead;
        }

        [DataTestMethod]
        [DataRow("Uid", (string)null)]
        [DataRow("questions_and_answers", "")]
        [DataRow("notes", "Sample notes\nBla-Bla")]
        public void GetPropertyFromLeadTest(string leadField, string value)
        {            
            var lead = ConstructSampleLead(TestUser.BobTester.Uid);
            var propertyFromLead = LeadMappingHelper.GetDtoPropertyFromLead(lead, leadField);

            if (value != null)
            {
                propertyFromLead.Should()
                    .NotBeNull("because '" + leadField + "' property should be retrieved from lead record properly");
            }

            propertyFromLead.Should()
                .Be(value, "because '" + leadField + "' property should have the correct value of the lead record field.");
        }

        [TestMethod]
        public async Task GetSalesForceMappedLeadTest()
        {
            var crm = CrmData.Init(TestUser.BobTester, TestSystem.Instance);
            var lead = ConstructSampleLead(TestUser.BobTester.Uid);
            var userCrmConfiguration = await crm.AuthorizeSalesForce();
            var mappedLead = LeadMappingHelper.GetMappedLead(lead, userCrmConfiguration);
            foreach (var mappedField in mappedLead)
            {
                var key = mappedField.Key;
                CrmDefaultsHelper.DefaultCrmMappings[CrmSystemAbbreviation.Salesforce].Should()
                    .ContainValue(key, "because '" + mappedField + "' property should be mapped from lead record properly");
            }

            mappedLead.Should()
                .ContainKey("FirstName", "because 'FirstName' property should be posted to SalesForce from lead record properly");

            mappedLead["FirstName"].Should()
                .Be(lead.FirstName, "because 'FirstName' property should be posted to SalesForce from lead record properly");

            mappedLead.Should()
                .ContainKey("LastName", "because 'LastName' property should be posted to SalesForce from lead record properly");

            mappedLead["LastName"].Should()
                .Be(lead.LastName, "because 'LastName' property should be posted to SalesForce from lead record properly");

            mappedLead.Should()
                .ContainKey("Phone", "because 'LastName' property should be posted to SalesForce from lead record properly");

            mappedLead["Phone"].Should()
                .Be(lead.Phones[0].Phone, "because 'LastName' property should be posted to SalesForce from lead record properly");

            mappedLead.Should()
                .ContainKey("Email", "because 'LastName' property should be posted to SalesForce from lead record properly");

            mappedLead["Email"].Should()
                .Be(lead.Emails[0].Email, "because 'LastName' property should be posted to SalesForce from lead record properly");
        }

        [TestMethod]
        public void AdjustSalesForceFieldsMappingTest()
        {
            var mapping = new Dictionary<string, object>()
            {
                {"notes", "Description"},
                {"first_name", false},
                {"last_name", null},
                {"company_name", true},
                {"company_url", "Website"},
                {"job_title", "Title"},
                {"zip_code", "PostalCode"},
                {"address", "Street"},
                {"city", "City"},
                {"state", "State"},
                {"country", "Country"},
                {"qualification", "Rating"},
                {"work_phone1", "Phone"},
            };

            var adjustedMapping = LeadMappingHelper.AdjustFieldsMapping(mapping, CrmSystemAbbreviation.Salesforce);

            foreach (var key in mapping.Keys)
            {
                adjustedMapping.Should()
                    .ContainKey(key, "because all keys are valid");
            }

            adjustedMapping["company_name"].Should()
                .Be(CrmDefaultsHelper.DefaultCrmMappings[CrmSystemAbbreviation.Salesforce]["company_name"], "because company_name is enabled by passing true");
        }
    }
}
