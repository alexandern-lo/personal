using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudioMobile;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Models.Services;

namespace LiveOakApp.Models.ViewModels
{
    public class ResourcesViewModel : DataContext
    {
        public ObservableList<ResourceViewModel> Resources { get; } = new ObservableList<ResourceViewModel>();

        public ResourcesViewModel()
        {
            var service = ServiceLocator.Instance.ResourcesService;
            var resourceDTOs = service.Resources;
            if (resourceDTOs != null)
                Resources.Reset(resourceDTOs.ConvertAll((ResourceDTO input) => new ResourceViewModel(input)));

            LoadResourcesCommand = new CachableCommandViewModel<List<ResourceDTO>>(service.ResourcesRequest);

            TrackResourceSentCommand = new AsyncCommand
            {
                Action = TrackResourceSent
            };

            Bindings.Property(LoadResourcesCommand, _ => _.Result).UpdateTarget((a) =>
            {
                LOG.Debug("received resources: {0}", a.Value.Count);
                Resources.Reset(a.Value.ConvertAll((ResourceDTO input) => new ResourceViewModel(input)));
            });
            Bindings.Bind();
        }

        public string GetResourceSharingUrl(ResourceViewModel resource) 
        {
            var uid = resource.Uid.ToString();
            return string.Format("{0}api/v1/users/resources/{1}/open", ApplicationConfig.ApiRootUrl, uid);
        }

        public CachableCommandViewModel<List<ResourceDTO>> LoadResourcesCommand { get; private set; }

        public bool CanSend { get; set; }

        public AsyncCommand TrackResourceSentCommand { get; private set; }

        async Task TrackResourceSent(object resourcesList) 
        {
            var resources = resourcesList as List<ResourceViewModel>;
            var resourcesUid = resources.ConvertAll((input) => input.Uid);
            await ServiceLocator.Instance.ApiService.SendResourceSentTrackingRequest(resourcesUid, TrackResourceSentCommand.Token);
        }
    }
}

