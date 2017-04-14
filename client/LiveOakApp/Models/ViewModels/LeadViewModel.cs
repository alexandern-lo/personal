using System;
using System.Linq;
using System.Collections.Generic;
using ServiceStack;
using StudioMobile;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Models.Data.Entities;
using LiveOakApp.Resources;

namespace LiveOakApp.Models.ViewModels
{
    public class LeadViewModel : DataContext
    {
        public Lead Lead { get; } // only access this from ViewModels

        public LeadViewModel(Lead lead)
        {
            Lead = lead;
        }

        LeadDTO LeadDTO { get { return Lead.LeadDTO; } }

        public string EventUID { get { return LeadDTO.Event?.UID; } }
        public FileResource PhotoResource { get { return Lead.PhotoResource; } }
        public DateTime? LastEditDate { get { return Lead.LeadRecord?.OverwriteUpdatedAt ?? LeadDTO.ClientsideUpdatedAt; } }

        public string FullName
        {
            get
            {
                var parts = new List<string> { LeadDTO.FirstName, LeadDTO.LastName };
                var result = string.Join(" ", parts.Where(_ => !_.IsNullOrEmpty()));
                if (!result.IsNullOrEmpty()) return result;
                var email = LeadDTO.Emails.FirstOrDefault((arg) => !string.IsNullOrWhiteSpace(arg.Email));
                if (email == null)
                    return L10n.Localize("LeadDefaultName", "Lead");
                return email.Email;
            }
        }

        public string JobInfo
        {
            get
            {
                var parts = new List<string> { LeadDTO.CompanyName, LeadDTO.JobTitle };
                return string.Join(", ", parts.Where(_ => !_.IsNullOrEmpty()));
            }
        }

        public List<LeadEmailViewModel> Emails { get { return LeadDTO.Emails.ConvertAll(_ => new LeadEmailViewModel(_)); } }
    }
}
