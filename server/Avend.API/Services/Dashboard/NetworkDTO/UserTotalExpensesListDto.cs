using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Avend.API.Model.NetworkDTO;

namespace Avend.API.Services.Dashboard.NetworkDTO
{
    [DataContract]
    public class UserTotalExpensesListDto
    {
        [DataMember(Name = "event_uids")]
        public IEnumerable<Guid> EventUids { get; set; }

        [DataMember(Name = "total_expenses")]
        public MoneyDto TotalExpenses { get; set; }

        [DataMember(Name = "user_expenses")]
        public IEnumerable<UserTotalExpensesDto> UserExpenses { get; set; }
    }
}