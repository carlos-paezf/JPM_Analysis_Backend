using System.ComponentModel.DataAnnotations;
using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.DTO
{
    public abstract class AccountDTO : BaseDTO
    {
        [Key]
        [Required(ErrorMessage = "El número de cuenta es requerido")]
        public string AccountNumber { get; set; } = null!;

        [Required(ErrorMessage = "El nombre de cuenta es requerido")]
        public string AccountName { get; set; } = null!;

        [Required(ErrorMessage = "El tipo de cuenta es requerido")]
        public string AccountType { get; set; } = null!;

        public string? BankCurrency { get; set; }
    }


    public class AccountSimpleDTO : AccountDTO
    {
        public AccountSimpleDTO(Account account)
        {
            AccountNumber = account.AccountNumber;
            AccountName = account.AccountName;
            AccountType = account.AccountType;
            BankCurrency = account.BankCurrency;
        }
    }


    public class AccountEagerDTO : AccountDTO
    {
        public ICollection<ClientSimpleDTO> Clients { get; set; } = null!;
        public ICollection<UserEntitlementSimpleDTO> UserEntitlements { get; set; } = null!;

        public AccountEagerDTO(Account account, ICollection<ClientSimpleDTO> clientDTO, ICollection<UserEntitlementSimpleDTO> userEntitlementDTOs)
        {
            AccountNumber = account.AccountNumber;
            AccountName = account.AccountName;
            AccountType = account.AccountType;
            BankCurrency = account.BankCurrency;
            Clients = clientDTO;
            UserEntitlements = userEntitlementDTOs;
        }
    }
}