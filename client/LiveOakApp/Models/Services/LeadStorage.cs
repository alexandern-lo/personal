using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using ServiceStack;
using SL4N;
using LiveOakApp.Models.Data.Records;

namespace LiveOakApp.Models.Services
{
    public class LeadStorage
    {
        static readonly ILogger LOG = LoggerFactory.GetLogger<LeadStorage>();

        readonly UserSQLiteDatabase database;

        bool DatabaseCreated { get; set; }

        public LeadStorage(UserSQLiteDatabase userSQLiteDatabase)
        {
            database = userSQLiteDatabase;
            userSQLiteDatabase.OnCanGetConnectionChanged += (sender, e) => CreateDatabaseIfNeccesarry();
            CreateDatabaseIfNeccesarry();
        }

        public async Task<int> CreateLead(LeadRecord record, CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            await conn.InsertAsync(record);
            LOG.Debug("CreateLead: {0}", record.Id);
            return record.Id;
        }

        public async Task<int> UpdateLeadData(int leadId, Dictionary<string, object> data, CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            var query = string.Format("UPDATE {0} SET", LeadRecord.LeadsTableName);
            var keys = data.Keys;
            query += keys.Select(key => string.Format(" {0} = ?", key)).Join(",");
            var values = keys.Select(_ => data[_]);
            query += string.Format(" WHERE {0} = ?", LeadRecord.LeadsColumnIDName);
            values = values.Append(leadId);
            var result = await conn.ExecuteAsync(query, values.ToArray());
            LOG.Debug("UpdateLeadData {0}: {1}", leadId, result);
            return result;
        }

        public async Task<int> ClearUploadRequiredAtIfUnchanged(int leadId, DateTime? UploadRequiredAt, CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            var query = string.Format("UPDATE {0} SET", LeadRecord.LeadsTableName);
            var values = new List<object>().AsEnumerable();
            query += string.Format(" {0} = ?", LeadRecord.LeadsColumnUploadRequiredAtName);
            values = values.Append(null);
            query += string.Format(" WHERE {0} = ?", LeadRecord.LeadsColumnIDName);
            values = values.Append(leadId);
            query += string.Format(" AND {0} = ?", LeadRecord.LeadsColumnUploadRequiredAtName);
            values = values.Append(UploadRequiredAt);
            var result = await conn.ExecuteAsync(query, values.ToArray());
            LOG.Debug("ClearUploadRequiredAtIfUnchanged {0}: {1}", leadId, result);
            return result;
        }

        public async Task<int> UpdatePhotoUrlIfLocalPathUnchanged(int leadId, string uploadedUrl, string localPhotoPath, CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            var query = string.Format("UPDATE {0} SET", LeadRecord.LeadsTableName);
            var values = new List<object>().AsEnumerable();
            query += string.Format(" {0} = ?", LeadRecord.LeadsColumnRemotePhotoUrlName);
            values = values.Append(uploadedUrl);
            query += string.Format(" WHERE {0} = ?", LeadRecord.LeadsColumnIDName);
            values = values.Append(leadId);
            query += string.Format(" AND {0} = ?", LeadRecord.LeadsColumnLocalPhotoPathName);
            values = values.Append(localPhotoPath);
            var result = await conn.ExecuteAsync(query, values.ToArray());
            LOG.Debug("UpdatePhotoUrlIfLocalPathUnchanged {0}: {1}", leadId, result);
            return result;
        }

        public async Task<int> UpdateCardFrontUrlIfLocalPathUnchanged(int leadId, string uploadedUrl, string localPath, CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            var query = string.Format("UPDATE {0} SET", LeadRecord.LeadsTableName);
            var values = new List<object>().AsEnumerable();
            query += string.Format(" {0} = ?", LeadRecord.LeadsColumnRemoteCardFrontUrlName);
            values = values.Append(uploadedUrl);
            query += string.Format(" WHERE {0} = ?", LeadRecord.LeadsColumnIDName);
            values = values.Append(leadId);
            query += string.Format(" AND {0} = ?", LeadRecord.LeadsColumnLocalCardFrontPathName);
            values = values.Append(localPath);
            var result = await conn.ExecuteAsync(query, values.ToArray());
            LOG.Debug("UpdateCardFrontUrlIfLocalPathUnchanged {0}: {1}", leadId, result);
            return result;
        }

        public async Task<int> UpdateCardBackUrlIfLocalPathUnchanged(int leadId, string uploadedUrl, string localPath, CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            var query = string.Format("UPDATE {0} SET", LeadRecord.LeadsTableName);
            var values = new List<object>().AsEnumerable();
            query += string.Format(" {0} = ?", LeadRecord.LeadsColumnRemoteCardBackUrlName);
            values = values.Append(uploadedUrl);
            query += string.Format(" WHERE {0} = ?", LeadRecord.LeadsColumnIDName);
            values = values.Append(leadId);
            query += string.Format(" AND {0} = ?", LeadRecord.LeadsColumnLocalCardBackPathName);
            values = values.Append(localPath);
            var result = await conn.ExecuteAsync(query, values.ToArray());
            LOG.Debug("UpdateCardFrontUrlIfLocalPathUnchanged {0}: {1}", leadId, result);
            return result;
        }

        public async Task<LeadRecord> LoadLead(int leadId, CancellationToken? cancellationToken)
        {
            if (leadId <= 0) return null;
            var conn = database.GetConnection();
            return await conn.FindAsync<LeadRecord>(leadId);
        }

        public async Task<LeadRecord> FindLead(string UID, CancellationToken? cancellationToken)
        {
            if (UID == null) return null;
            var conn = database.GetConnection();
            return await conn.FindAsync<LeadRecord>(_ => _.UID == UID);
        }

        public async Task<LeadRecord> FindOldestLeadForUpload(IEnumerable<int> skipLeadIds, CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            return await conn.Table<LeadRecord>()
                             .Where(_ => _.UploadRequiredAt != null)
                             .Where(_ => _.ServerUpdatedAt <= _.OverwriteUpdatedAt 
                                    || _.ServerUpdatedAt == null
                                    || (_.OverwriteUpdatedAt == null && _.ServerUpdatedAt == null)) // TODO: remove this compat part later
                             .Where(_ => !skipLeadIds.Contains(_.Id))
                             .OrderBy(_ => _.UploadRequiredAt)
                             .FirstOrDefaultAsync();
        }

        public async Task<LeadRecord> FindOldestLeadForOverwriteDecision(CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            return await conn.Table<LeadRecord>()
                             .Where(_ => _.UploadRequiredAt != null)
                             .Where(_ => _.ServerUpdatedAt > _.OverwriteUpdatedAt || (_.ServerUpdatedAt != null && _.OverwriteUpdatedAt == null))
                             .OrderBy(_ => _.UploadRequiredAt)
                             .FirstOrDefaultAsync();
        }

        public async Task<List<LeadRecord>> LoadLeads(bool onlyNotDeleted, CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            var query = conn.Table<LeadRecord>();
            if (onlyNotDeleted)
            {
                query = query.Where(_ => _.DeletedAt == null);
            }
            return await query.ToListAsync();
        }

        public async Task<List<LeadRecord>> LoadLeads(List<int> leadIds, CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            var query = conn.Table<LeadRecord>();
            query = query.Where(_ => leadIds.Contains(_.Id));
            return await query.ToListAsync();
        }

        public async Task<List<string>> FindUidsOfDeletedLeads(CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            var query = string.Format("SELECT {0} FROM {1} WHERE {2} IS NOT NULL AND {0} IS NOT NULL",
                                      LeadRecord.LeadsColumnUIDName,
                                      LeadRecord.LeadsTableName,
                                      LeadRecord.LeadsColumnDeletedAtName);
            var result = await conn.QueryAsync<LeadRecord>(query);
            return result.Select(_ => _.UID).ToList();
        }

        public async Task<List<string>> FindAllPhotoPathsOfLeads(CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            var query = string.Format("SELECT {0} FROM {1} WHERE {0} IS NOT NULL",
                                      LeadRecord.LeadsColumnLocalPhotoPathName,
                                      LeadRecord.LeadsTableName);
            var result = await conn.QueryAsync<LeadRecord>(query);
            return result.Select(_ => _.LocalPhotoPath).ToList();
        }

        public async Task<List<string>> FindAllCardFrontPathsOfLeads(CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            var query = string.Format("SELECT {0} FROM {1} WHERE {0} IS NOT NULL",
                                      LeadRecord.LeadsColumnLocalCardFrontPathName,
                                      LeadRecord.LeadsTableName);
            var result = await conn.QueryAsync<LeadRecord>(query);
            return result.Select(_ => _.LocalCardFrontPath).ToList();
        }

        public async Task<List<string>> FindAllCardBackPathsOfLeads(CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            var query = string.Format("SELECT {0} FROM {1} WHERE {0} IS NOT NULL",
                                      LeadRecord.LeadsColumnLocalCardBackPathName,
                                      LeadRecord.LeadsTableName);
            var result = await conn.QueryAsync<LeadRecord>(query);
            return result.Select(_ => _.LocalCardBackPath).ToList();
        }

        public async Task<int> DropAllUploadedLeads(CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            int deleted = -1;
            await conn.RunInTransactionAsync((syncConn) =>
            {
                deleted = syncConn.Table<LeadRecord>()
                                  .Delete(_ => _.UploadRequiredAt == null);
            });
            LOG.Debug("DropAllUploadedLeads: {0}", deleted);
            return deleted;
        }

        public Task DropLead(int leadId, CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            var query = string.Format("DELETE FROM {0} WHERE {1} = ?",
                                      LeadRecord.LeadsTableName,
                                      LeadRecord.LeadsColumnIDName);
            return conn.ExecuteAsync(query, leadId);
        }

        #region

        public async Task<List<ExportRequestRecord>> LoadExportRequests(CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            var query = conn.Table<ExportRequestRecord>();
            return await query.ToListAsync();
        }

        public async Task<ExportRequestRecord> FindExportRequest(int leadId, CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            return await conn.FindAsync<ExportRequestRecord>(_ => _.LeadId == leadId);
        }

        public async Task<int> CreateExportRequest(ExportRequestRecord record, CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            await conn.InsertAsync(record);
            LOG.Debug("CreatedExportRequest: {0}", record.LeadId);
            return record.LeadId;
        }

        public async Task<int> DropExportRequests(List<int> leadIds, CancellationToken? cancellationToken)
        {
            var conn = database.GetConnection();
            int deleted = -1;
            await conn.RunInTransactionAsync((syncConn) =>
            {
                deleted = syncConn.Table<ExportRequestRecord>()
                                  .Delete(_ => leadIds.Contains(_.LeadId));
            });
            LOG.Debug("DropExportRequests: {0}", deleted);
            return deleted;
        }

        #endregion

        void CreateDatabaseIfNeccesarry()
        {
            if (!database.CanGetConnection())
            {
                DatabaseCreated = false;
                return;
            }
            if (DatabaseCreated) return;
            DatabaseCreated = true;
            var conn = database.GetConnection();
            conn.RunInTransactionAsync((db) =>
            {
                db.CreateTable<LeadRecord>();
                db.CreateTable<ExportRequestRecord>();
            }).Wait();
        }
    }
}
