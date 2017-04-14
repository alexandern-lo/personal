using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Avend.API.Services.Events.NetworkDTO
{
    [DataContract]
    public class EventAgendaItemMassDeleteRequestDto
    {
        [DataMember(Name = "agenda_item_uids")]
        public List<Guid?> Uids { get; set; }
    }
}
