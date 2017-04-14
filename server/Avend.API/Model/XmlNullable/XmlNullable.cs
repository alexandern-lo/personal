using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Newtonsoft.Json;

namespace Avend.API.Model.XmlNullable
{
    /// <summary>
    /// Generics-based class for implementing nullable XML types.
    /// Mimics behaviour for classic Nullables, but focused on XML serialization.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract(IsReference = false)]
    [JsonConverter(typeof(XmlNullableConverter))]
    public struct XmlNullable<T> : IXmlNullable, IXmlSerializable
    {
        public bool HasValue { get; private set; }
        public Type ValueType => typeof(T);

        [DataMember]
        public T Value { get; private set; }

        private XmlNullable(T value)
        {
            HasValue = true;
            Value = value;
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader.GetAttribute("nil") == "true")
            {
                ReadNullValue();
                return;
            }

            ReadNonNullValue(reader);
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            throw new NotSupportedException();
        }

        private void ReadNullValue()
        {
            HasValue = false;
        }

        private void ReadNonNullValue(XmlReader reader)
        {
            reader.ReadStartElement();

            var str = reader.ReadContentAsString();

            try
            {
                HasValue = true;

                if (typeof(T) == typeof(DateTime))
                {
                    DateTime dateTime;

                    var result = DateTime.TryParse(str, out dateTime);

                    if (result)
                        Value = (T)Convert.ChangeType(dateTime, typeof(T));
                    else
                    {
                        HasValue = false;
                        Value = default(T);
                    }
                }
                else
                {
                    Value = (T) Convert.ChangeType(str, typeof(T));
                }
            }
            catch (Exception)
            {
                HasValue = false;
                Value = default(T);
            }

            reader.ReadEndElement();
        }

        public static implicit operator XmlNullable<T>(T value)
        {
            return new XmlNullable<T>(value);
        }

        /// <summary>
        /// Overrides ToString to show the real value the object is holding.
        /// </summary>
        /// <returns>
        /// Value being stored or null if object doesn't has a value.
        /// </returns>
        public override string ToString()
        {
            return HasValue ? Value.ToString() : "null";
        }
    }
}