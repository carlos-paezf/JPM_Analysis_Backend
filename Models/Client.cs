using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class Client : BaseModel
    {
        /// <summary>
        /// Auto-incremental ID
        /// </summary>
        [Key]
        public int Id { get; }
        public string? ProductId { get; set; }
        public string? AccountNumber { get; set; }

        [JsonIgnore]
        public virtual Account? AccountNumberNavigation { get; set; }

        [JsonIgnore]
        public virtual Product? Product { get; set; }
    }
}
