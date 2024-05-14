using System.Text.Json.Serialization;


namespace BackendJPMAnalysis.Models
{
    public partial class ReportHistoryModel
    {
        public string Id { get; set; } = null!;
        public string? AppUserId { get; set; }
        public string ReportName { get; set; } = null!;
        public string? ReportComments { get; set; }
        public DateTime? RunReportDate { get; set; }
        public DateTime? ReportUploadDate { get; set; }

        [JsonIgnore]
        public virtual AppUserModel? AppUser { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, AppUserId: {AppUserId}, ReportName: {ReportName}, RunReportDate: {RunReportDate}, ReportComments: '{ReportComments}', ReportUploadDate: {ReportUploadDate}";
        }
    }

    public enum QueryOption { load, compare }
}
