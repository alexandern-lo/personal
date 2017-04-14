using System;
using System.Xml;
using Recurly.AspNetCore.Extensions;

namespace Recurly.AspNetCore
{
    public class Note : RecurlyEntity
    {
        public string AccountCode { get; protected set; }
        public string Message { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        internal Note(XmlReader xmlReader)
        {
            ReadXml(xmlReader);
        }

        internal override void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.Name == "note" && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if (reader.NodeType != XmlNodeType.Element) continue;

                string href;
                switch (reader.Name)
                {
                    case "account":
                        href = reader.GetAttribute("href");
                        if (null != href)
                            AccountCode = Uri.UnescapeDataString(href.Substring(href.LastIndexOf("/") + 1));
                        break;

                    case "message":
                        Message = reader.ReadElementContentAsString();
                        break;

                    case "created_at":
                        CreatedAt = reader.ReadElementContentAsDateTime();
                        break;
                }
            }
        }

        internal override void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}