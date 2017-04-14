using System;
using System.Collections.Generic;

namespace Avend.API.Model
{
    public enum LeadExportResultStatus
    {
        Created,
        Updated,
        Failed,
    }

    public class LeadExportResult
    {
        public LeadExportResultStatus Status { get; set; }
        public DateTime ProcessedAt { get; set; }

        public Guid LeadUid { get; set; }
        public LeadRecord LeadRecord { get; set; }

        public string Message { get; set; } = "";
        public bool IsRecoverable { get; set; } = false;
    }
}