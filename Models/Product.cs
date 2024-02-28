using System.ComponentModel.DataAnnotations;


namespace BackendJPMAnalysis.Models
{
    public partial class Product : BaseEntity
    {
        public Product()
        {
            Clients = new HashSet<Client>();
            UserEntitlements = new HashSet<UserEntitlement>();
        }

        /// <summary>
        /// Product name in snake_case
        /// </summary>
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `productName` es requerida")]
        public string ProductName { get; set; } = null!;

        public string? SubProduct { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
        public virtual ICollection<UserEntitlement> UserEntitlements { get; set; }
    }
}
