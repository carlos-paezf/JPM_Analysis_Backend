using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class ProductModel : BaseModel
    {
        private string? _subProduct;

        public ProductModel()
        {
            Clients = new HashSet<ClientModel>();
            UserEntitlements = new HashSet<UserEntitlementModel>();
        }

        /// <summary>
        /// Product name and sub-product in snake_case
        /// </summary>
        [Key]
        public string Id { get; private set; } = null!;

        [Required(ErrorMessage = "La propiedad `productName` es requerida")]
        public string ProductName { get; set; } = null!;

        public string? SubProduct
        {
            get => _subProduct;
            set
            {
                _subProduct = value;
                Id ??= StringUtil.SnakeCase(ProductName) + ((value != null) ? "_" + StringUtil.SnakeCase(value) : "");
            }
        }

        [JsonIgnore]
        public virtual ICollection<ClientModel> Clients { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserEntitlementModel> UserEntitlements { get; set; }
    }
}
