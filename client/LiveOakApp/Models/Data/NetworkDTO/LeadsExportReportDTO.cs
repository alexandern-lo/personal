﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    /// <summary>
    /// Network data object for passing report on the export process.
    /// </summary>
    [DataContract(Name = "leads_export_report")]
    public class LeadsExportReportDTO
    {
        public const string SHOW_EXPORT_REPORT_EVENT_NAME = "show_export_report";
        public const string EXPORT_CREATED_LIST_NAME = "export_created";
        public const string EXPORT_UPDATED_LIST_NAME = "export_updated";
        public const string EXPORT_FAILED_LIST_NAME = "export_failed";

        public LeadsExportReportDTO()
        {
            CreatedLeads = new List<LeadExportSuccessDetails>();
            UpdatedLeads = new List<LeadExportSuccessDetails>();
            FailedLeads = new List<LeadExportFailureDetails>();
        }

        [DataMember(Name = "total_created")]
        public long TotalCreated { get; set; }

        [DataMember(Name = "total_updated")]
        public long TotalUpdated { get; set; }

        [DataMember(Name = "total_failed")]
        public long TotalFailed { get; set; }

        [DataMember(Name = "created_leads")]
        public List<LeadExportSuccessDetails> CreatedLeads { get; set; }

        [DataMember(Name = "updated_leads")]
        public List<LeadExportSuccessDetails> UpdatedLeads { get; set; }

        [DataMember(Name = "failed_leads")]
        public List<LeadExportFailureDetails> FailedLeads { get; set; }
    }

    [DataContract(Name = "leads_export_success_details")]
    public class LeadExportSuccessDetails
    {
        [DataMember(Name = "lead_uid")]
        public Guid LeadUid { get; set; }

        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }

        [DataMember(Name = "last_name")]
        public string LastName { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "exported_at")]
        public DateTime ExportedAt { get; set; }
    }

    [DataContract(Name = "leads_export_failure_details")]
    public class LeadExportFailureDetails
    {
        [DataMember(Name = "lead_uid")]
        public Guid LeadUid { get; set; }

        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }

        [DataMember(Name = "last_name")]
        public string LastName { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "failed_at")]
        public DateTime FailedAt { get; set; }

        [DataMember(Name = "reason")]
        public string Reason { get; set; }
    }
}
