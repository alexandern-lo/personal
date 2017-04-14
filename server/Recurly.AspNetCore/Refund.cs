using System.Xml;
using Recurly.AspNetCore.Extensions;

namespace Recurly.AspNetCore
{
    public class Refund : RecurlyEntity
    {
        public bool Prorate { get; protected set; }
        public int Quantity { get; protected set; }
        public string Uuid { get; protected set; }

        internal Refund(Adjustment adjustment, bool prorate, int quantity)
        {
            Prorate = prorate;
            Quantity = quantity;
            Uuid = adjustment.Uuid;
        }

        internal override void ReadXml(XmlReader reader)
        {
            throw new System.NotImplementedException();
        }

        internal override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("adjustment");

            writer.WriteElementString("uuid", Uuid);
            writer.WriteElementString("quantity", Quantity.AsString());
            writer.WriteElementString("prorate", Prorate.AsString());

            writer.WriteEndElement(); // adjustment
        }
    }
}
