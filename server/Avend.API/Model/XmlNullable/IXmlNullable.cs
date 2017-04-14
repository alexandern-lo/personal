using System;

namespace Avend.API.Model.XmlNullable
{
    /// <summary>
    /// Interface for supporting nullable XML types.
    /// Exposes HasValue and ValueType properties.
    /// </summary>
    public interface IXmlNullable
    {
        bool HasValue { get; }
        Type ValueType { get; }
    }
}