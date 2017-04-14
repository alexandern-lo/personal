using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace Avend.API.Model.XmlNullable
{
    /// <summary>
    /// Converter for propert writing the XmlNullable to JSON.
    /// It writes the real value instead of wrapping object.
    /// </summary>
    public class XmlNullableConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var nullableValue = value as IXmlNullable;

            if (nullableValue == null)
            {
                writer.WriteValue(null as object);

                return;
            }

            if (!nullableValue.HasValue)
            {
                writer.WriteValue(null as object);

                return;
            }

            var type = value.GetType();

            var propertyInfos = type.GetProperties();

            var propertyInfo = propertyInfos.FirstOrDefault(
                fldRecord =>
                    fldRecord.CustomAttributes.Any(
                        record => record.AttributeType == typeof(DataMemberAttribute)
                        )
                );

            if (propertyInfo == null)
            {
                writer.WriteValue(null as object);

                return;
            }

            var propValue = propertyInfo.GetValue(value);

            writer.WriteValue(propValue);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }

            if (typeof(Type) == typeof(DateTime))
                return (XmlNullable<DateTime>)(DateTime) Convert.ChangeType(reader.Value, typeof(DateTime));

            if (typeof(Type) == typeof(int))
                return (XmlNullable<int>)(int) Convert.ChangeType(reader.Value, typeof(int));

            return reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }
    }
}