using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using SL4N;
using StudioMobile;
using LiveOakApp.Models.Data.NetworkDTO;

namespace LiveOakApp.Models.Services
{
    public class ProfileService
    {
        static readonly ILogger LOG = LoggerFactory.GetLogger<ProfileService>();

        readonly ApiService ApiService;
        readonly TermsOfUseService TermsOfUseService;

        public bool IsSubscriptionValid
        {
            get
            {
                if (Profile == null) return true; // assume valid until we have data
                var isValid = !Profile.CurrentSubscription?.Expired ?? false;
                if (isValid)
                {
                    ReceivedInvalidSubscriptionError = false;
                }
                if (ReceivedInvalidSubscriptionError) return false;
                return isValid;
            }
        }

        public bool IsSuperAdmin 
        {
            get
            {
                if (Profile == null) return false;
                return Profile.User.Role == UserDTO.UserRole.SuperAdmin;
            }
        }

        bool ReceivedInvalidSubscriptionError { get; set; } = false;

        public UserProfileDTO Profile { get { return ProfileRequest.Result; } }

        // use GetProfile, don't execute this directly
        public CachableRequest<UserProfileDTO> ProfileRequest { get; private set; }

        DateTimeOffset profileUpdatedAt = DateTimeOffset.MinValue;

        public async Task LoadProfileFromCacheIfNeeded()
        {
            if (ProfileRequest.DataIsLoadedToCache) return;
            await ProfileRequest.LoadFromCache();
        }

        public ProfileService(ApiService apiService, CacheStorage CacheStorage, TermsOfUseService termsOfUseService)
        {
            ApiService = apiService;
            TermsOfUseService = termsOfUseService;
            ApiService.ApiExceptionReceived += (sender, args) => CheckForSubscriptionExpiredError(args.exception);
            ProfileRequest = new CachableRequest<UserProfileDTO>(
                CacheStorage,
                "profile",
                (eTag, token) => ApiService.GetProfile(eTag, token)
            );
        }

        public async Task GetProfile(CancellationToken? cancellationToken)
        {
            await ProfileRequest.LoadFromNetwork(cancellationToken);
            profileUpdatedAt = DateTimeOffset.Now;
            if (Profile != null)
            {
                await TermsOfUseService.TermsRequest.ReplaceCache(Profile.AcceptedTerms);
            }
        }

        public async Task GetProfileNoThrow(CancellationToken? cancellationToken)
        {
            try
            {
                await GetProfile(cancellationToken);
            }
            catch (Exception error)
            {
                LOG.Warn("Profile loading failed: {0}", error.Message);
            }
        }

        public void UpdatedProfileIfNeeded()
        {
            if (profileUpdatedAt > DateTimeOffset.Now.AddMinutes(-5))
            {
                return;
            }
            GetProfileNoThrow(null).Ignore();
        }

        void CheckForSubscriptionExpiredError(Exception exception)
        {
            if (exception.AllExceptions().Any(e => e is SubscriptionExpiredError))
            {
                HandleSubscriptionExpiredError().Ignore();
            }
        }

        async Task HandleSubscriptionExpiredError()
        {
            ReceivedInvalidSubscriptionError = true;
            await ProfileRequest.DropData();
            NavigationManager.Instance.NavigateToRequiredStateIfNeeded();
        }
    }
}
