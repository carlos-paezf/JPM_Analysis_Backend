using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class ProductModel : BaseModel
    {
        public ProductModel()
        {
            Id = StringUtil.SnakeCase(ProductName);

            Clients = new HashSet<ClientModel>();
            UserEntitlements = new HashSet<UserEntitlementModel>();
        }

        /// <summary>
        /// Product name in snake_case
        /// </summary>
        [Key]
        public string Id { get; } = null!;

        [Required(ErrorMessage = "La propiedad `productName` es requerida")]
        public string ProductName { get; set; } = null!;

        public string? SubProduct { get; set; }

        [JsonIgnore]
        public virtual ICollection<ClientModel> Clients { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserEntitlementModel> UserEntitlements { get; set; }
    }
}
