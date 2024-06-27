namespace BackendJPMAnalysis.DTO
{
    public class ApproveChangesDTO
    {
        public EntitiesToChangesDTO EntitiesToChange { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public DateTime? RunReportDate { get; set; }
        public string? Observations { get; set; }
        public string AppUserId { get; set; } = null!;
    }
}