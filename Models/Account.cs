using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace BackendJPMAnalysis.Models
{
    public partial class Account : BaseModel
    {
        public Account()
        {
            Clients = new HashSet<Client>();
            UserEntitlements = new HashSet<UserEntitlement>();
        }

        [Key]
        [Required(ErrorMessage = "El número de cuenta es requerido")]
        public string AccountNumber { get; set; } = null!;

        [Required(ErrorMessage = "El nombre de cuenta es requerido")]
        public string AccountName { get; set; } = null!;

        [Required(ErrorMessage = "El tipo de cuenta es requerido")]
        public string AccountType { get; set; } = null!;

        public string? BankCurrency { get; set; }

        [JsonIgnore]
        public virtual ICollection<Client> Clients { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserEntitlement> UserEntitlements { get; set; }
    }
}
