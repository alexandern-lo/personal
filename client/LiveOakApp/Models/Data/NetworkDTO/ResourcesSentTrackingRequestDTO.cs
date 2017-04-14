using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
	[DataContract(Name = "resources_sent_tracking_request")]
	public class ResourcesSentTrackingRequestDTO
	{
		[DataMember(Name = "resource_uids")]
		public List<Guid> Uids { get; set; }
	}
}