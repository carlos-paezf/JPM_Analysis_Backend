using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class UserEntitlement : BaseModel
    {
        /// <summary>
        /// Auto-incremental ID
        /// </summary>
        [Key]
        public int Id { get; }

        [Required(ErrorMessage = "La propiedad `accessId` es requerida")]
        public string AccessId { get; set; } = null!;

        public string? FunctionType { get; set; }

        public string? AccountNumber { get; set; }

        [Required(ErrorMessage = "La propiedad `productId` es requerida")]
        public string ProductId { get; set; } = null!;

        public string? FunctionId { get; set; }

        [JsonIgnore]
        public virtual CompanyUser? CompanyUser { get; set; }

        [JsonIgnore]
        public virtual Account? AccountNumberNavigation { get; set; }

        [JsonIgnore]
        public virtual Function? Function { get; set; }

        [JsonIgnore]
        public virtual Product? Product { get; set; }
    }
}
