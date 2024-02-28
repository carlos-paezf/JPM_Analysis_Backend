using System.Text.Json.Serialization;


namespace BackendJPMAnalysis.Models
{
    public partial class AppHistory
    {
        /// <summary>
        /// Autoincremental
        /// </summary>
        public int Id { get; set; }
        public int? AppUserId { get; set; }
        public string? AppFunction { get; set; }
        public string? AppTable { get; set; }

        [JsonIgnore]
        public virtual AppUser? AppUser { get; set; }
    }
}
