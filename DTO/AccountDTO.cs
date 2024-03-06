using System.ComponentModel.DataAnnotations;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.DTO
{
    public abstract class AccountDTO : BaseDTO
    {
        public string AccountNumber { get; set; } = null!;

        [Required(ErrorMessage = "El nombre de cuenta es requerido")]
        public string AccountName { get; set; } = null!;

        [Required(ErrorMessage = "El tipo de cuenta es requerido")]
        public string AccountType { get; set; } = null!;

        public string? BankCurrency { get; set; }
    }


    public class AccountSimpleDTO : AccountDTO
    {
        public AccountSimpleDTO() { }

        public AccountSimpleDTO(AccountModel account)
        {
            AccountNumber = account.AccountNumber;
            AccountName = account.AccountName;
            AccountType = account.AccountType;
            BankCurrency = account.BankCurrency;
            CreatedAt = account.CreatedAt;
            UpdatedAt = account.UpdatedAt;
            DeletedAt = account.DeletedAt;
        }
    }


    public class AccountEagerDTO : AccountDTO
    {
        public ICollection<ClientSimpleDTO> Clients { get; set; } = null!;
        public ICollection<UserEntitlementSimpleDTO> UserEntitlements { get; set; } = null!;

        public AccountEagerDTO(AccountModel account, ICollection<ClientSimpleDTO> clientDTO, ICollection<UserEntitlementSimpleDTO> userEntitlementDTOs)
        {
            AccountNumber = account.AccountNumber;
            AccountName = account.AccountName;
            AccountType = account.AccountType;
            BankCurrency = account.BankCurrency;
            Clients = clientDTO;
            UserEntitlements = userEntitlementDTOs;
            CreatedAt = account.CreatedAt;
            UpdatedAt = account.UpdatedAt;
            DeletedAt = account.DeletedAt;
        }
    }
}