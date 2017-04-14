using System.Runtime.Serialization;

namespace Avend.API.Model.NetworkDTO
{
    [DataContract(Name = "money")]
    public class MoneyDto
    {
        [DataMember(Name = "amount", IsRequired = true)]
        public decimal Amount { get; set; }

        [DataMember(Name = "currency", IsRequired = true)]
        public CurrencyCode? Currency { get; set; }
    }
}
