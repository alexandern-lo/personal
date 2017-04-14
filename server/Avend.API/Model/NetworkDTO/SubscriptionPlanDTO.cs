using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Avend.API.Services;
using Recurly.AspNetCore;
using Recurly.AspNetCore.Configuration;

namespace Avend.API.Model.NetworkDTO
{
    /// <summary>
    /// Network DTO class for passing subscription plan information to frontend Web App.
    /// </summary>
    [DataContract(Name = "subscription_plan")]
    public class SubscriptionPlanDTO
    {
        /// <summary>
        /// Unique identifier representing the specific plan.
        /// </summary>
        /// <value>Get or sets the unique identifier representing the specific plan.</value>
        [DataMember(Name = "plan_code")]
        public string Code { get; set; }

        /// <summary>
        /// Plan's type.
        /// </summary>
        /// <value>Plan's type: either individual or corporate.</value>
        [DataMember(Name = "plan_type")]
        public string Type { get; set; }

        /// <summary>
        /// Number of seat users allowed under this plan.
        /// </summary>
        /// <value>Number of seat users allowed under this plan.</value>
        [DataMember(Name = "seats_number")]
        public int SeatsNumber { get; set; }

        /// <summary>
        /// Plan's name.
        /// </summary>
        /// <value>Plan's name.</value>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Plan's description.
        /// </summary>
        /// <value>Plan's description.</value>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Billing period.
        /// </summary>
        /// <value>Billing period.</value>
        [DataMember(Name = "billing_period")]
        public string BillingPeriod { get; set; }

        /// <summary>
        /// Billing prices with corresponding currencies.
        /// </summary>
        /// <value>Billing prices with corresponding currencies.</value>
        [DataMember(Name = "prices")]
        public List<SubscriptionPlanPriceDTO> Prices { get; set; }

        public static SubscriptionPlanDTO From(Plan planObj)
        {
            int seatsNumber;
            string planType;

            var parseRes = RecurlyService.TryParsePlanType(planObj.AccountingCode, out seatsNumber, out planType);

            if (!parseRes)
            {
                seatsNumber = 1;
                planType = "individual";
            }

            var dto = new SubscriptionPlanDTO()
            {
                Code = planObj.PlanCode,
                Type = planType,
                SeatsNumber = seatsNumber,

                Name = planObj.Name,
                Description = planObj.Description,

                BillingPeriod = GetIntervalString(planObj),
                Prices = new List<SubscriptionPlanPriceDTO>(),
            };

            foreach (var unitAmount in planObj.UnitAmountInCents)
            {
                var priceDto = new SubscriptionPlanPriceDTO()
                {
                    Currency = unitAmount.Key,
                    Price = new decimal(unitAmount.Value) / 100,
                };

                dto.Prices.Add(priceDto);
            }

            return dto;
        }

        private static string GetIntervalString(Plan planObj)
        {
            var interval = planObj.PlanIntervalLength + " ";

            switch (planObj.PlanIntervalUnit)
            {
                case Plan.IntervalUnit.Days:
                    switch (planObj.PlanIntervalLength)
                    {
                        case 1:
                            interval += "day";
                            break;
                        case 7:
                            interval = "1 week";
                            break;
                        default:
                            interval += "days";
                            break;
                    }
                    break;

                case Plan.IntervalUnit.Months:
                    switch (planObj.PlanIntervalLength)
                    {
                        case 1:
                            interval += "month";
                            break;
                        case 12:
                            interval = "1 year";
                            break;
                        default:
                            interval += "months";
                            break;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return interval;
        }
    }

    /// <summary>
    /// Network DTO class for passing subscription plan price information to frontend Web App.
    /// </summary>
    [DataContract(Name = "subscription_plan")]
    public class SubscriptionPlanPriceDTO
    {
        /// <summary>
        /// Plan price currency.
        /// </summary>
        /// <value>Plan price currency.</value>
        [DataMember(Name = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Plan price.
        /// </summary>
        /// <value>Plan price.</value>
        [DataMember(Name = "price")]
        public decimal Price { get; set; }
    }
}