using System.Reflection;
using Avend.API.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Avend.API.Middleware
{
    public class ErrorSkippingContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType == typeof(AttendeeCategoryRecord) &&
                property.PropertyType == typeof(EventRecord))
            {
                property.ShouldSerialize = instanceOfProblematic => false;
            }

            return property;
        }
    }
}