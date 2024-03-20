using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class UserEntitlementModel : BaseModel
    {
        private string? _functionType;

        /// <summary>
        /// Join AccessID, AccountNumber, ProductID, FunctionID and FunctionType in snake_case
        /// </summary>
        [Key]
        public string Id { get; private set; } = null!;

        [Required(ErrorMessage = "La propiedad `accessId` es requerida")]
        public string AccessId { get; set; } = null!;

        public string? AccountNumber { get; set; }

        [Required(ErrorMessage = "La propiedad `productId` es requerida")]
        public string ProductId { get; set; } = null!;

        public string? FunctionId { get; set; }

        public string? FunctionType
        {
            get => _functionType;
            set
            {
                _functionType = value;
                Id ??= StringUtil.SnakeCase(
                    AccessId + '_'
                    + (AccountNumber ?? string.Empty) + '_'
                    + ProductId + '_'
                    + (FunctionId ?? string.Empty) + '_'
                    + (FunctionType ?? string.Empty));
            }
        }

        [JsonIgnore]
        public virtual CompanyUserModel? CompanyUser { get; set; }

        [JsonIgnore]
        public virtual AccountModel? Account { get; set; }

        [JsonIgnore]
        public virtual FunctionModel? Function { get; set; }

        [JsonIgnore]
        public virtual ProductModel? Product { get; set; }
    }
}
