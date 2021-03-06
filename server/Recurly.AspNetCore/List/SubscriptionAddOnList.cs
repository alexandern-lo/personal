﻿using System.Xml;

namespace Recurly.AspNetCore.List
{
    public class SubscriptionAddOnList : RecurlyList<SubscriptionAddOn>
    {
        private Subscription _subscription;

        public SubscriptionAddOnList(Subscription subscription)
        {
            _subscription = subscription;
        }

        public SubscriptionAddOnList(string url) : base(Client.HttpRequestMethod.Get, url)
        {
        }

        public override RecurlyList<SubscriptionAddOn> Start
        {
            get { return HasStartPage() ? new SubscriptionAddOnList(StartUrl) : RecurlyList.Empty<SubscriptionAddOn>(); }
        }

        public override RecurlyList<SubscriptionAddOn> Next
        {
            get { return HasNextPage() ? new SubscriptionAddOnList(NextUrl) : RecurlyList.Empty<SubscriptionAddOn>(); }
        }

        public override RecurlyList<SubscriptionAddOn> Prev
        {
            get { return HasPrevPage() ? new SubscriptionAddOnList(PrevUrl) : RecurlyList.Empty<SubscriptionAddOn>(); }
        }

        public override bool IncludeEmptyTag()
        {
            return true;
        }

        internal override void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.Name == "subscription_add_ons" && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "subscription_add_on")
                {
                    Add(new SubscriptionAddOn(reader));
                }
            }
        }

        /// <summary>
        /// Adds the given <see cref="T:Recurly.AspNetCore.AddOn"/> to the current Subscription.
        /// 
        /// Sample usage:
        /// <code>
        /// sub.AddOns.Add(planAddOn, quantity, unitInCents)
        /// sub.AddOns.Add(planAddOn, quantity) // unitInCents = planAddOn.UnitAmountInCents[this.Currency]
        /// sub.AddOns.Add(planAddOn) // default quantity = 1, unitInCents = planAddOn.UnitAmountInCents[this.Currency]
        /// </code>
        /// </summary>
        /// <param name="planAddOn">The <see cref="T:Recurly.AspNetCore.AddOn"/> to add to the current Subscription.</param>
        /// <param name="quantity">The quantity of the add-on. Optional, default is 1.</param>
        public void Add(AddOn planAddOn, int quantity = 1)
        {
            int amount;
            if (!planAddOn.UnitAmountInCents.TryGetValue(_subscription.Currency, out amount))
            {
                throw new ValidationException(
                    "The given AddOn does not have UnitAmountInCents for the currency of the subscription (" + _subscription.Currency + ")."
                    , null);
            }
            var sub = new SubscriptionAddOn(planAddOn.AddOnCode, amount, quantity);
            base.Add(sub);
        }

        /// <summary>
        /// Adds the given <see cref="T:Recurly.AspNetCore.AddOn"/> to the current Subscription.
        /// 
        /// Sample usage:
        /// <code>
        /// sub.AddOns.Add(planAddOn, quantity, unitInCents)
        /// sub.AddOns.Add(planAddOn, quantity) // unitInCents = planAddOn.UnitAmountInCents[this.Currency]
        /// sub.AddOns.Add(planAddOn) // default quantity = 1, unitInCents = planAddOn.UnitAmountInCents[this.Currency]
        /// </code>
        /// </summary>
        /// <param name="planAddOn">The <see cref="T:Recurly.AspNetCore.AddOn"/> to add to the current Subscription.</param>
        /// <param name="quantity">The quantity of the add-on. Optional, default is 1.</param>
        /// <param name="unitAmountInCents">Overrides the UnitAmountInCents of the add-on.</param>
        public void Add(AddOn planAddOn, int quantity, int unitAmountInCents)
        {
            var sub = new SubscriptionAddOn(planAddOn.AddOnCode, unitAmountInCents, quantity);
            base.Add(sub);
        }

        // sub.AddOns.Add(code, quantity, unitInCents);
        // sub.AddOns.Add(code, quantity); unitInCents=this.Plan.UnitAmountInCents[this.Currency]
        // sub.AddOns.Add(code); 1, unitInCents=this.Plan.UnitAmountInCents[this.Currency]
        public void Add(string planAddOnCode, int quantity=1)
        {
            var unitAmount = _subscription.Plan.AddOns.Find(ao => ao.AddOnCode == planAddOnCode).UnitAmountInCents[_subscription.Currency];
            var sub = new SubscriptionAddOn(planAddOnCode, unitAmount, quantity);
            base.Add(sub);
        }
        public void Add(string planAddOnCode, int quantity, int unitAmountInCents)
        {
            var sub = new SubscriptionAddOn(planAddOnCode, unitAmountInCents, quantity);
            base.Add(sub);
        }
    }
}
