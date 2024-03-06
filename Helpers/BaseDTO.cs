namespace BackendJPMAnalysis.Helpers
{
    public abstract class BaseDTO
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}