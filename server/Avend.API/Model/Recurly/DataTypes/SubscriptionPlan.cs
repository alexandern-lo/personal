using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Avend.API.Model.Recurly.DataTypes
{
    public class SubscriptionPlan
    {
        [XmlElement("plan_code", IsNullable = true)]
        [DataMember(Name = "plan_code")]
        public string Code { get; set; }

        [XmlElement("name", DataType = "string", IsNullable = true)]
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}