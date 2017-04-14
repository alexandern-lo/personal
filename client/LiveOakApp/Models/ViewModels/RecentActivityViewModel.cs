using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Models.Services;
using LiveOakApp.Resources;
using StudioMobile;

namespace LiveOakApp.Models.ViewModels
{
    public class RecentActivityViewModel : DataContext
    {
        readonly RecentActivityService Service = ServiceLocator.Instance.RecentActivityService;
        public CachableCommandViewModel<List<LeadRecentActivityDTO>> LoadRecentActivityCommand { get; private set; }
        public ObservableList<LeadRecentActivityViewModel> RecentActivityItems { get; } = new ObservableList<LeadRecentActivityViewModel>();

        public RecentActivityViewModel()
        {
            var resourceDTOs = Service.RecentActivityItems;
            if (resourceDTOs != null)
                RecentActivityItems.Reset(resourceDTOs.ConvertAll((LeadRecentActivityDTO input) => new LeadRecentActivityViewModel(input)));

            LoadRecentActivityCommand = new CachableCommandViewModel<List<LeadRecentActivityDTO>>(Service.RecentActivityRequest);

            Bindings.Property(LoadRecentActivityCommand, _ => _.Result).UpdateTarget((a) =>
            {
                LOG.Debug("Received LeadRecentActivity: {0}", a.Value.Count);
                RecentActivityItems.Reset(a.Value.ConvertAll((LeadRecentActivityDTO input) => new LeadRecentActivityViewModel(input)));
            });
            Bindings.Bind();
        }

        #region QRScanner
        public async Task<byte[]> TryDownloadVCard(string uri, CancellationToken? cancellationToken)
        {
            try
            {
                return await ServiceLocator.Instance.ApiService.DownloadSmallFileToMemory(uri, cancellationToken);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Cannot write more bytes to the buffer than the configured maximum buffer"))
                    throw new Exception(L10n.Localize("TooLargeVCardFileException", "File is too large for a vCard"));
                else if (ex.Message.Contains("specified hostname could not be found"))
                    throw new Exception(L10n.Localize("InvalidQRCodeErrorMessage", "This QR code doesn't contain vCard"));
                else
                    throw;
            }
        }
        #endregion
    }
}
