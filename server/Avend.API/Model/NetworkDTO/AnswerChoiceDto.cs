using System;
using System.Runtime.Serialization;

namespace Avend.API.Model.NetworkDTO
{
    [DataContract]
    public class AnswerChoiceDto
    {
        [DataMember(Name = "uid")]
        public Guid Uid { get; set; }

        [DataMember(Name = "position")]
        public int? Position { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

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

        public static AnswerChoiceDto From(AnswerChoiceRecord obj)
        {
            var dto = new AnswerChoiceDto()
            {
                Uid = obj.Uid,

                Position = obj.Position,
                Text = obj.Text,

                CreatedAt = obj.CreatedAt,
                UpdatedAt = obj.UpdatedAt,
            };

            return dto;
        }

        public void UpdateEventQuestionAnswer(AnswerChoiceRecord answerAnswer)
        {
            if (Text != null)
                answerAnswer.Text = Text;

            if (Position != null)
                answerAnswer.Position = Position.Value;
        }
    }
}
