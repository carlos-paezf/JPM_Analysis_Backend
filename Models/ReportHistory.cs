using System;
using System.Collections.Generic;

namespace BackendJPMAnalysis.Models
{
    public partial class ReportHistory
    {
        /// <summary>
        /// Autoincremental
        /// </summary>
        public int Id { get; set; }
        public int? AppUserId { get; set; }
        public string? ReportName { get; set; }
        public DateTime? ReportUploadDate { get; set; }

        public virtual AppUser? AppUser { get; set; }
    }
}
