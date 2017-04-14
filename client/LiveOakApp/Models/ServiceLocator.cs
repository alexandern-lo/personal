using SL4N;
using SL4N.NLog;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Threading.Tasks;
using System.IO;
using FFImageLoading;
using LiveOakApp.Models.Services;
using FFImageLoading.Helpers;
using StudioMobile;

namespace LiveOakApp.Models
{
    public class ServiceLocator
    {
        static readonly ServiceLocator _instance = new ServiceLocator();

		ServiceLocator()
		{
			InitLogging();

            DateTimeService = new DateTimeService();
            JsonService = new JsonService(DateTimeService);
            CacheStorage = new CacheStorage(JsonService);

            var userDatabase = new UserSQLiteDatabase();
            var leadsStorage = new LeadStorage(userDatabase);

            AuthService = new AuthService(CacheStorage, userDatabase);
            GraphApiService = new GraphApiService();

            ApiService = new ApiService(JsonService, AuthService);
            FileResourcesService = new FileResourcesService();
            MessagingCenter = MessagingCenter.Default;


            LeadsService = new LeadsService(CacheStorage, leadsStorage, JsonService);
            LeadsUploadService = new LeadsUploadService(leadsStorage, JsonService, ApiService, FileResourcesService, MessagingCenter);
            EventsService = new EventsService(CacheStorage);
            EventService = new EventService(ApiService);
            FontService = new FontService();
            RecentActivityService = new RecentActivityService(CacheStorage);
            AttendeesFiltersService = new AttendeesFiltersService(EventsService);
            AttendeesService = new AttendeesService(ApiService, AttendeesFiltersService);
            AgendaService = new AgendaService(ApiService);
            ResourcesService = new ResourcesService(CacheStorage);
            TermsOfUseService = new TermsOfUseService(ApiService, CacheStorage);
            ProfileService = new ProfileService(ApiService, CacheStorage, TermsOfUseService);
            PreloadService = new PreloadService(EventsService, ProfileService, AuthService);
            DashboardService = new DashboardService(ApiService);
            CrmService = new CrmService(ProfileService);
		}

		static void InitLogging()
		{
			// Step 1. Create configuration object 
			var config = new LoggingConfiguration();

			// Step 2. Create targets and add them to the configuration 
			var consoleTarget = new ConsoleTarget();
			config.AddTarget("console", consoleTarget);

			var fileTarget = new FileTarget();
			config.AddTarget("file", fileTarget);

			// Step 3. Set target properties 
			fileTarget.Layout = consoleTarget.Layout = @"${logger} ${message} ${exception:format=toString}";
            var personalDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            // TODO: use public folder on Android for logs
            fileTarget.FileName = Path.Combine(personalDir, "avendapp.log");

            #if DEBUG
            var logLevel = LogLevel.Trace;
            #else
            var logLevel = LogLevel.Debug;
            #endif

            // Step 4. Define rules
            var consoleRule = new LoggingRule("*", logLevel, consoleTarget);
            config.LoggingRules.Add(consoleRule);

            var fileRule = new LoggingRule("*", logLevel, fileTarget);
			config.LoggingRules.Add(fileRule);

			// Step 5. Activate the configuration
			LogManager.Configuration = config;

			LoggerFactory.Adapter = new NLogLoggerFactory();

            ImageService.Instance.Config.Logger = new ImageServiceLogger();
		}

        public void FlushCaches()
        {
        }

        bool isAppInForeground;
        public bool IsAppInForeground
        {
            get
            {
                return isAppInForeground;
            }
            set
            {
                isAppInForeground = value;
                RefreshLeadUploadService();
            }
        }

        void RefreshLeadUploadService()
        {
            LeadsUploadService.UploadingEnabled = AuthService.IsLoggedIn;
        }

        void CancelNetworkRequests()
        {
            ProfileService.ProfileRequest.CancelLoading();
            TermsOfUseService.TermsRequest.CancelLoading();
            EventsService.EventsRequest.CancelLoading();
            LeadsService.LeadsRequest.CancelLoading();
            RecentActivityService.RecentActivityRequest.CancelLoading();
            // TODO: add all CachableRequests
        }

        public void DropInMemoryCaches(bool dropUserData = false)
        {
            ImageService.Instance.InvalidateMemoryCache();
            if (dropUserData)
            {
                // profile and terms have low memory impact and are needed for streamlined UI navigation
                ProfileService.ProfileRequest.DropInMemoryCache();
                TermsOfUseService.TermsRequest.DropInMemoryCache();
            }
            EventsService.EventsRequest.DropInMemoryCache();
            LeadsService.LeadsRequest.DropInMemoryCache();
            RecentActivityService.RecentActivityRequest.DropInMemoryCache();
            // TODO: add all CachableRequests
        }

        public async Task DropAllCaches()
        {
            await CacheStorage.DropAllObjects();
            DropInMemoryCaches(true);
            await ImageService.Instance.InvalidateDiskCacheAsync();
        }

        public async Task OnLogoutCleanup()
        {
            RefreshLeadUploadService();
            CancelNetworkRequests();
            await DropAllCaches();
            PreloadService.ResetPreloadingState();
        }

        public void OnLoggedInSetup()
        {
            RefreshLeadUploadService();
        }

        public void Terminate()
        {
            IsAppInForeground = false;
        }

        public static ServiceLocator Instance
        {
            get { return _instance; }
        }

        public CacheStorage CacheStorage { get; internal set; }

        public AttendeesService AttendeesService { get; internal set; }
        public LeadsService LeadsService { get; internal set; }
        public LeadsUploadService LeadsUploadService { get; internal set; }
        public EventsService EventsService { get; internal set; }
        public EventService EventService { get; internal set; }
        public RecentActivityService RecentActivityService { get; internal set; }
        public AgendaService AgendaService { get; internal set; }
        public AuthService AuthService { get; internal set; }
        public GraphApiService GraphApiService { get; internal set; }
        public AttendeesFiltersService AttendeesFiltersService { get; internal set; }
        public DateTimeService DateTimeService { get; internal set; }
        public JsonService JsonService { get; internal set; }
        public ApiService ApiService { get; internal set; }
        public TermsOfUseService TermsOfUseService { get; internal set; }
        public FontService FontService { get; internal set; }
        public ResourcesService ResourcesService { get; internal set; }
        public PreloadService PreloadService { get; internal set; }
        public ProfileService ProfileService { get; internal set; }
        public FileResourcesService FileResourcesService { get; internal set; }
        public DashboardService DashboardService { get; internal set; }
        public CrmService CrmService { get; internal set; }
        public MessagingCenter MessagingCenter { get; internal set; }
	}

    public class ImageServiceLogger : IMiniLogger
    {
        readonly SL4N.ILogger LOG = LoggerFactory.GetLogger<ImageService>();

        public void Debug(string message)
        {
            LOG.Debug(message);
        }

        public void Error(string errorMessage)
        {
            LOG.Error(errorMessage);
        }

        public void Error(string errorMessage, Exception ex)
        {
            LOG.Error(errorMessage, ex);
        }
    }
}
