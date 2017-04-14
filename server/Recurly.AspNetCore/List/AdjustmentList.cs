using System.Xml;

namespace Recurly.AspNetCore.List
{
    public class AdjustmentList : RecurlyList<Adjustment>
    {
        public override RecurlyList<Adjustment> Start
        {
            get { return HasStartPage() ? new AdjustmentList(StartUrl) : RecurlyList.Empty<Adjustment>(); }
        }

        public override RecurlyList<Adjustment> Next
        {
            get { return HasNextPage() ? new AdjustmentList(NextUrl) : RecurlyList.Empty<Adjustment>(); }
        }

        public override RecurlyList<Adjustment> Prev
        {
            get { return HasPrevPage() ? new AdjustmentList(PrevUrl) : RecurlyList.Empty<Adjustment>(); }
        }

        public AdjustmentList()
        {
        }

        public AdjustmentList(string url) : base(Client.HttpRequestMethod.Get, url)
        {
        }

        internal override void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if ((reader.Name == "adjustments" || reader.Name == "line_items") && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "adjustment")
                {
                    Add(new Adjustment(reader));
                }
            }
        }
    }
}