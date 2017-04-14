using System;

namespace Avend.API.Infrastructure
{
    /// <summary>
    /// Simple utility to convert string into nullable enum values which act as filter values.
    /// </summary>
    public static class EnumFilter
    {
        public static TEnum? Parse<TEnum>(string strValue) where TEnum : struct
        {
            TEnum value;
            if (Enum.TryParse(strValue, true, out value)) return value;
            return null;
        }
    }
}