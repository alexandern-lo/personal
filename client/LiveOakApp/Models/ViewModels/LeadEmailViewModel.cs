using System;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Resources;

namespace LiveOakApp.Models.ViewModels
{
    public class LeadEmailViewModel
    {
        readonly EmailDTO EmailDTO;

        public string Email { get { return EmailDTO.Email; } }

        public string EmailDescription
        {
            get
            {
                return string.Format("{0}: {1}", GetEmailTypeString(EmailDTO.TypeEnum), Email);
            }
        }

        public LeadEmailViewModel(EmailDTO emailDTO)
        {
            EmailDTO = emailDTO;
        }

        static string GetEmailTypeString(EmailDTO.EmailType emailType)
        {
            switch (emailType)
            {
                case EmailDTO.EmailType.Home: return L10n.Localize("HomeEmailType", "Home");
                case EmailDTO.EmailType.Other: return L10n.Localize("OtherEmailType", "Other");
                case EmailDTO.EmailType.Work: return L10n.Localize("WorkEmailType", "Work");
                default: return "";
            }
        }
    }
}
