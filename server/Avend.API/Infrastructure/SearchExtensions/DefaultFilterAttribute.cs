using System;

namespace Avend.API.Infrastructure.SearchExtensions
{
    public class DefaultFilterAttribute : Attribute
    {
        public DefaultFilterAttribute(params string[] properties)
        {
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            Properties = properties;
        }

        public string [] Properties { get; }
    }
}