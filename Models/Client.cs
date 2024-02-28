namespace BackendJPMAnalysis.Models
{
    public partial class Client : BaseModel
    {
        /// <summary>
        /// Auto-incremental ID
        /// </summary>
        public int Id { get; }
        public string? ProductId { get; set; }
        public string? AccountNumber { get; set; }

        public virtual Account? AccountNumberNavigation { get; set; }
        public virtual Product? Product { get; set; }
    }
}
