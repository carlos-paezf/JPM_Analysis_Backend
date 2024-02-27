using System.ComponentModel.DataAnnotations;


namespace BackendJPMAnalysis.Models
{
    public partial class Account : BaseEntity
    {
        public Account()
        {
            Clients = new HashSet<Client>();
            UserEntitlements = new HashSet<UserEntitlement>();
        }

        [Required(ErrorMessage = "El número de cuenta es requerido")]
        public string AccountNumber { get; set; } = null!;

        [Required(ErrorMessage = "El nombre de cuenta es requerido")]
        public string AccountName { get; set; } = null!;

        [Required(ErrorMessage = "El tipo de cuenta es requerido")]
        public string AccountType { get; set; } = null!;

        public string? BankCurrency { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
        public virtual ICollection<UserEntitlement> UserEntitlements { get; set; }
    }
}
