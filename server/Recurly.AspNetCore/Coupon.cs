﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Recurly.AspNetCore.Extensions;
using Recurly.AspNetCore.List;

namespace Recurly.AspNetCore
{
    public class Coupon : RecurlyEntity
    {
        public enum CouponState : short
        {
            All = 0,
            Redeemable,
            Expired,
            Inactive,
            MaxedOut
        }

        public enum CouponDiscountType
        {
            Percent,
            Dollars
        }

        public enum CouponDuration
        {
            Forever,
            SingleUse,
            Temporal
        }

        public enum CouponTemporalUnit
        {
            Day,
            Week,
            Month,
            Year
        }

        public enum RedemptionResourceType
        {
            Account,
            Subscription
        }

        public enum CouponType
        {
            Bulk,
            SingleCode,
            UniqueCode
        }

        public RecurlyList<CouponRedemption> Redemptions { get; private set; }

        public string CouponCode { get; set; }
        public string Name { get; set; }
        public string HostedDescription { get; set; }
        public string InvoiceDescription { get; set; }
        public DateTime? RedeemByDate { get; set; }
        public bool? SingleUse { get; set; }
        public int? AppliesForMonths { get; set; }
        public CouponDuration? Duration { get; set; }
        public CouponTemporalUnit? TemporalUnit { get; set; }
        public int? TemporalAmount { get; set; }
        public int? MaxRedemptions { get; set; }
        public bool? AppliesToAllPlans { get; set; }
        public bool? AppliesToNonPlanCharges { get; set; }
        public int? MaxRedemptionsPerAccount { get; set; }
        public string UniqueCodeTemplate { get; set; }

        public CouponDiscountType DiscountType { get; private set; }
        public CouponState State { get; private set; }
        public RedemptionResourceType RedemptionResource { get; set; }
        public CouponType Type { get; set; } = CouponType.SingleCode;

        /// <summary>
        /// A dictionary of currencies and discounts
        /// </summary>
        public Dictionary<string, int> DiscountInCents { get; private set; }
        public int? DiscountPercent { get; private set; }

        private int? NumberOfUniqueCodes { get; set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private string memberUrl()
        {
            return UrlPrefix + CouponCode;
        }

        /// <summary>
        /// A list of plans to limit the coupon
        /// </summary>

        private List<string> _plans;

        public List<string> Plans
        {
            get { return _plans ?? (_plans = new List<string>()); }
        }

        #region Constructors

        internal Coupon()
        {
        }

        internal Coupon(XmlReader xmlReader)
        {
            ReadXml(xmlReader);
        }

        /// <summary>
        /// Creates a coupon, discounted by a fixed amount
        /// </summary>
        /// <param name="couponCode"></param>
        /// <param name="name"></param>
        /// <param name="discountInCents">dictionary of currencies and discounts</param>
        public Coupon(string couponCode, string name, Dictionary<string, int> discountInCents)
        {
            CouponCode = couponCode;
            Name = name;
            DiscountInCents = discountInCents;
            DiscountType = CouponDiscountType.Dollars;
        }

        /// <summary>
        /// Creates a coupon, discounted by percentage
        /// </summary>
        /// <param name="couponCode"></param>
        /// <param name="name"></param>
        /// <param name="discountPercent"></param>
        public Coupon(string couponCode, string name, int discountPercent)
        {
            CouponCode = couponCode;
            Name = name;
            DiscountPercent = discountPercent;
            DiscountType = CouponDiscountType.Percent;
        }

        #endregion

        internal const string UrlPrefix = "/coupons/";

        /// <summary>
        /// Creates this coupon.
        /// </summary>
        public void Create()
        {
            Client.Instance.PerformRequest(Client.HttpRequestMethod.Post,
                UrlPrefix, 
                WriteXml,
                ReadXml);
        }

        public void Update()
        {
            Client.Instance.PerformRequest(Client.HttpRequestMethod.Put,
                UrlPrefix + Uri.EscapeUriString(CouponCode),
                WriteXmlUpdate);
        }

        public void Restore()
        {
            Client.Instance.PerformRequest(Client.HttpRequestMethod.Put,
                UrlPrefix + Uri.EscapeUriString(CouponCode) + "/restore",
                WriteXmlUpdate);
        }

        /// <summary>
        /// Deactivates this coupon.
        /// </summary>
        public void Deactivate()
        {
            Client.Instance.PerformRequest(Client.HttpRequestMethod.Delete,
                UrlPrefix + Uri.EscapeUriString(CouponCode));
        }

        public async Task<RecurlyList<Coupon>> GetUniqueCouponCodes()
        {
            var coupons = new CouponList();

            var statusCode = await Client.Instance.PerformRequest(Client.HttpRequestMethod.Get,
                memberUrl() + "/unique_coupon_codes/",
                coupons.ReadXmlList);

            return statusCode == HttpStatusCode.NotFound ? null : coupons;
        }

        public async Task<RecurlyList<Coupon>> Generate(int amount)
        {
            NumberOfUniqueCodes = amount;
            var coupons = new CouponList();

            var statusCode = await Client.Instance.PerformRequest(Client.HttpRequestMethod.Post,
                memberUrl() + "/generate/",
                this.WriteGenerateXml,
                async response => await coupons.ReadFromLocation(response));

            return statusCode == HttpStatusCode.NotFound ? null : coupons;
        }


        #region Read and Write XML documents

        internal override void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                // End of coupon element, get out of here
                if (reader.Name == "coupon" && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if (reader.NodeType != XmlNodeType.Element) continue;

                DateTime date;
                int m;
                switch (reader.Name)
                {
                    case "coupon_code":
                        CouponCode = reader.ReadElementContentAsString();
                        break;
                     
                    case "name":
                        Name = reader.ReadElementContentAsString();
                        break;

                    case "state":
                        State = reader.ReadElementContentAsString().ParseAsEnum<CouponState>();
                        break;

                    case "discount_type":
                        DiscountType = reader.ReadElementContentAsString().ParseAsEnum<CouponDiscountType>();
                        break;

                    case "redemption_resource":
                        RedemptionResource = reader.ReadElementContentAsString().ParseAsEnum<RedemptionResourceType>();
                        break;

                    case "discount_percent":
                        DiscountPercent = reader.ReadElementContentAsInt();
                        break;

                    case "redeem_by_date":
                        if (DateTime.TryParse(reader.ReadElementContentAsString(), out date))
                            RedeemByDate = date;
                        break;

                    case "single_use":
                        SingleUse = reader.ReadElementContentAsBoolean();
                        break;

                    case "applies_for_months":
                        if (int.TryParse(reader.ReadElementContentAsString(), out m))
                            AppliesForMonths = m;
                        break;

                    case "duration":
                        Duration = reader.ReadElementContentAsString().ParseAsEnum<CouponDuration>();
                        break;

                    case "temporal_unit":
                        var element_content = reader.ReadElementContentAsString();
                        if (element_content != "")
                          TemporalUnit = element_content.ParseAsEnum<CouponTemporalUnit>();
                        break;

                    case "temporal_amount":
                        if (int.TryParse(reader.ReadElementContentAsString(), out m))
                            TemporalAmount = m;
                        break;

                    case "max_redemptions":
                        if (int.TryParse(reader.ReadElementContentAsString(), out m))
                            MaxRedemptions = m;
                        break;

                    case "applies_to_all_plans":
                        AppliesToAllPlans = reader.ReadElementContentAsBoolean();
                        break;

                    case "applies_to_non_plan_charges":
                        AppliesToNonPlanCharges = reader.ReadElementContentAsBoolean();
                        break;

                    case "max_redemptions_per_account":
                        if (int.TryParse(reader.ReadElementContentAsString(), out m))
                            MaxRedemptionsPerAccount = m;
                        break;

                    case "description":
                        HostedDescription = reader.ReadElementContentAsString();
                        break;

                    case "invoice_description":
                        InvoiceDescription = reader.ReadElementContentAsString();
                        break;

                    case "unique_code_template":
                        UniqueCodeTemplate = reader.ReadElementContentAsString();
                        break;

                    case "coupon_type":
                        var type_content = reader.ReadElementContentAsString();
                        if (type_content != "")
                          Type = type_content.ParseAsEnum<CouponType>();
                        break;

                    case "created_at":
                        CreatedAt = reader.ReadElementContentAsDateTime();
                        break;

                    case "updated_at":
                        UpdatedAt = reader.ReadElementContentAsDateTime();
                        break;

                    case "plan_codes":
                        ReadXmlPlanCodes(reader);
                        break;

                    case "discount_in_cents":
                        ReadXmlDiscounts(reader);
                        break;
                        
                }
            }
        }

        internal void ReadXmlPlanCodes(XmlReader reader)
        {
            Plans.Clear();

            while (reader.Read())
            {
                if (reader.Name == "plan_codes" && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if (reader.NodeType != XmlNodeType.Element) continue;
                switch (reader.Name)
                {
                    case "plan_code":
                        Plans.Add(reader.ReadElementContentAsString());
                        break;

                }
            }
        }

        internal void ReadXmlDiscounts(XmlReader reader)
        {            
            DiscountInCents = new Dictionary<string, int>();

            while (reader.Read())
            {
                if (reader.Name == "discount_in_cents" && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    DiscountInCents.Add(reader.Name, reader.ReadElementContentAsInt());
                }
            }
        }

        internal override void WriteXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("coupon"); // Start: coupon

            xmlWriter.WriteElementString("coupon_code", CouponCode);
            xmlWriter.WriteElementString("name", Name);
            xmlWriter.WriteElementString("hosted_description", HostedDescription);
            xmlWriter.WriteElementString("invoice_description", InvoiceDescription);

            if (RedeemByDate.HasValue)
                xmlWriter.WriteElementString("redeem_by_date", RedeemByDate.Value.ToString("s"));

            if (SingleUse.HasValue)
                xmlWriter.WriteElementString("single_use", SingleUse.Value.AsString());

            if (AppliesForMonths.HasValue)
                xmlWriter.WriteElementString("applies_for_months", AppliesForMonths.Value.AsString());
            if (Duration != null)
              xmlWriter.WriteElementString("duration", Duration.ToString().EnumNameToTransportCase());
            if (TemporalUnit != null)
              xmlWriter.WriteElementString("temporal_unit", TemporalUnit.ToString().EnumNameToTransportCase());
            if (TemporalAmount.HasValue)
                xmlWriter.WriteElementString("temporal_amount", TemporalAmount.Value.ToString());

            if (AppliesToAllPlans.HasValue)
                xmlWriter.WriteElementString("applies_to_all_plans", AppliesToAllPlans.Value.AsString());

            if (AppliesToNonPlanCharges.HasValue)
                xmlWriter.WriteElementString("applies_to_non_plan_charges", AppliesToNonPlanCharges.Value.AsString());

            if(MaxRedemptions.HasValue)
                xmlWriter.WriteElementString("max_redemptions", MaxRedemptions.Value.AsString());

            if (MaxRedemptionsPerAccount.HasValue)
                xmlWriter.WriteElementString("max_redemptions_per_account", MaxRedemptionsPerAccount.Value.AsString());

            xmlWriter.WriteElementString("discount_type", DiscountType.ToString().EnumNameToTransportCase());

            xmlWriter.WriteElementString("redemption_resource", RedemptionResource.ToString().EnumNameToTransportCase());

            xmlWriter.WriteElementString("coupon_type", Type.ToString().EnumNameToTransportCase());

            if (Type == CouponType.Bulk)
                xmlWriter.WriteElementString("unique_code_template", UniqueCodeTemplate);   

            if (CouponDiscountType.Percent == DiscountType && DiscountPercent.HasValue)
                xmlWriter.WriteElementString("discount_percent", DiscountPercent.Value.AsString());

            if (CouponDiscountType.Dollars == DiscountType)
            {
                xmlWriter.WriteIfCollectionHasAny("discount_in_cents", DiscountInCents, pair => pair.Key,
                    pair => pair.Value.AsString());
            }

            xmlWriter.WriteIfCollectionHasAny("plan_codes", Plans, s => "plan_code", s => s);

            xmlWriter.WriteEndElement(); // End: coupon
        }


        public void WriteGenerateXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("coupon"); // Start: coupon
            xmlWriter.WriteElementString("number_of_unique_codes", NumberOfUniqueCodes.Value.AsString());
            xmlWriter.WriteEndElement(); // End: coupon
        }

        internal void WriteXmlUpdate(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("coupon"); // Start: coupon

            if (!Name.IsNullOrEmpty())
                xmlWriter.WriteElementString("name", Name);

            if (!HostedDescription.IsNullOrEmpty())
                xmlWriter.WriteElementString("hosted_description", HostedDescription);

            if (!InvoiceDescription.IsNullOrEmpty())
                xmlWriter.WriteElementString("invoice_description", InvoiceDescription);

            if (RedeemByDate.HasValue)
                xmlWriter.WriteElementString("redeem_by_date", RedeemByDate.Value.ToString("s"));

            if (MaxRedemptions.HasValue)
                xmlWriter.WriteElementString("max_redemptions", MaxRedemptions.Value.AsString());

            if (MaxRedemptionsPerAccount.HasValue)
                xmlWriter.WriteElementString("max_redemptions_per_account", MaxRedemptionsPerAccount.Value.AsString());

            xmlWriter.WriteEndElement(); // End: coupon
        }

        #endregion

        #region Object Overrides

        public override string ToString()
        {
            return "Recurly Account Coupon: " + CouponCode;
        }

        public override bool Equals(object obj)
        {
            var coupon = obj as Coupon;
            return coupon != null && Equals(coupon);
        }

        public bool Equals(Coupon coupon)
        {
            return CouponCode == coupon.CouponCode;
        }

        public override int GetHashCode()
        {
            return CouponCode.GetHashCode();
        }

        #endregion
    }

    public sealed class Coupons
    {
        /// <summary>
        /// Look up a coupon
        /// </summary>
        /// <param name="couponCode">Coupon code</param>
        /// <returns></returns>
        public static async Task<Coupon> Get(string couponCode)
        {
            var coupon = new Coupon();

            var statusCode = await Client.Instance.PerformRequest(Client.HttpRequestMethod.Get,
                urlPath: Coupon.UrlPrefix + Uri.EscapeUriString(couponCode),
                readXmlDelegate: coupon.ReadXml);

            return statusCode == HttpStatusCode.NotFound ? null : coupon;
        }

        /// <summary>
        /// Lists coupons, limited to state
        /// </summary>
        /// <param name="state">Account state to retrieve</param>
        /// <returns></returns>
        public static RecurlyList<Coupon> List(Coupon.CouponState state = Coupon.CouponState.All)
        {
            return new CouponList(Coupon.UrlPrefix + (state != Coupon.CouponState.All ? "?state=" + state.ToString().EnumNameToTransportCase() : ""));
        }
    }
}
