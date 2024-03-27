using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class AccountModel : BaseModel
    {
        public AccountModel()
        {
            ProductsAccounts = new HashSet<ProductAccountModel>();
            UserEntitlements = new HashSet<UserEntitlementModel>();
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
        public virtual ICollection<ProductAccountModel> ProductsAccounts { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserEntitlementModel> UserEntitlements { get; set; }

        public override string ToString()
        {
            return $"AccountNumber: {AccountNumber}, AccountName: {AccountName}, AccountType: {AccountType}, BankCurrency: {BankCurrency}";
        }
    }

    public static class AccountModelExtensions
    {
        public static string GetId(this AccountModel account)
        {
            return EntityExtensions.GetId(account);
        }
    }
}
