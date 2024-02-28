using System.Text.Json.Serialization;


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

        [JsonIgnore]
        public virtual AppUser? AppUser { get; set; }
    }
}
