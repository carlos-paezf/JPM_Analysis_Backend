using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class UserEntitlementModel : BaseModel
    {
        /// <summary>
        /// Auto-incremental ID
        /// </summary>
        [Key]
        public string Id { get; } = null!;

        [Required(ErrorMessage = "La propiedad `accessId` es requerida")]
        public string AccessId { get; set; } = null!;

        public string? FunctionType { get; set; }

        public string? AccountNumber { get; set; }

        [Required(ErrorMessage = "La propiedad `productId` es requerida")]
        public string ProductId { get; set; } = null!;

        public string? FunctionId { get; set; }

        [JsonIgnore]
        public virtual CompanyUserModel? CompanyUser { get; set; }

        [JsonIgnore]
        public virtual AccountModel? AccountNumberNavigation { get; set; }

        [JsonIgnore]
        public virtual FunctionModel? Function { get; set; }

        [JsonIgnore]
        public virtual ProductModel? Product { get; set; }
    }
}
