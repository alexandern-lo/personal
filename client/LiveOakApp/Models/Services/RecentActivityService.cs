using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiveOakApp.Models.Data.NetworkDTO;
using SL4N;

namespace LiveOakApp.Models.Services
{
    public class RecentActivityService
    {
        public List<LeadRecentActivityDTO> RecentActivityItems { get { return RecentActivityRequest.Result; } }
        public CachableRequest<List<LeadRecentActivityDTO>> RecentActivityRequest { get; private set; }

        public RecentActivityService(CacheStorage CacheStorage)
        {
            RecentActivityRequest = new CachableRequest<List<LeadRecentActivityDTO>>(
                CacheStorage,
                "recentActivity",
                (eTag, token) => ServiceLocator.Instance.ApiService.GetLeadsRecentActivity(10, eTag, token)
            );
        }
    }
}
