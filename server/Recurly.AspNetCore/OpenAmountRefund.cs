using System.Xml;
using Recurly.AspNetCore.Extensions;

namespace Recurly.AspNetCore
{
    class OpenAmountRefund : RecurlyEntity
    {
        public int AmountInCents { get; protected set; }
        private Invoice.RefundOrderPriority RefundPriority;

        internal OpenAmountRefund(int amountInCents, Invoice.RefundOrderPriority refundPriority = Invoice.RefundOrderPriority.Credit)
        {
            AmountInCents = amountInCents;
            RefundPriority = refundPriority;
        }

        internal override void ReadXml(XmlReader reader)
        {
            throw new System.NotImplementedException();
        }

        internal override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("invoice");
            writer.WriteElementString("refund_apply_order", RefundPriority.ToString().ToLower());
            writer.WriteElementString("amount_in_cents", AmountInCents.AsString());
            writer.WriteEndElement();
        }
    }
}
