using System;
using System.Runtime.Serialization;
using Avend.API.Model.NetworkDTO;

namespace Avend.API.Services.Dashboard.NetworkDTO
{
    [DataContract]
    public class UserTotalExpensesDto
    {
        [DataMember(Name = "user_uid")]
        public Guid? UserUid { get; set; }

        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }

        [DataMember(Name = "last_name")]
        public string LastName { get; set; }

        [DataMember(Name = "amount")]
        public MoneyDto Amount { get; set; }
    }
}
