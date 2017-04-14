using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ServiceStack;
using SL4N;
using StudioMobile;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Models.Data.Records;
using System.Linq;
using System.Net;
using System.IO;
using FFImageLoading;
using System.Collections;

namespace LiveOakApp.Models.Services
{
    public class LeadsUploadService
    {
        static readonly ILogger LOG = LoggerFactory.GetLogger<LeadsUploadService>();

        readonly LeadStorage LeadStorage;
        readonly JsonService JsonService;
        readonly ApiService ApiService;
        readonly FileResourcesService FileResourcesService;
        readonly MessagingCenter MessagingCenter;

        const int DelayMinutesAfterError = 1;

        CancellationTokenSource TokenSource;
        TimeSpan NextUploadDelay { get; set; } = new TimeSpan(0);
        bool UploadFinished { get; set; }
        bool IsUploading { get; set; }
        bool IsUploadingContinuously { get; set; }

        int lastLeadId = -1;
        HashSet<int> failedLeadIds = new HashSet<int>();
        Dictionary<int, int> repeatedlyFailedLeadIds = new Dictionary<int, int>();

        Task<bool> currentUploadLeadTask;
        int currentUploadLeadId = -1;

        int manuallyUploadingLeadId = -1;

        public LeadsUploadService(LeadStorage leadStorage, JsonService jsonService, ApiService apiService, FileResourcesService fileResourcesService, MessagingCenter messagingCenter)
        {
            LeadStorage = leadStorage;
            JsonService = jsonService;
            ApiService = apiService;
            FileResourcesService = fileResourcesService;
            MessagingCenter = messagingCenter;
        }

        bool uploadingEnabled;
        public bool UploadingEnabled
        {
            get
            {
                return uploadingEnabled;
            }
            set
            {
                if (uploadingEnabled == value) return;
                uploadingEnabled = value;
                if (uploadingEnabled)
                {
                    TokenSource = new CancellationTokenSource();
                    StartUploadLeadsContinuouslyIfNeeded();
                }
                else
                {
                    TokenSource.Cancel();
                }
            }
        }

        public void StartUploadLeadsContinuouslyIfNeeded()
        {
            LOG.Debug("Called StartUploadLeadsContinuouslyIfNeeded");
            UploadFinished = false;
            UploadLeadsContinuously().Ignore();
        }

        public async Task<bool> ManuallyUploadLeadById(int leadId, CancellationToken cancellationToken)
        {
            LOG.Debug("Called ManuallyUploadLeadById: {0}", leadId);
            return await DoManuallyUploadLeadById(leadId, cancellationToken);
        }

        async Task UploadLeadsContinuously()
        {
            if (IsUploadingContinuously) return;
            try
            {
                IsUploadingContinuously = true;
                while (UploadingEnabled && !UploadFinished)
                {
                    LOG.Debug("Waiting {0} before next upload", NextUploadDelay);
                    await Task.Delay(NextUploadDelay, TokenSource.Token);
                    NextUploadDelay = new TimeSpan(0);
                    if (TokenSource.IsCancellationRequested) break;
                    await UploadNextLead(TokenSource.Token);
                }
            }
            finally
            {
                await ExportLeadsToCRMIfNeeded(CancellationToken.None);
                IsUploadingContinuously = false;
            }
        }

        async Task UploadNextLead(CancellationToken cancellationToken)
        {
            if (IsUploading) return;
            try
            {
                IsUploading = true;
                var uploaded = await DoUploadNextLead(failedLeadIds, cancellationToken);
                if (!uploaded)
                {
                    if (failedLeadIds.Count > 0)
                    {
                        LOG.Debug("No more leads to upload ({0} upload errors)", failedLeadIds.Count);
                        NextUploadDelay = new TimeSpan(0, DelayMinutesAfterError, 0);
                        failedLeadIds.Clear();
                    }
                    else
                    {
                        UploadFinished = true;
                        LOG.Debug("No more leads to upload");
                    }
                }
            }
            catch (WebException error)
            {
                LOG.Warn("failed to upload lead (no internet)", error);
                NextUploadDelay = new TimeSpan(0, DelayMinutesAfterError, 0);
            }
            catch (Exception error)
            {
                if (lastLeadId >= 0)
                {
                    failedLeadIds.Add(lastLeadId);
                    if (repeatedlyFailedLeadIds.ContainsKey(lastLeadId))
                        repeatedlyFailedLeadIds[lastLeadId]++;
                    else
                        repeatedlyFailedLeadIds[lastLeadId] = 1;
                    if (repeatedlyFailedLeadIds[lastLeadId] > 10)
                    {
                        LOG.Error("failed to upload lead {0} multiple times: {1}", lastLeadId, error);
                    }
                }
                LOG.Warn("failed to upload lead", error);
            }
            finally
            {
                IsUploading = false;
            }
        }

        async Task<bool> DoUploadNextLead(IEnumerable<int> skipLeadIds, CancellationToken cancellationToken)
        {
            var skipLeadIdsAndManualLeadId = skipLeadIds;
            if (manuallyUploadingLeadId > 0)
            {
                skipLeadIdsAndManualLeadId = skipLeadIdsAndManualLeadId.Append(manuallyUploadingLeadId);
            }
            var lead = await LeadStorage.FindOldestLeadForUpload(skipLeadIdsAndManualLeadId, cancellationToken);
            if (lead == null)
            {
                lastLeadId = -1;
                return false;
            }
            try
            {
                lastLeadId = lead.Id;
                currentUploadLeadId = lead.Id;
                currentUploadLeadTask = DoUploadSingleLead(lead, cancellationToken);
                return await currentUploadLeadTask;
            }
            finally
            {
                currentUploadLeadId = -1;
                currentUploadLeadTask = null;
            }
        }

        async Task<bool> DoManuallyUploadLeadById(int leadId, CancellationToken cancellationToken)
        {
            if (currentUploadLeadId == leadId && currentUploadLeadTask != null)
            {
                await currentUploadLeadTask;
            }
            var lead = await LeadStorage.LoadLead(leadId, cancellationToken);
            if (lead == null)
            {
                return false;
            }
            try
            {
                manuallyUploadingLeadId = lead.Id;
                return await DoUploadSingleLead(lead, cancellationToken);
            }
            finally
            {
                manuallyUploadingLeadId = -1;
            }
        }

        async Task<bool> DoUploadSingleLead(LeadRecord lead, CancellationToken cancellationToken)
        {
            LOG.Debug("Started uploading lead with id: {0}", lead.Id);
            try
            {
                var leadDTO = PrepareLeadDTOForServer(lead);

                await UploadLeadPhotoIfNeeded(lead, leadDTO, cancellationToken);
                await UploadLeadCardFrontIfNeeded(lead, leadDTO, cancellationToken);
                await UploadLeadCardBackIfNeeded(lead, leadDTO, cancellationToken);

                if (ShouldCreateLeadOnServer(lead))
                {
                    await CreateLeadOnServer(lead, leadDTO, cancellationToken);
                }
                else
                {
                    try
                    {
                        await UpdateLeadOnServer(lead, leadDTO, cancellationToken);
                    }
                    catch (ServerHasNewerLeadError error)
                    {
                        await MarkLeadHasOldData(lead, error.ServerUpdatedAt);
                        return true;
                    }
                }
                await DeleteLeadOnServerIfNeeded(lead, cancellationToken);
                await LeadStorage.ClearUploadRequiredAtIfUnchanged(lead.Id, lead.UploadRequiredAt, CancellationToken.None);
                CleanupAfterLeadDeletedOnServerIfNeeded(lead);
            }
            catch (MultipleException multipleError)
            {
                if (!IsLeadMarkedForDeletion(lead))
                {
                    throw;
                }
                var leadNotFoundError = multipleError.AllExceptions().FirstOrDefault(_ => _ is LeadNotFoundError) as LeadNotFoundError;
                var eventNotFoundError = multipleError.AllExceptions().FirstOrDefault(_ => _ is EventNotFoundError) as EventNotFoundError;
                if (leadNotFoundError == null && eventNotFoundError == null)
                    throw;
                else if (leadNotFoundError != null && eventNotFoundError == null)
                    LOG.Warn("Failed to delete lead on server with uid: {0}, already deleted", lead.UID);
                else
                    LOG.Warn("Failed to upload lead on server with uid: {0}, event does not exist", lead.UID);
                await LeadStorage.ClearUploadRequiredAtIfUnchanged(lead.Id, lead.UploadRequiredAt, CancellationToken.None);
                CleanupAfterLeadDeletedOnServerIfNeeded(lead);
                return true;
            }
            return true;
        }

        LeadDTO PrepareLeadDTOForServer(LeadRecord lead)
        {
            var leadDTO = JsonService.Deserialize<LeadDTO>(lead.LeadJson);
            if (leadDTO.UID == null)
                leadDTO.UID = lead.UID;
            leadDTO.ClientsideUpdatedAt = lead.OverwriteUpdatedAt;
            leadDTO.EnsureAllFieldsPresent();
            return leadDTO;
        }

        async Task<bool> UploadLeadPhotoIfNeeded(LeadRecord lead, LeadDTO leadDTO, CancellationToken cancellationToken)
        {
            if (lead.LocalPhotoPath == null || lead.RemotePhotoUrl != null) return false;

            var absolutePath = ServiceLocator.Instance.FileResourcesService.AbsolutePathForFile(lead.LocalPhotoPath);
            var fileName = Path.GetFileName(lead.LocalPhotoPath);
            var tokenResult = await ApiService.CreateUploadTokenUri(fileName, cancellationToken);
            var uploadedUri = await ApiService.UploadFile(tokenResult.Content, absolutePath, cancellationToken);
            LOG.Debug("Uploaded photo for lead uid: {0} to uri: {1}", lead.UID, uploadedUri);

            ImageService.Instance.LoadFile(absolutePath).CacheKey(uploadedUri).Preload();

            lead.RemotePhotoUrl = uploadedUri;
            leadDTO.PhotoUrl = uploadedUri;
            var updated = await LeadStorage.UpdatePhotoUrlIfLocalPathUnchanged(lead.Id, uploadedUri, lead.LocalPhotoPath, CancellationToken.None);
            LOG.Debug("Saved ({0} rows) photo url for lead uid: {1} to uri: {2}", updated, lead.UID, uploadedUri);
            return true;
        }

        async Task<bool> UploadLeadCardFrontIfNeeded(LeadRecord lead, LeadDTO leadDTO, CancellationToken cancellationToken)
        {
            if (lead.LocalCardFrontPath == null || lead.RemoteCardFrontUrl != null) return false;

            var absolutePath = ServiceLocator.Instance.FileResourcesService.AbsolutePathForFile(lead.LocalCardFrontPath);
            var fileName = Path.GetFileName(lead.LocalCardFrontPath);
            var tokenResult = await ApiService.CreateUploadTokenUri(fileName, cancellationToken);
            var uploadedUri = await ApiService.UploadFile(tokenResult.Content, absolutePath, cancellationToken);
            LOG.Debug("Uploaded card front for lead uid: {0} to uri: {1}", lead.UID, uploadedUri);

            ImageService.Instance.LoadFile(absolutePath).CacheKey(uploadedUri).Preload();

            lead.RemoteCardFrontUrl = uploadedUri;
            leadDTO.BusinessCardFrontUrl = uploadedUri;
            var updated = await LeadStorage.UpdateCardFrontUrlIfLocalPathUnchanged(lead.Id, uploadedUri, lead.LocalCardFrontPath, CancellationToken.None);
            LOG.Debug("Saved ({0} rows) card front url for lead uid: {1} to uri: {2}", updated, lead.UID, uploadedUri);
            return true;
        }

        async Task<bool> UploadLeadCardBackIfNeeded(LeadRecord lead, LeadDTO leadDTO, CancellationToken cancellationToken)
        {
            if (lead.LocalCardBackPath == null || lead.RemoteCardBackUrl != null) return false;

            var absolutePath = ServiceLocator.Instance.FileResourcesService.AbsolutePathForFile(lead.LocalCardBackPath);
            var fileName = Path.GetFileName(lead.LocalCardBackPath);
            var tokenResult = await ApiService.CreateUploadTokenUri(fileName, cancellationToken);
            var uploadedUri = await ApiService.UploadFile(tokenResult.Content, absolutePath, cancellationToken);
            LOG.Debug("Uploaded card back for lead uid: {0} to uri: {1}", lead.UID, uploadedUri);

            ImageService.Instance.LoadFile(absolutePath).CacheKey(uploadedUri).Preload();

            lead.RemoteCardBackUrl = uploadedUri;
            leadDTO.BusinessCardBackUrl = uploadedUri;
            var updated = await LeadStorage.UpdateCardBackUrlIfLocalPathUnchanged(lead.Id, uploadedUri, lead.LocalCardBackPath, CancellationToken.None);
            LOG.Debug("Saved ({0} rows) card back url for lead uid: {1} to uri: {2}", updated, lead.UID, uploadedUri);
            return true;
        }

        bool ShouldCreateLeadOnServer(LeadRecord lead)
        {
            return lead.UID.IsNullOrEmpty();
        }

        async Task CreateLeadOnServer(LeadRecord lead, LeadDTO leadDTO, CancellationToken cancellationToken)
        {
            var result = await ApiService.CreateLead(leadDTO, cancellationToken);
            var leadResponseDTO = result.Content;
            var newUid = leadResponseDTO.UID;
            LOG.Debug("Created lead on server with uid: {0}", newUid);

            lead.UID = newUid;
            var uidData = new Dictionary<string, object> { { LeadRecord.LeadsColumnUIDName, newUid } };
            await LeadStorage.UpdateLeadData(lead.Id, uidData, CancellationToken.None);
        }

        async Task UpdateLeadOnServer(LeadRecord lead, LeadDTO leadDTO, CancellationToken cancellationToken)
        {
            try
            {
                await ApiService.UpdateLead(lead.UID, leadDTO, cancellationToken);
                LOG.Debug("Updated lead on server with uid: {0}", lead.UID);
            }
            catch (MultipleException multipleError)
            {
                var oldLeadError = multipleError.AllExceptions().FirstOrDefault(_ => _ is ServerHasNewerLeadError) as ServerHasNewerLeadError;
                if (oldLeadError != null)
                {
                    throw oldLeadError;
                }
                throw;
            }
        }

        async Task MarkLeadHasOldData(LeadRecord lead, DateTime? serverUpdatedAt)
        {
            lead.ServerUpdatedAt = serverUpdatedAt;
            var serverUpdatedAtData = new Dictionary<string, object> { { LeadRecord.LeadsColumnServerUpdatedAtName, serverUpdatedAt } };
            await LeadStorage.UpdateLeadData(lead.Id, serverUpdatedAtData, CancellationToken.None);
            LOG.Warn("ServerHasNewerLeadError: {0} for uid: {1}", serverUpdatedAt, lead.UID);
        }

        bool IsLeadMarkedForDeletion(LeadRecord lead)
        {
            return lead.DeletedAt != null;
        }

        async Task DeleteLeadOnServerIfNeeded(LeadRecord lead, CancellationToken cancellationToken)
        {
            if (!IsLeadMarkedForDeletion(lead))
            {
                return;
            }
            await ApiService.DeleteLead(lead.UID, cancellationToken);
            LOG.Debug("Deleted lead on server with uid: {0}", lead.UID);
        }

        void CleanupAfterLeadDeletedOnServerIfNeeded(LeadRecord lead)
        {
            if (lead.LocalPhotoPath != null)
            {
                FileResourcesService.DeleteFileNoThrow(lead.LocalPhotoPath);
                lead.LocalPhotoPath = null;
            }
            if (lead.LocalCardFrontPath != null)
            {
                FileResourcesService.DeleteFileNoThrow(lead.LocalCardFrontPath);
                lead.LocalCardFrontPath = null;
            }
            if (lead.LocalCardBackPath != null)
            {
                FileResourcesService.DeleteFileNoThrow(lead.LocalCardBackPath);
                lead.LocalCardBackPath = null;
            }
        }

        async Task ExportLeadsToCRMIfNeeded(CancellationToken cancellationToken)
        {
            var exportRequestRecords = await LeadStorage.LoadExportRequests(cancellationToken);
            if (exportRequestRecords.IsNullOrEmpty()) return;
            var exportLeadsIds = exportRequestRecords.Select((arg) => arg.LeadId).ToList();
            var exportLeads = await LeadStorage.LoadLeads(exportLeadsIds, cancellationToken);
            var exportUIDs = exportLeads.Select((arg) => arg.UID).ToList();
            if (exportUIDs.IsNullOrEmpty()) return;
            var exportReport = await ApiService.ExportLeadsToCRM(exportUIDs, cancellationToken);
            LOG.Debug("Exported to CRM leads on server count: {0}", exportUIDs.Count);
            await LeadStorage.DropExportRequests(exportLeadsIds, cancellationToken);

            var notificationData = new Dictionary<string, IEnumerable>();
            notificationData.Add(LeadsExportReportDTO.EXPORT_CREATED_LIST_NAME, exportReport.Content.CreatedLeads);
            notificationData.Add(LeadsExportReportDTO.EXPORT_UPDATED_LIST_NAME, exportReport.Content.UpdatedLeads);
            notificationData.Add(LeadsExportReportDTO.EXPORT_FAILED_LIST_NAME, exportReport.Content.FailedLeads);
            MessagingCenter.Post(LeadsExportReportDTO.SHOW_EXPORT_REPORT_EVENT_NAME, notificationData);
        }
    }
}
