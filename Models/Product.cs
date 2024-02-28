using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class Product : BaseModel
    {
        public Product()
        {
            Id = StringUtil.SnakeCase(ProductName);

            Clients = new HashSet<Client>();
            UserEntitlements = new HashSet<UserEntitlement>();
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
        public virtual ICollection<Client> Clients { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserEntitlement> UserEntitlements { get; set; }
    }
}
