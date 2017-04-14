using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack;
using LiveOakApp.Models.Data.Entities;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Resources;

namespace LiveOakApp.Models.ViewModels
{
    public class LeadOverwriteViewModel
    {
        Lead Lead { get; }

        public LeadOverwriteViewModel(Lead lead)
        {
            Lead = lead;
        }

        LeadDTO LeadDTO { get { return Lead.LeadDTO; } }

        string FullName
        {
            get
            {
                var parts = new List<string> { LeadDTO.FirstName, LeadDTO.LastName };
                return string.Join(" ", parts.Where(_ => !_.IsNullOrEmpty()));
            }
        }

        #region Info Messages

        public string DialogTitle
        {
            get
            {
                return L10n.Localize("LeadOverwriteTitle", "Server has more recent data for a lead");
            }
        }

        public string DialogMessage
        {
            get
            {
                var dateService = ServiceLocator.Instance.DateTimeService;
                var leadInfoFormat = L10n.Localize("LeadOverwriteInfoFormat", "Lead: {0}\nChanged on server: {1}\nChanged locally: {2}\n");
                var serverDate = dateService.DateTimeToDisplayString(Lead.LeadRecord.ServerUpdatedAt);
                var clientDate = dateService.DateTimeToDisplayString(Lead.LeadRecord.OverwriteUpdatedAt);
                var leadInfo = string.Format(leadInfoFormat, FullName, serverDate, clientDate);
                var message = L10n.Localize("LeadOverwriteMessageStart", "Do you want to overwrite server data with local data?");
                return leadInfo + message;
            }
        }

        public string OverwriteActionName
        {
            get
            {
                return L10n.Localize("LeadOverwriteAction", "Overwrite with local data");
            }
        }

        public string DiscardActionName
        {
            get
            {
                return L10n.Localize("LeadDiscardAction", "Discard local data");
            }
        }

        public string CancelActionName
        {
            get
            {
                return L10n.Localize("LeadDecideLaterAction", "Decide Later");
            }
        }

        #endregion

        #region Actions

        public async Task OverwiteLeadChanges()
        {
            await ServiceLocator.Instance.LeadsService.MarkLeadForOverwrite(Lead.LeadRecord.Id, null);
        }

        public async Task DiscardLeadChanges()
        {
            await ServiceLocator.Instance.LeadsService.DiscardChangesForLead(Lead.LeadRecord.Id, null);
        }

        #endregion
    }
}
