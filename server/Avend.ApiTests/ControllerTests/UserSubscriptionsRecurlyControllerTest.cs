using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;

using Avend.ApiTests.Infrastructure;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests
{
    [TestClass]
    public class UserSubscriptionsRecurlyControllerTest : BaseControllerTest
    {
        private const string WebHookEndpointUrl = "/api/v1/users/subscriptions/recurly";

        private const string AccountCreatedXML = "account_new.xml";
        private const string AccountBillingXML = "account_billing.xml";
        private const string InvoiceCreatedXML = "invoice_new.xml";
        private const string InvoiceClosedXML = "invoice_closed.xml";
        private const string PaymentXML = "payment.xml";
        private const string SubscriptionCreatedXML = "subscription_new.xml";
        private const string SubscriptionRenewedXML = "subscription_renew.xml";
        private const string SubscriptionExpiredXML = "subscription_expired.xml";

        [DataTestMethod]
        [DataRow(AccountCreatedXML, "new_account_notification")]
        [DataRow(AccountBillingXML, "billing_info_updated_notification")]
        [DataRow(InvoiceCreatedXML, "new_invoice_notification")]
        [DataRow(InvoiceClosedXML, "closed_invoice_notification")]
        [DataRow(PaymentXML, "successful_payment_notification")]
        [DataRow(SubscriptionCreatedXML, "new_subscription_notification")]
        [DataRow(SubscriptionRenewedXML, "renew_subscription_notification")]
        [DataRow(SubscriptionExpiredXML, "expired_subscription_notification")]
        public void TestXmlResourcesValidity(string xmlFile, string expectedRootTagName)
        {
            XDocument doc = null;

            Action xmlReadingAction = () =>
            {
                using (var xmlFileStream = ResourcesHelper.GetTestSampleAsStream(xmlFile))
                {
                    doc = XDocument.Load(xmlFileStream);
                }
            };

            xmlReadingAction
                .ShouldNotThrow("because file '{0}' should be well-formed and readable", xmlFile);

            doc.Root.Name.LocalName.Should()
                .Be(expectedRootTagName,
                    "because root tag <{0}> in file {1} should match the expected <{2}>",
                    doc.Root.Name, xmlFile, expectedRootTagName);
        }

        [DataTestMethod]
        [DataRow(AccountCreatedXML, "data: \"New account\"")]
        [DataRow(AccountBillingXML, "data: \"Billing info updated\"")]
        [DataRow(InvoiceCreatedXML, "data: \"New invoice\"")]
        [DataRow(InvoiceClosedXML, "data: \"Closed invoice\"")]
        [DataRow(PaymentXML, "data: \"Payment\"")]
        [DataRow(SubscriptionCreatedXML, "data: \"New subscription\"")]
        [DataRow(SubscriptionRenewedXML, "data: \"Renewed subscription\"")]
        [DataRow(SubscriptionExpiredXML, "data: \"Expired subscription\"")]
        public async Task TestRecurlyWebHookEndpointForXmlFile(string xmlFile, string expectedResponseData)
        {
            var xmlFileStream = ResourcesHelper.GetTestSampleAsStream(xmlFile);
            var postContent = new StreamContent(xmlFileStream);
            postContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml; charset=utf-8");

            var postResponseMessage = await BobTA.PostAsync(WebHookEndpointUrl, postContent);
            var postResponseBody = await postResponseMessage.Content.ReadAsStringAsync();

            postResponseMessage.IsSuccessStatusCode.Should()
                .BeTrue("because we expect Recurly WebHook to complete succesfully with request {0}",
                    xmlFile);

            postResponseBody.Should()
                .Contain(expectedResponseData,
                    "because Recurly WebHook response should contain '{0}' for file '{1}'",
                    expectedResponseData, xmlFile);
        }
    }
}
