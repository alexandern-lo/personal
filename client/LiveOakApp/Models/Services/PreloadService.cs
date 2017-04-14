using System;
using System.Threading.Tasks;
using SL4N;
using StudioMobile;

namespace LiveOakApp.Models.Services
{
    public class PreloadService
    {
        static readonly ILogger LOG = LoggerFactory.GetLogger<PreloadService>();

        readonly ProfileService ProfileService;
        readonly AuthService AuthService;
        readonly EventsService EventsService;

        bool PreloadStarted { get; set; } = false;

        public PreloadService(EventsService eventsService, ProfileService profileService, AuthService authService)
        {
            EventsService = eventsService;
            ProfileService = profileService;
            AuthService = authService;
        }

        public void StartPreloadingIfNeeded()
        {
            if (PreloadStarted) return;
            if (!AuthService.IsLoggedIn) return;
            if (!ProfileService.ProfileRequest.DataIsLoadedToCache) return;
            if (!ProfileService.IsSubscriptionValid) return;

            PreloadStarted = true;
            StartPreloading().Ignore();
        }

        public void ResetPreloadingState()
        {
            PreloadStarted = false;
        }

        async Task StartPreloading()
        {
            try
            {
                await EventsService.EventsRequest.LoadFromNetwork(null);
            }
            catch (Exception error)
            {
                LOG.Error("Preload failed", error);
            }
        }
    }
}
