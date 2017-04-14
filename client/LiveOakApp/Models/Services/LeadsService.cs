using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SL4N;
using StudioMobile;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Models.Data.Records;
using LiveOakApp.Models.Data.Entities;
using System.Net;

namespace LiveOakApp.Models.Services
{
    public class LeadsService
    {
        static readonly ILogger LOG = LoggerFactory.GetLogger<LeadsService>();

        const int PAGE_SIZE = 50;

        public CachableRequest<List<LeadDTO>> LeadsRequest { get; private set; }

        readonly LeadStorage LeadStorage;
        readonly JsonService JsonService;

        Converter<LeadDTO, string> serializeLeadDTO;

        public LeadsService(CacheStorage CacheStorage, LeadStorage leadStorage, JsonService jsonService)
        {
            JsonService = jsonService;
            LeadStorage = leadStorage;
            LeadsRequest = new CachableRequest<List<LeadDTO>>(
                CacheStorage,
                "leads",
                (eTag, token) => ServiceLocator.Instance.ApiService.GetLeads(0, PAGE_SIZE, eTag, token),
                () => CleanupUploadedLeads().Ignore()
            );

            serializeLeadDTO = _ =>
            {
                Stopwatch jsonWatch = Stopwatch.StartNew();
                var leadJson = JsonService.Serialize(_);
                jsonWatch.Stop();
                LOG.Trace("serialized leadDTO in {0} ms", jsonWatch.ElapsedMilliseconds);
                return leadJson;
            };
        }

        public async Task<List<Lead>> GetAllLeads(CancellationToken? cancellationToken)
        {
            var allLeads = new List<Lead>();

            await LeadsRequest.LoadFromCache();
            var networkLeads = LeadsRequest.Result?.ToList() ?? new List<LeadDTO>();
            var storageLeads = await LeadStorage.LoadLeads(true, cancellationToken);
            var recordsByUid = storageLeads.Where(_ => _.UID != null).GroupBy(_ => _.UID).ToDictionary(g => g.Key, g => g.First());
            var deletedLeadsUids = await LeadStorage.FindUidsOfDeletedLeads(cancellationToken);
            var deletedByUid = deletedLeadsUids.ToDictionary(u => u);

            foreach (var lead in networkLeads)
            {
                var uid = lead.UID;
                if (recordsByUid.ContainsKey(uid))
                {
                    var record = recordsByUid[uid];
                    recordsByUid.Remove(uid);
                    allLeads.Add(new Lead(record));
                }
                else if (!deletedByUid.ContainsKey(uid))
                {
                    allLeads.Add(new Lead(lead));
                }
            }
            var missingFromServerLeads = recordsByUid.Values.ToList().ConvertAll(record => new Lead(record));
            allLeads.AddRange(missingFromServerLeads);
            var localLeads = storageLeads.Where(_ => _.UID == null).ToList().ConvertAll(record => new Lead(record));
            allLeads.AddRange(localLeads);
            LOG.Debug("GetAllLeads: network: {0}, storage: {1}, deleted: {2}", networkLeads.Count, storageLeads.Count, deletedLeadsUids.Count);
            return allLeads;
        }

        public async Task<int> SaveLead(int leadId, LeadDTO lead, FileResource photo, FileResource cardFront, FileResource cardBack, CancellationToken? cancellationToken)
        {
            var result = await DoSaveLead(leadId, lead, photo, cardFront, cardBack, cancellationToken, false);
            ServiceLocator.Instance.LeadsUploadService.StartUploadLeadsContinuouslyIfNeeded();
            return result;
        }

        LeadRecord CreateLeadRecord(LeadDTO lead, FileResource photo, FileResource cardFront, FileResource cardBack)
        {
            var leadJson = serializeLeadDTO(lead);
            var record = new LeadRecord();
            record.UID = lead.UID;
            record.LeadJson = leadJson;
            record.LocalPhotoPath = photo.RelativeLocalPath;
            record.RemotePhotoUrl = photo.RemoteUrl;
            record.LocalCardFrontPath = cardFront.RelativeLocalPath;
            record.RemoteCardFrontUrl = cardFront.RemoteUrl;
            record.LocalCardBackPath = cardBack.RelativeLocalPath;
            record.RemoteCardBackUrl = cardBack.RemoteUrl;
            return record;
        }

        public async Task<int> CreateLeadLocallyIfNeeded(int leadId, LeadDTO lead, FileResource photo, FileResource cardFront, FileResource cardBack, CancellationToken? cancellationToken)
        {
            var record = await FindLead(leadId, lead.UID, cancellationToken);
            if (record == null)
                return await DoCreateLeadLocally(lead, photo, cardFront, cardBack, cancellationToken);
            else
                return record.Id;
        }

        async Task<int> DoCreateLeadLocally(LeadDTO lead, FileResource photo, FileResource cardFront, FileResource cardBack, CancellationToken? cancellationToken)
        {
            var record = CreateLeadRecord(lead, photo, cardFront, cardBack);
            return await LeadStorage.CreateLead(record, cancellationToken);
        }

        async Task<int> DoSaveLead(int leadId, LeadDTO lead, FileResource photo, FileResource cardFront, FileResource cardBack, CancellationToken? cancellationToken, bool markAsDeleted)
        {
            var record = await FindLead(leadId, lead.UID, cancellationToken);
            if (record == null)
            {
                record = CreateLeadRecord(lead, photo, cardFront, cardBack);
                if (markAsDeleted)
                {
                    record.DeletedAt = DateTime.Now;
                }
                record.UploadRequiredAt = DateTime.Now;
                record.OverwriteUpdatedAt = DateTime.Now;
                return await LeadStorage.CreateLead(record, cancellationToken);
            }
            else
            {
                var dataToSave = new Dictionary<string, object>();
                if (ShouldWriteResource(photo, record.LocalPhotoPath, record.RemotePhotoUrl))
                {
                    dataToSave[LeadRecord.LeadsColumnLocalPhotoPathName] = photo.RelativeLocalPath;
                    dataToSave[LeadRecord.LeadsColumnRemotePhotoUrlName] = photo.RemoteUrl;
                }
                if (ShouldWriteResource(cardFront, record.LocalCardFrontPath, record.RemoteCardFrontUrl))
                {
                    dataToSave[LeadRecord.LeadsColumnLocalCardFrontPathName] = cardFront.RelativeLocalPath;
                    dataToSave[LeadRecord.LeadsColumnRemoteCardFrontUrlName] = cardFront.RemoteUrl;
                }
                if (ShouldWriteResource(cardBack, record.LocalCardBackPath, record.RemoteCardBackUrl))
                {
                    dataToSave[LeadRecord.LeadsColumnLocalCardBackPathName] = cardBack.RelativeLocalPath;
                    dataToSave[LeadRecord.LeadsColumnRemoteCardBackUrlName] = cardBack.RemoteUrl;
                }
                var leadJson = serializeLeadDTO(lead);
                dataToSave[LeadRecord.LeadsColumnLeadJsonName] = leadJson;
                if (markAsDeleted)
                {
                    dataToSave[LeadRecord.LeadsColumnDeletedAtName] = record.DeletedAt ?? DateTime.Now;
                }
                dataToSave[LeadRecord.LeadsColumnUploadRequiredAtName] = DateTime.Now;
                dataToSave[LeadRecord.LeadsColumnOverwriteUpdatedAtName] = DateTime.Now;
                dataToSave[LeadRecord.LeadsColumnExportRequiredAtName] = null;
                await LeadStorage.UpdateLeadData(record.Id, dataToSave, cancellationToken);
                await LeadStorage.DropExportRequests(new List<int> { record.Id }, cancellationToken);
                return record.Id;
            }
        }

        public async Task<int> UpdateLeadLocally(int leadId, LeadDTO lead, FileResource photo, FileResource cardFront, FileResource cardBack, CancellationToken? cancellationToken)
        {
            var record = await FindLead(leadId, lead.UID, cancellationToken);
            if (record == null)
            {
                record = CreateLeadRecord(lead, photo, cardFront, cardBack);
                record.OverwriteUpdatedAt = DateTime.Now;
                return await LeadStorage.CreateLead(record, cancellationToken);
            }
            else
            {
                var dataToSave = new Dictionary<string, object>();
                if (ShouldWriteResource(photo, record.LocalPhotoPath, record.RemotePhotoUrl))
                {
                    dataToSave[LeadRecord.LeadsColumnLocalPhotoPathName] = photo.RelativeLocalPath;
                    dataToSave[LeadRecord.LeadsColumnRemotePhotoUrlName] = photo.RemoteUrl;
                }
                if (ShouldWriteResource(cardFront, record.LocalCardFrontPath, record.RemoteCardFrontUrl))
                {
                    dataToSave[LeadRecord.LeadsColumnLocalCardFrontPathName] = cardFront.RelativeLocalPath;
                    dataToSave[LeadRecord.LeadsColumnRemoteCardFrontUrlName] = cardFront.RemoteUrl;
                }
                if (ShouldWriteResource(cardBack, record.LocalCardBackPath, record.RemoteCardBackUrl))
                {
                    dataToSave[LeadRecord.LeadsColumnLocalCardBackPathName] = cardBack.RelativeLocalPath;
                    dataToSave[LeadRecord.LeadsColumnRemoteCardBackUrlName] = cardBack.RemoteUrl;
                }
                var leadJson = serializeLeadDTO(lead);
                dataToSave[LeadRecord.LeadsColumnLeadJsonName] = leadJson;
                dataToSave[LeadRecord.LeadsColumnOverwriteUpdatedAtName] = DateTime.Now;
                await LeadStorage.UpdateLeadData(record.Id, dataToSave, cancellationToken);
                return record.Id;
            }
        }

        bool ShouldWriteResource(FileResource newResource, string oldPath, string oldUrl)
        {
            var newPhotoPath = newResource.RelativeLocalPath;
            var newPhotoUrl = newResource.RemoteUrl;
            return ((newPhotoPath == null && newPhotoUrl == null)
                    || (newPhotoPath != null && newPhotoPath != oldPath)
                    || (newPhotoUrl != null && newPhotoUrl != oldUrl)
                   );
        }

        /// <summary>
        /// Exports lead to CRM
        /// </summary>
        /// <returns>True if exported. False if scheduled to export later.</returns>
        /// <param name="leadId">Lead identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task<LeadsExportReportDTO> ExportLeadToCRM(int leadId, CancellationToken? cancellationToken)
        {
            return await DoExportLeadToCRM(leadId, cancellationToken);
        }

        async Task<LeadsExportReportDTO> DoExportLeadToCRM(int leadId, CancellationToken? cancellationToken)
        {
            try
            {
                var uploaded = await ServiceLocator.Instance.LeadsUploadService.ManuallyUploadLeadById(leadId, cancellationToken ?? CancellationToken.None);
                if (!uploaded)
                {
                    throw new LeadNotFoundError(); // should never happen
                }
                var lead = await LeadStorage.LoadLead(leadId, cancellationToken);
                var exportResult = await ServiceLocator.Instance.ApiService.ExportLeadToCRM(lead.UID, cancellationToken);
                var exportReport = exportResult.Content;
                if (exportReport.TotalFailed > 0)
                {
                    throw new LeadCRMExportFailedError(exportReport.FailedLeads.First().Reason);
                }
                return exportReport;
            }
            catch (Exception ex)
            {
#if __ANDROID__
                // this exceptions only occurs on android.
                if (!(ex is Java.Net.UnknownHostException || ex is Javax.Net.Ssl.SSLException || ex is WebException))
                    throw;
#else
                if (!(ex is WebException))
                    throw;
#endif
                var dataToSave = new Dictionary<string, object>();
                dataToSave[LeadRecord.LeadsColumnUploadRequiredAtName] = DateTime.Now;
                dataToSave[LeadRecord.LeadsColumnExportRequiredAtName] = DateTime.Now;
                await LeadStorage.UpdateLeadData(leadId, dataToSave, cancellationToken);

                var lead = await LeadStorage.LoadLead(leadId, cancellationToken);

                var record = await LeadStorage.FindExportRequest(lead.Id, cancellationToken);
                if (record == null)
                    await LeadStorage.CreateExportRequest(new ExportRequestRecord() { LeadId = lead.Id }, cancellationToken);
                
                ServiceLocator.Instance.LeadsUploadService.StartUploadLeadsContinuouslyIfNeeded();
            }
            return null;
        }

        public async Task<int> MarkLeadForOverwrite(int leadId, CancellationToken? cancellationToken)
        {
            var result = await DoMarkLeadForOverwrite(leadId, cancellationToken);
            ServiceLocator.Instance.LeadsUploadService.StartUploadLeadsContinuouslyIfNeeded();
            return result;
        }

        async Task<int> DoMarkLeadForOverwrite(int leadId, CancellationToken? cancellationToken)
        {
            var record = await FindLead(leadId, null, cancellationToken);
            var dataToSave = new Dictionary<string, object>();
            dataToSave[LeadRecord.LeadsColumnUploadRequiredAtName] = DateTime.Now;
            dataToSave[LeadRecord.LeadsColumnOverwriteUpdatedAtName] = DateTime.Now;
            await LeadStorage.UpdateLeadData(record.Id, dataToSave, cancellationToken);
            return record.Id;
        }

        public async Task<int> DeleteLead(int leadId, LeadDTO lead, FileResource photo, FileResource cardFront, FileResource cardBack, CancellationToken? cancellationToken)
        {
            var result = await DoSaveLead(leadId, lead, photo, cardFront, cardBack, cancellationToken, true);
            ServiceLocator.Instance.LeadsUploadService.StartUploadLeadsContinuouslyIfNeeded();
            return result;
        }

        async Task<LeadRecord> FindLead(int leadId, string UID, CancellationToken? cancellationToken)
        {
            return await LeadStorage.LoadLead(leadId, cancellationToken)
                                    ?? await LeadStorage.FindLead(UID, cancellationToken);
        }

        async Task CleanupUploadedLeads()
        {
            var deleted = await LeadStorage.DropAllUploadedLeads(null);
            LOG.Debug("Dropped {0} uploaded leads", deleted);
        }

        public async Task CleanupOldOrphanedFiles(CancellationToken? cancellationToken = null)
        {
            var photos = await LeadStorage.FindAllPhotoPathsOfLeads(cancellationToken);
            var cardFronts = await LeadStorage.FindAllCardFrontPathsOfLeads(cancellationToken);
            var cardBacks = await LeadStorage.FindAllCardBackPathsOfLeads(cancellationToken);
            var preservePaths = photos.Concat(cardFronts).Concat(cardBacks);
            ServiceLocator.Instance.FileResourcesService.DeleteOldUnusedFiles(preservePaths);
        }

        public async Task<Lead> FindOldestLeadForOverwriteDecision(CancellationToken? cancellationToken)
        {
            var record = await LeadStorage.FindOldestLeadForOverwriteDecision(cancellationToken);
            if (record == null) return null;
            return new Lead(record);
        }

        public Task DiscardChangesForLead(int leadId, CancellationToken? cancellationToken)
        {
            return LeadStorage.DropLead(leadId, cancellationToken);
        }
    }
}
