namespace BackendJPMAnalysis.Models
{
    public abstract class BaseEntity
    {
        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; }
        public DateTime? DeletedAt { get; set; }
    }
}