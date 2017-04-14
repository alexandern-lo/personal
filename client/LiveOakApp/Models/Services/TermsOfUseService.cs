using System;
using System.Threading;
using System.Threading.Tasks;
using SL4N;
using LiveOakApp.Models.Data.NetworkDTO;

namespace LiveOakApp.Models.Services
{
    public class TermsOfUseService
    {
        static readonly ILogger LOG = LoggerFactory.GetLogger<TermsOfUseService>();

        readonly ApiService ApiService;

        public bool IsAccepted { get { return Terms?.AcceptedDate != null; } }

        public TermsOfUseDTO Terms { get { return TermsRequest.Result; } }

        public CachableRequest<TermsOfUseDTO> TermsRequest { get; private set; }

        public async Task LoadTermsFromCacheIfNeeded()
        {
            if (TermsRequest.DataIsLoadedToCache) return;
            await TermsRequest.LoadFromCache();
        }

        public TermsOfUseService(ApiService apiService, CacheStorage CacheStorage)
        {
            ApiService = apiService;
            TermsRequest = new CachableRequest<TermsOfUseDTO>(
                CacheStorage,
                "terms",
                (eTag, token) => ApiService.GetTerms(eTag, token)
            );
        }

        public async Task Accept(TermsOfUseDTO terms, CancellationToken? cancellationToken)
        {
            var result = await ApiService.AcceptTerms(terms.UID, cancellationToken);
            LOG.Debug("accept terms: " + result.Content);
            terms.AcceptedDate = new DateTime();
            await TermsRequest.ReplaceCache(terms);
        }

        public async Task Decline()
        {
            await ServiceLocator.Instance.AuthService.Logout();
        }
    }
}
