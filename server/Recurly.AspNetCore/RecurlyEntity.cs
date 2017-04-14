using System.Xml;

namespace Recurly.AspNetCore
{
    public abstract class RecurlyEntity
    {
        internal QueryStringBuilder Build { get; set; }

        protected RecurlyEntity()
        {
            Build = new QueryStringBuilder();
        }

        internal abstract void ReadXml(XmlReader reader);
        internal abstract void WriteXml(XmlWriter writer);
    }
}
