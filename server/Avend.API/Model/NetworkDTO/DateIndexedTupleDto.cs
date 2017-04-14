using System;
using System.Runtime.Serialization;

namespace Avend.API.Model.NetworkDTO
{
    /// <summary>
    /// Special DTO for processing value lists indexed by date (and most times used to build charts).
    /// </summary>
    /// 
    /// <typeparam name="T">Type of value to be used. Usually decimal or a collection of those.</typeparam>
    [DataContract(Name = "date_and_value")]
    public class DateIndexedTupleDto<T>
    {
        [DataMember(Name = "date")]
        public DateTime Date { get; set; }

        [DataMember(Name = "value")]
        public T Value { get; set; }

        public override string ToString()
        {
            return $"{nameof(Date)}: {Date}, {nameof(Value)}: {Value}";
        }
    }
}