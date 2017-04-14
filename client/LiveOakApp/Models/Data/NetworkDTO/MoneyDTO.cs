using System;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract(Name = "money")]
    public class MoneyDTO
    {
        public enum MoneyCurrency
        {
            USD
        }

        [DataMember(Name = "amount", IsRequired = true)]
        public decimal Amount { get; set; }

        [DataMember(Name = "currency", IsRequired = true)]
        string Currency { get; set; }

        public MoneyCurrency CurrencyEnum
        {
            get
            {
                switch (Currency.ToLower())
                {
                    case "usd": return MoneyCurrency.USD;
                }
                return MoneyCurrency.USD;
            }
            set
            {
                Currency = value.ToString().ToLower();
            }
        }
    }
}
