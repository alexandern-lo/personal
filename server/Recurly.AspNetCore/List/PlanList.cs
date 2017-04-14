using System.Xml;

namespace Recurly.AspNetCore.List
{
    public class PlanList : RecurlyList<Plan>
    {
        internal PlanList(string baseUrl) : base(Client.HttpRequestMethod.Get, baseUrl)
        {
        }

        public override RecurlyList<Plan> Start => HasStartPage() ? new PlanList(StartUrl) : RecurlyList.Empty<Plan>();

        public override RecurlyList<Plan> Next => HasNextPage() ? new PlanList(NextUrl) : RecurlyList.Empty<Plan>();

        public override RecurlyList<Plan> Prev => HasPrevPage() ? new PlanList(PrevUrl) : RecurlyList.Empty<Plan>();

        internal override void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.Name == "plans" && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "plan")
                {
                    Add(new Plan(reader));
                }
            }
        }
    }
}