namespace BackendJPMAnalysis.Models
{
    public abstract class BaseModel
    {
        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; }
        public DateTime? DeletedAt { get; set; }
    }
}