using System;
using System.Xml;

namespace Recurly.AspNetCore.Extensions
{
    public static class XmlReaderExtensions
    {
        private static readonly XmlNamespaceManager defaultManager = new XmlNamespaceManager(new NameTable());

        /// <summary>
        /// Convenience implementation of <see cref="T:System.Xml.XmlReader.ReadElementContentAsDateTime()"/>.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> to read from.</param>
        /// <param name="manager">Optional <see cref="T:System.Xml.XmlNamespaceManager"/> to use for namespace resolutions.</param>
        public static DateTime ReadElementContentAsDateTime(this XmlReader reader, XmlNamespaceManager manager = null)
        {
          return (DateTime) reader.ReadElementContentAs(typeof(DateTime), manager ?? defaultManager);
        }
    }
}