using System;
using System.Runtime.Serialization;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;

namespace Avend.API.Services.Events.NetworkDTO
{
    [DataContract]
    public class EventUserGoalsDto : IEventBasedDto
    {
        [DataMember(Name = "event_user_goal_uid")]
        public Guid? Uid { get; set; }

        [DataMember(Name = "user_uid")]
        public Guid? UserUid { get; set; }

        [DataMember(Name = "event_uid", IsRequired = true)]
        public Guid? EventUid { get; set; }

        [DataMember(Name = "leads_goal", IsRequired = true)]
        public int? LeadsGoal { get; set; }

        public void ApplyChangesToModel(EventUserGoalsRecord eventUserGoalsRecord)
        {
            if (LeadsGoal.HasValue)
                eventUserGoalsRecord.LeadsGoal = LeadsGoal.Value;
        }

        public static EventUserGoalsDto From(EventUserGoalsRecord eventUserGoalsRecord, Guid? eventUid, Guid? userUid = null)
        {
            var dto = new EventUserGoalsDto()
            {
                Uid = eventUserGoalsRecord.Uid,

                EventUid = eventUid,
                UserUid = userUid,

                LeadsGoal = eventUserGoalsRecord.LeadsGoal,
            };

          return dto;
        }
    }
}
