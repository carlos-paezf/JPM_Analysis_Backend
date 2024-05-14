using System.Text.Json.Serialization;


namespace BackendJPMAnalysis.Models
{
    public partial class AppUserModel
    {
        public AppUserModel()
        {
            AppHistories = new HashSet<AppHistoryModel>();
            ReportHistories = new HashSet<ReportHistoryModel>();
        }

        /// <summary>
        /// Autoincremental
        /// </summary>
        public string Id { get; set; } = null!;
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? AppRole { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        [JsonIgnore]
        public virtual ICollection<AppHistoryModel> AppHistories { get; set; }
        [JsonIgnore]
        public virtual ICollection<ReportHistoryModel> ReportHistories { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Username: {Username}, Name: {Name}, AppRole: {AppRole}, Email: {Email}";
        }
    }
}
