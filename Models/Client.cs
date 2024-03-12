using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class ClientModel : BaseModel
    {
        /// <summary>
        /// Auto-incremental ID
        /// </summary>
        [Key]
        public int Id { get; }
        public string? ProductId { get; set; }
        public string? AccountNumber { get; set; }

        [JsonIgnore]
        public virtual AccountModel? Account { get; set; }

        [JsonIgnore]
        public virtual ProductModel? Product { get; set; }
    }
}
