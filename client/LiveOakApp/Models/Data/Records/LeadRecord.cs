using System;
using SQLite;

namespace LiveOakApp.Models.Data.Records
{
    [Table(LeadsTableName)]
    public class LeadRecord
    {
        public const string LeadsTableName = "leads";

        public const string LeadsColumnIDName = "id";
        public const string LeadsColumnUIDName = "lead_uid";
        public const string LeadsColumnLeadJsonName = "lead_json";
        public const string LeadsColumnLocalPhotoPathName = "local_photo_path";
        public const string LeadsColumnRemotePhotoUrlName = "remote_photo_url";
        public const string LeadsColumnLocalCardFrontPathName = "local_card_front_path";
        public const string LeadsColumnRemoteCardFrontUrlName = "remote_card_front_url";
        public const string LeadsColumnLocalCardBackPathName = "local_card_back_path";
        public const string LeadsColumnRemoteCardBackUrlName = "remote_card_back_url";
        public const string LeadsColumnServerUpdatedAtName = "server_updated_at";
        public const string LeadsColumnOverwriteUpdatedAtName = "overwrite_updated_at";
        public const string LeadsColumnDeletedAtName = "deleted_at";
        public const string LeadsColumnUploadRequiredAtName = "upload_required_at";
        public const string LeadsColumnExportRequiredAtName = "export_required_at";

        [Column(LeadsColumnIDName), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column(LeadsColumnUIDName), Unique]
        public string UID { get; set; }

        [Column(LeadsColumnLeadJsonName)]
        public string LeadJson { get; set; }

        [Column(LeadsColumnLocalPhotoPathName)]
        public string LocalPhotoPath { get; set; }

        [Column(LeadsColumnRemotePhotoUrlName)]
        public string RemotePhotoUrl { get; set; }

        [Column(LeadsColumnLocalCardFrontPathName)]
        public string LocalCardFrontPath { get; set; }

        [Column(LeadsColumnRemoteCardFrontUrlName)]
        public string RemoteCardFrontUrl { get; set; }

        [Column(LeadsColumnLocalCardBackPathName)]
        public string LocalCardBackPath { get; set; }

        [Column(LeadsColumnRemoteCardBackUrlName)]
        public string RemoteCardBackUrl { get; set; }

        [Column(LeadsColumnServerUpdatedAtName)]
        public DateTime? ServerUpdatedAt { get; set; }

        [Column(LeadsColumnOverwriteUpdatedAtName)]
        public DateTime? OverwriteUpdatedAt { get; set; }

        [Column(LeadsColumnDeletedAtName)]
        public DateTime? DeletedAt { get; set; }

        [Column(LeadsColumnUploadRequiredAtName)]
        public DateTime? UploadRequiredAt { get; set; }

        [Column(LeadsColumnExportRequiredAtName)]
        public DateTime? ExportRequiredAt { get; set; }
    }
}
