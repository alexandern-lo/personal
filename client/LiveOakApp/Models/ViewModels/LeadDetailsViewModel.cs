using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ServiceStack;
using StudioMobile;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Models.Data.Entities;
using LiveOakApp.Models.Services;
using LiveOakApp.Resources;
using System.Globalization;

namespace LiveOakApp.Models.ViewModels
{
    public class LeadDetailsViewModel : DataContext
    {
        EventsService eventsService;
        LeadsService leadsService;
        FileResourcesService fileResourcesService;
        CrmService crmService;
        const string UNITED_STATES_COUNTRY_NAME = "United States";

        public readonly string[] CountriesList = { UNITED_STATES_COUNTRY_NAME, "", "Afghanistan", "Åand Islands", "Albania", "Algeria", "American Samoa", "Andorra", "Angola", "Anguilla", "Antigua and Barbuda", "Argentina", "Armenia", "Aruba", "Australia", "Austria", "Azerbaijan", "Bangladesh", "Barbados", "Bahamas", "Bahrain", "Belarus", "Belgium", "Belize", "Benin", "Bermuda", "Bhutan", "Bolivia", "Bosnia and Herzegovina", "Botswana", "Brazil", "British Indian Ocean Territory", "British Virgin Islands", "Brunei Darussalam", "Bulgaria", "Burkina Faso", "Burma", "Burundi", "Cambodia", "Cameroon", "Canada", "Cape Verde", "Cayman Islands", "Central African Republic", "Chad", "Chile", "China", "Christmas Island", "Cocos(Keeling) Islands", "Colombia", "Comoros", "Congo-Brazzaville", "Congo-Kinshasa", "Cook Islands", "Costa Rica", "Croatia", "Curaço", "Cyprus", "Czech Republic", "Denmark", "Djibouti", "Dominica", "Dominican Republic", "East Timor", "Ecuador", "El Salvador", "Egypt", "Equatorial Guinea", "Eritrea", "Estonia", "Ethiopia", "Falkland Islands", "Faroe Islands", "Federated States of Micronesia", "Fiji", "Finland", "France", "French Guiana", "French Polynesia", "French Southern Lands", "Gabon", "Gambia", "Georgia", "Germany", "Ghana", "Gibraltar", "Greece", "Greenland", "Grenada", "Guadeloupe", "Guam", "Guatemala", "Guernsey", "Guinea", "Guinea-Bissau", "Guyana", "Haiti", "Heard and McDonald Islands", "Honduras", "Hong Kong", "Hungary", "Iceland", "India", "Indonesia", "Iraq", "Ireland", "Isle of Man", "Israel", "Italy", "Jamaica", "Japan", "Jersey", "Jordan", "Kazakhstan", "Kenya", "Kiribati", "Kuwait", "Kyrgyzstan", "Laos", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg", "Macau", "Macedonia", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Marshall Islands", "Martinique", "Mauritania", "Mauritius", "Mayotte", "Mexico", "Moldova", "Monaco", "Mongolia", "Montenegro", "Montserrat", "Morocco", "Mozambique", "Namibia", "Nauru", "Nepal", "Netherlands", "New Caledonia", "New Zealand", "Nicaragua", "Niger", "Nigeria", "Niue", "Norfolk Island", "Northern Mariana Islands", "Norway", "Oman", "Pakistan", "Palau", "Panama", "Papua New Guinea", "Paraguay", "Peru", "Philippines", "Pitcairn Islands", "Poland", "Portugal", "Puerto Rico", "Qatar", "Rénion", "Romania", "Russia", "Rwanda", "Saint Barthéemy", "Saint Helena", "Saint Kitts and Nevis", "Saint Lucia", "Saint Martin", "Saint Pierre and Miquelon", "Saint Vincent", "Samoa", "San Marino", "Sã Toménd Prícipe", "Saudi Arabia", "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Sint Maarten", "Slovakia", "Slovenia", "Solomon Islands", "Somalia", "South Africa", "South Georgia", "South Korea", "Spain", "Sri Lanka", "Sudan", "Suriname", "Svalbard and Jan Mayen", "Sweden", "Swaziland", "Switzerland", "Syria", "Taiwan", "Tajikistan", "Tanzania", "Thailand", "Togo", "Tokelau", "Tonga", "Trinidad and Tobago", "Tunisia", "Turkey", "Turkmenistan", "Turks and Caicos Islands", "Tuvalu", "Uganda", "Ukraine", "United Arab Emirates", "United Kingdom", "Uruguay", "Uzbekistan", "Vanuatu", "Vatican City", "Vietnam", "Venezuela", "Wallis and Futuna", "Western Sahara", "Yemen", "Zambia", "Zimbabwe" };
        public readonly string[] StatesList = { "", "Other", "Alabama", "Alaska", "American Samoa", "Arizona", "Arkansas", "California", "Colorado", "Connecticut", "Delaware", "District Of Columbia", "Federated States Of Micronesia", "Florida", "Georgia", "Guam", "Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", "Kansas", "Kentucky", "Louisiana", "Maine", "Marshall Islands", "Maryland", "Massachusetts", "Michigan", "Minnesota", "Mississippi", "Missouri", "Montana", "Nebraska", "Nevada", "New Hampshire", "New Jersey", "New Mexico", "New York", "North Carolina", "North Dakota", "Northern Mariana Islands", "Ohio", "Oklahoma", "Oregon", "Palau", "Pennsylvania", "Puerto Rico", "Rhode Island", "South Carolina", "South Dakota", "Tennessee", "Texas", "Utah", "Vermont", "Virgin Islands", "Virginia", "Washington", "West Virginia", "Wisconsin", "Wyoming" };
        public bool IsStatesPickerEnabled { get { return LeadCountry == UNITED_STATES_COUNTRY_NAME; } }

        public void CommonInit()
        {
            eventsService = ServiceLocator.Instance.EventsService;
            leadsService = ServiceLocator.Instance.LeadsService;
            fileResourcesService = ServiceLocator.Instance.FileResourcesService;
            crmService = ServiceLocator.Instance.CrmService;

            LoadEventsCommand = new AsyncCommand
            {
                Action = LoadEventsAction,
                CanExecute = CanExecuteLoadEvents
            };

            CreateEmail = new Command
            {
                Action = CreateEmailAction,
                CanExecute = CanCreateEmail
            };

            CreatePhone = new Command
            {
                Action = CreatePhoneAction,
                CanExecute = CanCreatePhone
            };

            SaveLead = new AsyncCommand
            {
                Action = SaveLeadAction,
                CanExecute = CanExecuteSaveLeadAction
            };

            UpdateLeadLocally = new AsyncCommand
            {
                Action = UpdateLeadLocallyAction,
                CanExecute = CanExecuteUpdateLeadLocallyAction
            };

            DeleteLead = new AsyncCommand
            {
                Action = DeleteLeadAction,
                CanExecute = CanExecuteDeleteLeadAction
            };

            CreateLeadLocally = new AsyncCommand
            {
                Action = CreateLeadLocallyAction,
                CanExecute = CanCreateLeadLocally
            };

            DeleteLead.CanExecuteChanged += (sender, e) => SaveLead.RaiseCanExecuteChanged();

            SetEventCommand = new Command
            {
                Action = SetEventAction
            };

            ExportLeadToCrmCommand = new AsyncCommand
            {
                Action = ExportLeadToCrmAction,
                CanExecute = CanExportLeadToCrm
            };

            _event = Value<EventViewModel>(null);
            _leadUID = Value("");
            _leadName = Value("");
            _leadSurname = Value("");
            _leadCompany = Value("");
            _leadAddress = Value("");
            _leadCity = Value("");
            _leadZipCode = Value("");
            _leadCompanyURL = Value("");
            _leadState = Value("");
            _leadTitle = Value("");
            _leadCountry = Value(UNITED_STATES_COUNTRY_NAME);
            _leadPhotoResource = Value(new FileResource(null, null));
            _leadCardFrontResource = Value(new FileResource(null, null));
            _leadCardBackResource = Value(new FileResource(null, null));
            _leadNotes = Value("");
            _leadFirstEntryLocationName = Value("");
            _leadFirstEntryLocation = Value((MapLocation3D)null);
            _classification = Value(LeadStates.Cold);

            IsFirstTimeEditing = true;
        }

        public LeadDetailsViewModel()
        {
            CommonInit();

            EnsureAtLeastOnePhoneEmailVMExists();
        }

        public LeadDetailsViewModel(Lead lead)
        {
            CommonInit();

            _leadId = lead.LeadRecord?.Id ?? 0;
            var leadEventUID = lead.LeadDTO.EventUID ?? lead.LeadDTO.Event?.UID;
            var leadEvent = eventsService.Events?.FirstOrDefault(_ => _.UID.Equals(leadEventUID));
            if (leadEvent != null)
            {
                _event = Value(new EventViewModel(leadEvent));
            }
            else if (leadEventUID != null)
            {
                _event = Value(EventViewModel.CreateLoadingEvent(leadEventUID));
                LoadEventsCommand.Execute();
            }
            _leadUID = Value(lead.LeadDTO.UID);
            _leadName = Value(lead.LeadDTO.FirstName ?? "");
            _leadSurname = Value(lead.LeadDTO.LastName ?? "");
            _leadCompany = Value(lead.LeadDTO.CompanyName ?? "");
            _leadAddress = Value(lead.LeadDTO.Address ?? "");
            _leadCity = Value(lead.LeadDTO.City ?? "");
            _leadZipCode = Value(lead.LeadDTO.ZipCode ?? "");
            _leadCompanyURL = Value(lead.LeadDTO.CompanyUrl ?? "");
            _leadState = Value(lead.LeadDTO.State ?? "");
            _leadTitle = Value(lead.LeadDTO.JobTitle ?? "");
            _leadCountry = Value(lead.LeadDTO.Country ?? "");
            _leadNotes = Value(lead.LeadDTO.Notes ?? "");
            _leadPhotoResource = Value(lead.PhotoResource);
            _leadCardFrontResource = Value(lead.CardFrontResource);
            _leadCardBackResource = Value(lead.CardBackResource);
            _leadFirstEntryLocationName = Value(lead.LeadDTO.FirstEntryLocation);
            MapLocation3D location = null;
            var lat = lead.LeadDTO.FirstEntryLocationLatitude;
            var lon = lead.LeadDTO.FirstEntryLocationLongitude;
            if (lat.HasValue && lon.HasValue)
            {
                location = new MapLocation3D { Location = new MapLocation(lat.Value, lon.Value) };
            }
            _leadFirstEntryLocation = Value(location);
            _classification = Value(LeadStateFromClassificatonType(lead.LeadDTO.ClassificationEnum));

            AddPhoneVMList(lead.LeadDTO.Phones.ConvertAll(item => new LeadDetailsPhoneViewModel(this, item)));
            AddEmailVMList(lead.LeadDTO.Emails.ConvertAll(item => new LeadDetailsEmailViewModel(this, item)));
            SetupQualifyVMList(lead.LeadDTO.QuestionAnswers);
            EnsureAtLeastOnePhoneEmailVMExists();

            leadExportStatuses.AddRange(lead.LeadDTO.ExportStatuses);
            CalculateCrmExportState(lead);

            IsFirstTimeEditing = false;
        }

        public LeadDetailsViewModel(AttendeeViewModel attendee, EventViewModel eventViewModel)
        {
            CommonInit();

            _event = Value(eventViewModel);
            _leadName = Value(attendee.FirstName ?? "");
            _leadSurname = Value(attendee.LastName ?? "");
            _leadCompany = Value(attendee.Company ?? "");
            _leadTitle = Value(attendee.Title ?? "");
            _leadPhotoResource = Value(new FileResource(null, attendee.AvatarUrl));

            AddPhoneVM(new LeadDetailsPhoneViewModel(this, attendee.Phone));
            AddEmailVM(new LeadDetailsEmailViewModel(this, attendee.Email));
            SetupQualifyVMList(new List<LeadQuestionAnswerDTO>());
            EnsureAtLeastOnePhoneEmailVMExists();

            PerformSave();
        }

        public LeadDetailsViewModel(Card card, EventViewModel eventViewModel)
        {
            CommonInit();

            _event = Value(eventViewModel);
            _leadName = Value(card.Name ?? "");
            _leadSurname = Value(card.Surname ?? "");
            _leadCompany = Value(card.Company ?? "");
            _leadAddress = Value(card.Address ?? "");
            _leadCity = Value(card.City ?? "");
            _leadZipCode = Value(card.ZipCode ?? "");
            _leadCompanyURL = Value(card.CompanyURL ?? "");
            _leadState = Value(card.State ?? "");
            _leadTitle = Value(card.Title ?? "");
            _leadCountry = Value(card.Country ?? "");

            AddPhoneVMList(card.Phones.ConvertAll(item => new LeadDetailsPhoneViewModel(this, item)));
            AddEmailVMList(card.Emails.ConvertAll(item => new LeadDetailsEmailViewModel(this, item)));
            EnsureAtLeastOnePhoneEmailVMExists();

            PerformSave();
        }

        void EnsureAtLeastOnePhoneEmailVMExists()
        {
            if (!PhoneViewModels.Any())
            {
                AddPhoneVM(new LeadDetailsPhoneViewModel(this));
            }
            if (!EmailViewModels.Any())
            {
                AddEmailVM(new LeadDetailsEmailViewModel(this));
            }
        }

        #region QualifyTab open

        public bool EnsureCanSelectQualifyTab()
        {
            if (Event != null) return true;
            if (!IsNoEventSelectedErrorShown) FlashIsNoEventSelectedErrorShownProperty().Ignore();
            return false;
        }

        public bool HasAnsweredQuestions
        {
            get
            {
                return Questions.Any((question) => question.CheckedAnswers != null && !question.CheckedAnswers.IsNullOrEmpty());
            }
        }

        async Task FlashIsNoEventSelectedErrorShownProperty()
        {
            IsNoEventSelectedErrorShown = true;
            await Task.Delay(2500);
            IsNoEventSelectedErrorShown = false;
        }

        bool _isNoEventSelectedErrorShown;
        public bool IsNoEventSelectedErrorShown
        {
            get { return _isNoEventSelectedErrorShown || Event == null; }
            set
            {
                _isNoEventSelectedErrorShown = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Lead serialization

        LeadDTO DTOFromCurrentData()
        {
            var location = _leadFirstEntryLocation.Value;
            var answers = new List<LeadQuestionAnswerDTO>();
            Questions.Each((question, index) =>
            {
                if (question.LeadQuestionAnswersDTO != null)
                    answers.AddRange(question.LeadQuestionAnswersDTO);
            });

            var phonesDTO = new List<PhoneDTO>(PhoneViewModels.Select(_ => _.PhoneDTO));
            var emailsDTO = new List<EmailDTO>(EmailViewModels.Select(_ => _.EmailDTO));
            phonesDTO.Each((phone, index) => phone.Phone = phone.Phone?.Trim());
            emailsDTO.Each((email, index) => email.Email = email.Email?.Trim());

            return new LeadDTO
            {
                UID = _leadUID?.Trim().ToNullIfEmpty(),
                EventUID = Event?.UID?.Trim().ToNullIfEmpty(),
                FirstName = _leadName.Value?.Trim().ToNullIfEmpty(),
                LastName = _leadSurname.Value?.Trim().ToNullIfEmpty(),
                CompanyName = _leadCompany.Value?.Trim().ToNullIfEmpty(),
                Address = _leadAddress.Value?.Trim().ToNullIfEmpty(),
                City = _leadCity.Value?.Trim().ToNullIfEmpty(),
                ZipCode = _leadZipCode.Value?.Trim().ToNullIfEmpty(),
                CompanyUrl = _leadCompanyURL.Value?.Trim().ToNullIfEmpty(),
                State = _leadState.Value?.Trim().ToNullIfEmpty(),
                JobTitle = _leadTitle.Value?.Trim().ToNullIfEmpty(),
                Country = _leadCountry.Value?.Trim().ToNullIfEmpty(),
                Notes = _leadNotes.Value?.Trim().ToNullIfEmpty(),
                PhotoUrl = _leadPhotoResource.Value.RemoteUrl?.Trim().ToNullIfEmpty(),
                BusinessCardFrontUrl = _leadCardFrontResource.Value.RemoteUrl?.Trim().ToNullIfEmpty(),
                BusinessCardBackUrl = _leadCardBackResource.Value.RemoteUrl?.Trim().ToNullIfEmpty(),
                FirstEntryLocation = _leadFirstEntryLocationName.Value?.Trim().ToNullIfEmpty(),
                FirstEntryLocationLatitude = location != null ? location.Location.Latitude : (double?)null,
                FirstEntryLocationLongitude = location != null ? location.Location.Longitude : (double?)null,
                ClassificationEnum = LeadStateToClassificatonType(_classification),
                ExportStatuses = leadExportStatuses,

                Phones = phonesDTO.ThenDo((phones) => phones.Each((phone, index) => phone.Phone = phone.Phone?.Trim())).ToList(),
                Emails = emailsDTO.ThenDo((emails) => emails.Each((email, index) => email.Email = email.Email?.Trim())).ToList(),
                QuestionAnswers = answers
            };
        }

        #endregion

        #region Save

        public void PerformSave()
        {
            PerformSaveAsync().Ignore();
        }

        async Task PerformSaveAsync()
        {
            await SaveLead.CancelCommand.ExecuteAsync();
            await SaveLead.ExecuteAsync();
        }

        #endregion

        #region Lead actions

        public AsyncCommand SaveLead { get; private set; }

        async Task SaveLeadAction(object arg)
        {
            var dto = DTOFromCurrentData();
            var photo = _leadPhotoResource.Value;
            var cardFront = _leadCardFrontResource.Value;
            var cardBack = _leadCardBackResource.Value;
            _leadId = await leadsService.SaveLead(_leadId, dto, photo, cardFront, cardBack, SaveLead.Token);
            CrmExportState = CrmExportStates.NotExported;
        }

        bool CanExecuteSaveLeadAction(object arg)
        {
            return !SaveLead.IsRunning &&
                            !DeleteLead.IsRunning &&
                            HasValidFields;
        }

        public AsyncCommand UpdateLeadLocally { get; private set; }

        async Task UpdateLeadLocallyAction(object arg)
        {
            var dto = DTOFromCurrentData();
            var photo = _leadPhotoResource.Value;
            var cardFront = _leadCardFrontResource.Value;
            var cardBack = _leadCardBackResource.Value;
            _leadId = await leadsService.UpdateLeadLocally(_leadId, dto, photo, cardFront, cardBack, SaveLead.Token);
        }

        bool CanExecuteUpdateLeadLocallyAction(object arg)
        {
            return !SaveLead.IsRunning && !DeleteLead.IsRunning && !UpdateLeadLocally.IsRunning;
        }

        public AsyncCommand DeleteLead { get; private set; }

        async Task DeleteLeadAction(object arg)
        {
            var dto = DTOFromCurrentData();
            var photo = _leadPhotoResource.Value;
            var cardFront = _leadCardFrontResource.Value;
            var cardBack = _leadCardBackResource.Value;
            _leadId = await leadsService.DeleteLead(_leadId, dto, photo, cardFront, cardBack, DeleteLead.Token);
        }

        bool CanExecuteDeleteLeadAction(object arg)
        {
            return !DeleteLead.IsRunning;
        }

        public AsyncCommand CreateLeadLocally { get; private set; }

        async Task CreateLeadLocallyAction(object arg)
        {
            var dto = DTOFromCurrentData();
            var photo = _leadPhotoResource.Value;
            var cardFront = _leadCardFrontResource.Value;
            var cardBack = _leadCardBackResource.Value;
            _leadId = await leadsService.CreateLeadLocallyIfNeeded(_leadId, dto, photo, cardFront, cardBack, CreateLeadLocally.Token);
            ExportLeadToCrmCommand.RaiseCanExecuteChanged();
        }

        bool CanCreateLeadLocally(object arg)
        {
            return !CreateLeadLocally.IsRunning;
        }

        #endregion

        #region Email actions

        public Command CreateEmail { get; private set; }

        void CreateEmailAction(object arg)
        {
            AddEmailVM(new LeadDetailsEmailViewModel(this));
            PerformSave();
        }

        bool CanCreateEmail(object arg)
        {
            if (EmailViewModels.Count == 0) return true;
            var lastEmailVM = EmailViewModels[EmailViewModels.Count - 1];
            return EmailViewModels.Count < 4 && lastEmailVM.IsEmailValid && !string.IsNullOrEmpty(lastEmailVM.Email);
        }

        void AddEmailVM(LeadDetailsEmailViewModel emailVM)
        {
            EmailViewModels.Add(emailVM);
            CreateEmail.RaiseCanExecuteChanged();
            RaisePropertyChanged(() => HasNameOrEmail);
            RaisePropertyChanged(() => HasValidFields);
        }

        void AddEmailVMList(List<LeadDetailsEmailViewModel> emailVMList)
        {
            EmailViewModels.AddRange(emailVMList);
            CreateEmail.RaiseCanExecuteChanged();
            RaisePropertyChanged(() => HasNameOrEmail);
            RaisePropertyChanged(() => HasValidFields);
        }

        public void RemoveEmailVM(LeadDetailsEmailViewModel emailVM)
        {
            if (EmailViewModels.Count == 1) return;
            EmailViewModels.Remove(emailVM);
            CreateEmail.RaiseCanExecuteChanged();
            RaisePropertyChanged(() => HasNameOrEmail);
            RaisePropertyChanged(() => HasValidFields);
            PerformSave();
        }

        #endregion

        #region Phone actions

        public Command CreatePhone { get; private set; }

        void CreatePhoneAction(object arg)
        {
            AddPhoneVM(new LeadDetailsPhoneViewModel(this));
            PerformSave();
        }

        bool CanCreatePhone(object arg) 
        {
            if (PhoneViewModels.Count == 0) return true;
            var lastPhoneVM = PhoneViewModels[PhoneViewModels.Count - 1];
            return EmailViewModels.Count < 4 && !string.IsNullOrEmpty(lastPhoneVM.Phone);
        }

        void AddPhoneVM(LeadDetailsPhoneViewModel phoneVM)
        {
            PhoneViewModels.Add(phoneVM);
            CreatePhone.RaiseCanExecuteChanged();
        }

        void AddPhoneVMList(List<LeadDetailsPhoneViewModel> phoneVMList)
        {
            PhoneViewModels.AddRange(phoneVMList);
            CreatePhone.RaiseCanExecuteChanged();
        }

        public void RemovePhoneVM(LeadDetailsPhoneViewModel phoneVM)
        {
            if (PhoneViewModels.Count == 1) return;
            PhoneViewModels.Remove(phoneVM);
            CreatePhone.RaiseCanExecuteChanged();
            PerformSave();
        }

        #endregion

        #region Events

        public ObservableList<EventViewModel> Events = new ObservableList<EventViewModel>();

        public AsyncCommand LoadEventsCommand { get; private set; }

        async Task LoadEventsAction(object arg)
        {
            await eventsService.LoadEventsIfNeeded(LoadEventsCommand.Token);
            var eventsVMs = eventsService.SelectableEvents.ConvertAll((EventDTO input) => new EventViewModel(input));
            Events.Reset(eventsVMs);

            var eventUID = Event.UID;
            if (eventUID == null) return;
            var leadEvent = eventsService.Events?.FirstOrDefault(_ => _.UID.Equals(eventUID));
            if (leadEvent != null)
            {
                Event = new EventViewModel(leadEvent);
            }
        }

        bool CanExecuteLoadEvents(object arg)
        {
            return !LoadEventsCommand.IsRunning;
        }

        #endregion

        #region Files

        public float MaxImageDimention { get; } = 1024;

        public void ReplacePhoto(string tempAbsLocalPath)
        {
            LeadPhotoResource = fileResourcesService.CopyFileToPermanentPath(tempAbsLocalPath);
        }

        public void ReplaceBusinessCardFront(string tempAbsLocalPath)
        {
            LeadCardFrontResource = fileResourcesService.CopyFileToPermanentPath(tempAbsLocalPath);
        }

        public void ReplaceBusinessCardBack(string tempAbsLocalPath)
        {
            LeadCardBackResource = fileResourcesService.CopyFileToPermanentPath(tempAbsLocalPath);
        }

        #endregion

        #region Geocoding

        bool IsFirstTimeEditing { get; set; }

        ILocationManager LocationManager { get; set; }
        IAddressGeocoder AddressGeocoder { get; set; }

        public void StartDetectingLocationIfNeeded(Func<ILocationManager> locationManagerFactory, Func<IAddressGeocoder> geocoderFactory)
        {
            if (LeadFirstEntryLocationName.IsNullOrEmpty())
            {
                if (AddressGeocoder == null)
                {
                    AddressGeocoder = geocoderFactory();
                }
                GeocodeIfNeeded();
            }

            if (IsFirstTimeEditing && LeadFirstEntryLocation == null)
            {
                if (LocationManager == null)
                {
                    try
                    {
                        LocationManager = locationManagerFactory();
                        LocationManager.RequestForegroundPermissions();
                        LocationManager.StartUpdatingLocation();
                        LocationManager.LocationUpdated = LocationManager_LocationUpdated;
                    }
                    catch (Exception e)
                    {
                        LOG.Error("DetectingLocation failed", e);
                        StopDetectingLocation();
                    }
                }
            }
        }

        public void StopDetectingLocation()
        {
            var manager = LocationManager;
            if (manager != null)
            {
                manager.LocationUpdated = null;
                manager.StopUpdatingLocation();
                LocationManager = null;
            }
            CancelGeocoding();
        }

        void LocationManager_LocationUpdated(ILocationManager lm)
        {
            if (LocationManager == null) return;
            var location = LocationManager.Location;
            LeadFirstEntryLocation = location;
            StopDetectingLocation();
            LOG.Debug("location: {0},{1}", location.Location.Latitude, location.Location.Longitude);
            GeocodeIfNeeded();
        }

        Task geocodingTask;
        CancellationTokenSource geocodingCancellationSource;

        void GeocodeIfNeeded()
        {
            if (LeadFirstEntryLocation == null || !LeadFirstEntryLocationName.IsNullOrEmpty()) return;
            if (geocodingTask != null) return;
            geocodingCancellationSource = new CancellationTokenSource();
            geocodingTask = GeocodeAsyncNoThrow();
        }

        void CancelGeocoding()
        {
            geocodingCancellationSource?.Cancel();
        }

        async Task GeocodeAsyncNoThrow()
        {
            try
            {
                await GeocodeAsync();
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception e)
            {
                LOG.Error("geocoding failed", e);
            }
            geocodingTask = null;
        }

        async Task GeocodeAsync()
        {
            var result = await AddressGeocoder.Geocode(LeadFirstEntryLocation.Location, geocodingCancellationSource.Token);
            LOG.Debug("geocoded: {0}", result.Select(_ => _.ToString()).Join(","));
            if (result.Count <= 0) return;
            var address = result[0];
            LeadFirstEntryLocationName = address.ToString();
            LOG.Debug("address: {0}", address);
            AddressGeocoder = null;
        }

        #endregion

        #region CRM Export

        public CrmService.CrmType CurrentCrmType
        {
            get
            {
                return crmService.CurrentCrmType;
            }
        }

        public AsyncCommand ExportLeadToCrmCommand;

        async Task ExportLeadToCrmAction(object arg)
        {
            try
            {
                await CreateLeadLocally.ExecuteAsync();
            }
            catch (Exception error)
            {
                LOG.Warn("failed to save lead before Crm export", error);
            }
            var exportedNow = await leadsService.ExportLeadToCRM(_leadId, ExportLeadToCrmCommand.Token);
            if (exportedNow != null && exportedNow.TotalFailed == 0)
            {
                CrmExportState = CrmExportStates.Exported;
                var newExportedAt = exportedNow.TotalCreated != 0 ? exportedNow.CreatedLeads[0].ExportedAt : exportedNow.UpdatedLeads[0].ExportedAt;
                var currentCrmConfiguration = crmService.CurrentCrmConfiguration;
                var currentExportStatus = leadExportStatuses.FirstOrDefault(_ => _.UserCrmConfigurationUid == currentCrmConfiguration?.Uid);
                if (currentExportStatus != null)
                    currentExportStatus.ExportedAt = newExportedAt;
                else
                    leadExportStatuses.Add(new LeadExportStatusDTO() { UserCrmConfigurationUid = currentCrmConfiguration?.Uid, ExportedAt = newExportedAt });
            }
            else
            {
                CrmExportState = CrmExportStates.ExportScheduled;
            }
            await UpdateLeadLocally.ExecuteAsync();
        }

        bool CanExportLeadToCrm(object arg)
        {
            return crmService.CurrentCrmIsEnabled &&
                             CrmExportState == CrmExportStates.NotExported &&
                             HasValidFields;
        }

        public enum CrmExportStates
        {
            NotExported,
            Exported,
            ExportScheduled
        }

        CrmExportStates _crmExportState = CrmExportStates.NotExported;
        public CrmExportStates CrmExportState
        {
            get { return _crmExportState; }
            set
            {
                _crmExportState = value;
                RaisePropertyChanged();
                ExportLeadToCrmCommand.RaiseCanExecuteChanged();
            }
        }

        void CalculateCrmExportState(Lead lead)
        {
            var currentCrmConfig = crmService.CurrentCrmConfiguration?.Uid;
            if (currentCrmConfig == null)
            {
                CrmExportState = CrmExportStates.NotExported;
                return;
            }
            var status = lead.LeadDTO?.ExportStatuses.FirstOrDefault(
                _ => _.UserCrmConfigurationUid == currentCrmConfig);

            var exportedAt = status?.ExportedAt;
            if (lead.LeadRecord?.ExportRequiredAt == null && (status == null || exportedAt == null))
            {
                // never exported
                CrmExportState = CrmExportStates.NotExported;
                return;
            }
            var clientUpdatedAt = lead.LeadDTO.ClientsideUpdatedAt ?? lead.LeadDTO.UpdatedAt;
            if (lead.LeadRecord == null)
            {
                if (clientUpdatedAt == null)
                {
                    // server should always send ClientsideUpdatedAt or UpdatedAt
                    // TODO: we can't assume anything in this case
                    CrmExportState = CrmExportStates.Exported;
                    return;
                }
                if (clientUpdatedAt > exportedAt)
                {
                    CrmExportState = CrmExportStates.NotExported;
                }
                else
                {
                    CrmExportState = CrmExportStates.Exported;
                }
                return;
            }
            var exportRequiredAt = lead.LeadRecord.ExportRequiredAt;
            var uploadRequiredAt = lead.LeadRecord.UploadRequiredAt;
            if (uploadRequiredAt != null
                && (uploadRequiredAt > exportRequiredAt || exportRequiredAt == null))
            {
                // changed after export
                CrmExportState = CrmExportStates.NotExported;
                return;
            }
            if (exportRequiredAt != null
                && exportRequiredAt > exportedAt || exportedAt == null)
            {
                // export requested after previous export
                CrmExportState = CrmExportStates.ExportScheduled;
            }
            else
            {
                // actual export is after requested export
                CrmExportState = CrmExportStates.Exported;
            }
        }

        #endregion

        #region ContactTab Form

        Field<EventViewModel> _event;

        int _leadId;
        string _leadUID;

        Field<string> _leadName;
        Field<string> _leadSurname;
        Field<FileResource> _leadPhotoResource;
        Field<FileResource> _leadCardFrontResource;
        Field<FileResource> _leadCardBackResource;
        Field<string> _leadCompany;
        Field<string> _leadAddress;
        Field<string> _leadCity;
        Field<string> _leadZipCode;
        Field<string> _leadCompanyURL;
        Field<string> _leadState;
        Field<string> _leadTitle;
        Field<string> _leadCountry;
        Field<string> _leadNotes;
        Field<string> _leadFirstEntryLocationName;
        Field<MapLocation3D> _leadFirstEntryLocation;

        List<LeadExportStatusDTO> leadExportStatuses = new List<LeadExportStatusDTO>();
        public ObservableList<LeadDetailsPhoneViewModel> PhoneViewModels = new ObservableList<LeadDetailsPhoneViewModel>();
        public ObservableList<LeadDetailsEmailViewModel> EmailViewModels = new ObservableList<LeadDetailsEmailViewModel>();

        public EventViewModel Event
        {
            get { return _event.Value; }
            set
            {
                if (Equals(value, Event))
                    return;

                if (!value.IsSameUID(Event))
                {
                    _event.SetValue(value);
                    PerformSave();
                }
                else
                    _event.SetValue(value);
                IsNoEventSelectedErrorShown = Event == null;
                RaisePropertyChanged(() => HasValidFields);
                RaisePropertyChanged(() => IsLeadEventValid);
                ResetQuestions();

            }
        }

        public bool IsLeadEventValid
        {
            get
            {
                return Event != null;
            }
        }

        public string LeadName
        {
            get { return _leadName.Value; }
            set
            {
                _leadName.SetValue(value.Trim());
                RaisePropertyChanged(() => HasNameOrEmail);
                RaisePropertyChanged(() => HasValidFields);
                RaisePropertyChanged(() => IsLeadNameValid);
                RaisePropertyChanged(() => FullName);
                PerformSave();
            }
        }

        public bool IsLeadNameValid
        {
            get
            {
                var length = LeadName.Length;
                return length > 1 && length < 200;
            }
        }

        public string LeadSurname
        {
            get { return _leadSurname.Value; }
            set
            {
                _leadSurname.SetValue(value.Trim());
                RaisePropertyChanged(() => IsLeadSurnameValid);
                RaisePropertyChanged(() => HasValidFields);
                RaisePropertyChanged(() => FullName);
                PerformSave();
            }
        }

        public bool IsLeadSurnameValid
        {
            get
            {
                var length = LeadSurname.Length;
                return length > 1 && length < 200 || length == 0;
            }
        }

        public string FullName
        {
            get
            {
                var parts = new List<string> { LeadName, LeadSurname };
                var result = string.Join(" ", parts.Where(_ => !_.IsNullOrEmpty()));
                if (!result.IsNullOrEmpty()) return result;
                var email = EmailViewModels.FirstOrDefault((arg) => !string.IsNullOrWhiteSpace(arg.Email));
                if (email == null)
                    return L10n.Localize("LeadDefaultName", "Lead");
                return email.Email;
            }
        }

        public FileResource LeadPhotoResource
        {
            get { return _leadPhotoResource.Value; }
            private set
            {
                _leadPhotoResource.SetValue(value);
                PerformSave();
            }
        }

        public FileResource LeadCardFrontResource
        {
            get { return _leadCardFrontResource.Value; }
            private set
            {
                _leadCardFrontResource.SetValue(value);
                PerformSave();
            }
        }

        public FileResource LeadCardBackResource
        {
            get { return _leadCardBackResource.Value; }
            private set
            {
                _leadCardBackResource.SetValue(value);
                PerformSave();
            }
        }

        public string LeadCompany
        {
            get { return _leadCompany.Value; }
            set
            {
                _leadCompany.SetValue(value.Trim());
                RaisePropertyChanged(() => IsLeadCompanyValid);
                RaisePropertyChanged(() => HasValidFields);
                PerformSave();
            }
        }

        public bool IsLeadCompanyValid
        {
            get
            {
                var length = LeadCompany.Length;
                return length > 1 && length < 200 || length == 0;
            }
        }

        public string LeadAddress
        {
            get { return _leadAddress.Value; }
            set
            {
                _leadAddress.SetValue(value.Trim());
                RaisePropertyChanged(() => IsLeadAddressValid);
                RaisePropertyChanged(() => HasValidFields);
                PerformSave();
            }
        }

        public bool IsLeadAddressValid
        {
            get
            {
                var length = LeadAddress.Length;
                return length > 1 && length < 200 || length == 0;
            }
        }

        public string LeadCity
        {
            get { return _leadCity.Value; }
            set
            {
                _leadCity.SetValue(value.Trim());
                RaisePropertyChanged(() => IsLeadCityValid);
                RaisePropertyChanged(() => HasValidFields);
                PerformSave();
            }
        }

        public bool IsLeadCityValid
        {
            get
            {
                var length = LeadCity.Length;
                return length > 1 && length < 200 || length == 0;
            }
        }

        public string LeadZip
        {
            get { return _leadZipCode.Value; }
            set
            {
                _leadZipCode.SetValue(value.Trim());
                RaisePropertyChanged(() => IsLeadZipValid);
                RaisePropertyChanged(() => HasValidFields);
                PerformSave();
            }
        }

        public bool IsLeadZipValid
        {
            get
            {
                var length = LeadZip.Length;
                return length > 1 && length < 200 || length == 0;
            }
        }

        public string LeadState
        {
            get { return _leadState.Value; }
            set
            {
                _leadState.SetValue(value.Trim());
                RaisePropertyChanged(() => IsLeadStateValid);
                RaisePropertyChanged(() => HasValidFields);
                PerformSave();
            }
        }

        public bool IsLeadStateValid
        {
            get
            {
                var length = LeadState.Length;
                return length > 1 && length < 200 || length == 0;
            }
        }

        public string LeadNotes
        {
            get { return _leadNotes.Value; }
            set
            {
                _leadNotes.SetValue(value.Trim());
                RaisePropertyChanged(() => LeadNotes);
                RaisePropertyChanged(() => IsLeadNotesValid);
                RaisePropertyChanged(() => HasValidFields);
                PerformSave();
            }
        }

        public bool IsLeadNotesValid
        {
            get
            {
                var length = LeadNotes.Length;
                return length > 1 && length < 180 || length == 0;
            }
        }

        public string LeadFirstEntryLocationName
        {
            get { return _leadFirstEntryLocationName.Value; }
            set
            {
                _leadFirstEntryLocationName.SetValue(value.Trim());
                RaisePropertyChanged(() => LeadFirstEntryLocationDescription);
                PerformSave();
            }
        }

        public string LeadFirstEntryLocationDescription
        {
            get
            {
                if (!LeadFirstEntryLocationName.IsNullOrEmpty())
                    return LeadFirstEntryLocationName;
                if (LeadFirstEntryLocation != null)
                    return string.Format("{0},{1}", LeadFirstEntryLocation.Location.Latitude, LeadFirstEntryLocation.Location.Longitude);
                if (IsFirstTimeEditing)
                    return L10n.Localize("LeadEntryLocationAcquiringLabel", "Acquiring location");
                return L10n.Localize("LeadEntryLocationUnknownLabel", "Unknown location");
            }
        }

        public MapLocation3D LeadFirstEntryLocation
        {
            get { return _leadFirstEntryLocation.Value; }
            set
            {
                _leadFirstEntryLocation.SetValue(value);
                PerformSave();
            }
        }

        public string LeadCompanyURL
        {
            get { return _leadCompanyURL.Value; }
            set
            {
                _leadCompanyURL.SetValue(value.Trim());
                RaisePropertyChanged(() => IsLeadCompanyURLValid);
                RaisePropertyChanged(() => HasValidFields);
                PerformSave();
            }
        }

        public bool IsLeadCompanyURLValid
        {
            get
            {
                var length = LeadCompanyURL.Length;
                return length > 1 && length < 200 || length == 0;
            }
        }

        public string LeadTitle
        {
            get { return _leadTitle.Value; }
            set
            {
                _leadTitle.SetValue(value.Trim());
                RaisePropertyChanged(() => IsLeadTitleValid);
                RaisePropertyChanged(() => HasValidFields);
                PerformSave();
            }
        }

        public bool IsLeadTitleValid
        {
            get
            {
                var length = LeadTitle.Length;
                return length > 1 && length < 200 || length == 0;
            }
        }

        public string LeadCountry
        {
            get { return _leadCountry.Value; }
            set
            {
                if (_leadCountry.Value.Equals(value)) return;
                if (_leadCountry.Value.Equals(UNITED_STATES_COUNTRY_NAME) || value.Trim().Equals(UNITED_STATES_COUNTRY_NAME))
                    LeadState = "";
                _leadCountry.SetValue(value.Trim());
                RaisePropertyChanged(() => IsStatesPickerEnabled);
                RaisePropertyChanged(() => IsLeadCountryValid);
                RaisePropertyChanged(() => HasValidFields);
                PerformSave();
            }
        }

        public bool IsLeadCountryValid
        {
            get
            {
                var length = LeadCountry.Length;
                return length > 1 && length < 200 || length == 0;
            }
        }

        public bool HasValidFields
        {
            get
            {
                return Event != null 
                    && HasNameOrEmail
                    && IsLeadNameValid
                    && IsLeadSurnameValid
                    && IsLeadTitleValid
                    && IsLeadCountryValid
                    && IsLeadStateValid
                    && IsLeadCityValid
                    && IsLeadAddressValid
                    && IsLeadZipValid
                    && IsLeadCompanyValid
                    && IsLeadCompanyURLValid
                    && IsLeadNotesValid;
            }
        }

        public bool HasNameOrEmail
        {
            get
            {
                var hasEmail = EmailViewModels.FirstOrDefault((arg) => arg.IsEmailValid) != null;
                return IsLeadNameValid || (EmailViewModels.Count() > 0 && hasEmail);
            }
        }

        public void RaiseHasValidFieldsChanged()
        {
            RaisePropertyChanged(() => HasNameOrEmail);
            RaisePropertyChanged(() => HasValidFields);
        }

        #endregion

        #region QualifyTab Form

        Field<LeadStates> _classification;

        public LeadStates Classification
        {
            get { return _classification.Value; }
            set
            {
                _classification.SetValue(value);
                RaisePropertyChanged(() => CurrentState);
                PerformSave();
            }
        }

        public int CurrentState
        {
            get { return AllStates().IndexOf(Classification); }
            set { Classification = AllStates()[value]; }
        }

        public ObservableList<LeadDetailsQuestionViewModel> Questions = new ObservableList<LeadDetailsQuestionViewModel>();

        public ObservableList<LeadStates> States = new ObservableList<LeadStates>(AllStates());

        #endregion

        #region QualifyTab UI state

        int _currentQuestion;
        public int CurrentQuestion
        {
            get { return _currentQuestion; }
            set
            {
                _currentQuestion = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region QualifyTab questions

        void SetupQualifyVMList(List<LeadQuestionAnswerDTO> questionAnswers)
        {
            if (Event == null) return;
            var answersByQuestionUid = questionAnswers.GroupBy(_ => _.QuestionUID);
            var newQuestions = Event.EventDTO.Questions.OrderBy(_ => _.Position).Select(_ =>
            {
                var result = new List<LeadQuestionAnswerDTO>();
                answersByQuestionUid.FirstOrDefault((answers) =>
                {
                    if (answers.Key != _.UID) return false;
                    result.AddRange(answers.ToList());
                    return true;
                });
                return new LeadDetailsQuestionViewModel(this, _, result);
            });
            Questions.Reset(newQuestions);
        }

        void ResetQuestions()
        {
            var newQuestions = Event.EventDTO.Questions.ConvertAll(_ => new LeadDetailsQuestionViewModel(this, _, null));
            Questions.Reset(newQuestions);
        }

        #endregion

        #region QualifyTab commands

        public Command SetEventCommand { get; private set; }

        void SetEventAction(object eventObj)
        {
            Event = (EventViewModel)eventObj;
        }

        #endregion

        #region LeadStates

        public enum LeadStates
        {
            Hot,
            Warm,
            Cold
        }

        static List<LeadStates> AllStates()
        {
            return new List<LeadStates> { LeadStates.Cold, LeadStates.Warm, LeadStates.Hot };
        }

        static LeadStates LeadStateFromClassificatonType(LeadDTO.ClassificationType classification)
        {
            switch (classification)
            {
                case LeadDTO.ClassificationType.Hot:
                    return LeadStates.Hot;
                case LeadDTO.ClassificationType.Warm:
                    return LeadStates.Warm;
                case LeadDTO.ClassificationType.Cold:
                    return LeadStates.Cold;
                default:
                    return LeadStates.Cold;
            }
        }

        static LeadDTO.ClassificationType LeadStateToClassificatonType(LeadStates state)
        {
            switch (state)
            {
                case LeadStates.Hot:
                    return LeadDTO.ClassificationType.Hot;
                case LeadStates.Warm:
                    return LeadDTO.ClassificationType.Warm;
                case LeadStates.Cold:
                    return LeadDTO.ClassificationType.Cold;
                default:
                    return LeadDTO.ClassificationType.Cold;
            }
        }

        #endregion
    }
}
