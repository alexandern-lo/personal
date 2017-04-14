using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract]
    public class EventQuestionDTO
    {
        public EventQuestionDTO()
        {
            Answers = new List<EventQuestionAnswerDTO>();
        }

        [DataMember(Name = "uid")]
        public string UID { get; set; }

        [DataMember(Name = "position")]
        public int Position { get; set; }

        [DataMember(Name = "text")]
        public string Content { get; set; }

        [DataMember(Name = "answers")]
        public List<EventQuestionAnswerDTO> Answers { get; set; }
    }
}
