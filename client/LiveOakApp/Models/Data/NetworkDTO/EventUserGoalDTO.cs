using System;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract(Name = "event_user_goal")]
    public class EventUserGoalDTO
    {
        [DataMember(Name = "event_user_goal_uid")]
        public Guid? Uid { get; set; }

        [DataMember(Name = "event_uid", IsRequired = true)]
        public Guid? EventUid { get; set; }

        [DataMember(Name = "leads_goal", IsRequired = true)]
        public int? LeadsGoal { get; set; }
    }
}
