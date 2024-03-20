using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class ProductAccountModel : BaseModel
    {
        /// <summary>
        /// Auto-incremental ID
        /// </summary>
        [Key]
        public string Id { get; } = null!;
        public string? ProductId { get; set; }
        public string? AccountNumber { get; set; }

        [JsonIgnore]
        public virtual AccountModel? Account { get; set; }

        [JsonIgnore]
        public virtual ProductModel? Product { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, ProductId: {ProductId}, AccountNumber: {AccountNumber}";
        }
    }
}
