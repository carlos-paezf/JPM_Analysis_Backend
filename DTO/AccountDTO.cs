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
        public ICollection<ProductAccountSimpleDTO> ProductsAccounts { get; set; } = null!;
        public ICollection<UserEntitlementSimpleDTO> UserEntitlements { get; set; } = null!;

        public AccountEagerDTO(AccountModel account, ICollection<ProductAccountSimpleDTO> clientDTO, ICollection<UserEntitlementSimpleDTO> userEntitlementDTOs)
        {
            AccountNumber = account.AccountNumber;
            AccountName = account.AccountName;
            AccountType = account.AccountType;
            BankCurrency = account.BankCurrency;
            ProductsAccounts = clientDTO;
            UserEntitlements = userEntitlementDTOs;
            CreatedAt = account.CreatedAt;
            UpdatedAt = account.UpdatedAt;
            DeletedAt = account.DeletedAt;
        }
    }
}