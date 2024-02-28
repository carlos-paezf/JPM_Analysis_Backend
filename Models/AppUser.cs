using System.Text.Json.Serialization;


namespace BackendJPMAnalysis.Models
{
    public partial class AppUser
    {
        public AppUser()
        {
            AppHistories = new HashSet<AppHistory>();
            ReportHistories = new HashSet<ReportHistory>();
        }

        /// <summary>
        /// Autoincremental
        /// </summary>
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? AppRole { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        [JsonIgnore]
        public virtual ICollection<AppHistory> AppHistories { get; set; }
        [JsonIgnore]
        public virtual ICollection<ReportHistory> ReportHistories { get; set; }
    }
}
