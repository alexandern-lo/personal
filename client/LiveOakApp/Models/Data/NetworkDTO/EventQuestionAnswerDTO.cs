using System;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract]
    public class EventQuestionAnswerDTO
    {
        [DataMember(Name = "uid")]
        public string UID { get; set; }

        [DataMember(Name = "position")]
        public int Position { get; set; }

        [DataMember(Name = "text")]
        public string Content { get; set; }
    }
}
