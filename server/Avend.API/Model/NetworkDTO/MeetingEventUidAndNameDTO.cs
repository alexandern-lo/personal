using System;
using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace Avend.API.Model.NetworkDTO
{
    /// <summary>
    /// Data Transfer object for events.
    /// </summary>
    [DataContract(Name = "event_uid_and_name")]
    public class MeetingEventUidAndNameDTO
    {
       
        /// <summary>
        /// Unique identifier representing the specific event.
        /// </summary>
        /// <value>Unique identifier representing the specific event.</value>
        [DataMember(Name = "event_uid")]
        public Guid? Uid { get; set; }

        /// <summary>
        /// Event type. Can only be conference or personal at the moment.
        /// </summary>
        /// <value>Event type. Can only be conference or personal at the moment.</value>
        [DataMember(Name = "event_type")]
        public string Type { get; set; }

        /// <summary>
        /// Event name
        /// </summary>
        /// <value>Event name</value>
        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; set; }

        /// <summary>
        /// Event start date
        /// </summary>
        /// <value>Event start date</value>
        [DataMember(Name = "start_date", IsRequired = true)]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Event state
        /// </summary>
        /// <value>Event state</value>
        [DataMember(Name = "state", IsRequired = true)]
        public string State { get; set; }

        /// <summary>
        /// Event city
        /// </summary>
        /// <value>Event city</value>
        [DataMember(Name = "city", IsRequired = true)]
        public string City { get; set; }

        /// <summary>
        /// Questions count
        /// </summary>
        /// <value>Questions count</value>
        [DataMember(Name = "questions_count", IsRequired = true)]
        public int QuestionsCount { get; set; }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static MeetingEventUidAndNameDTO From(EventRecord eventObj, int questionsCount)
        {
            var dto = new MeetingEventUidAndNameDTO()
            {
                Uid = eventObj.Uid,
                Type = eventObj.Type,

                Name = eventObj.Name,
                StartDate = eventObj.StartDate,

                State = eventObj.State,
                City = eventObj.City,

                QuestionsCount = questionsCount,
            };

            return dto;
        }
    }
}
