using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiveOakApp.Models.Data.NetworkDTO;

namespace LiveOakApp.Models.Services
{
    public class ResourcesService
    {
        public List<ResourceDTO> Resources { get { return ResourcesRequest.Result; } }

        public CachableRequest<List<ResourceDTO>> ResourcesRequest { get; private set; }

        public ResourcesService(CacheStorage CacheStorage)
        {
            ResourcesRequest = new CachableRequest<List<ResourceDTO>>(
                CacheStorage,
                "resources",
                (eTag, token) => ServiceLocator.Instance.ApiService.GetResources(eTag, token)
            );
        }
    }
}
