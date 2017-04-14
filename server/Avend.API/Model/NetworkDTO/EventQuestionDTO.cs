using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Avend.API.Model.NetworkDTO
{
    [DataContract]
    public class EventQuestionDto
    {
        public EventQuestionDto()
        {
            Choices = new List<AnswerChoiceDto>();
        }

        [DataMember(Name = "uid")]
        public Guid Uid { get; set; }

        [DataMember(Name = "event_uid")]
        public Guid EventUid { get; set; }

        [DataMember(Name = "position")]
        public int? Position { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "answers")]
        public List<AnswerChoiceDto> Choices { get; set; }

        /// <summary>
        /// Date and time of the record's creation.
        /// </summary>
        /// <value>Date and time of the record's creation.</value>
        [DataMember(Name = "created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Date and time of the record's last update.
        /// </summary>
        /// <value>Date and time of the record's last update.</value>
        [DataMember(Name = "updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public static EventQuestionDto From(EventQuestionRecord obj, Guid eventUid)
        {
            var dto = new EventQuestionDto()
            {
                Uid = obj.Uid,

                EventUid = eventUid,

                Position = obj.Position,
                Text = obj.Text,

                Choices = (obj.Choices ?? new List<AnswerChoiceRecord>()).Select(AnswerChoiceDto.From).ToList(),

                CreatedAt = obj.CreatedAt,
                UpdatedAt = obj.UpdatedAt,
            };

            return dto;
        }
    }
}
