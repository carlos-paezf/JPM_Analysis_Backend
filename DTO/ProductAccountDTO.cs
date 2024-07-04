using System.ComponentModel.DataAnnotations;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.DTO
{
    public abstract class ProductAccountDTO : BaseDTO
    {
        [Key]
        public string Id { get; set; } = null!;
        public string? ProductId { get; set; }
        public string? AccountNumber { get; set; }
    }


    public class ProductAccountSimpleDTO : ProductAccountDTO
    {
        public ProductAccountSimpleDTO() { }

        public ProductAccountSimpleDTO(ProductAccountModel productAccount)
        {
            Id = productAccount.Id;
            ProductId = productAccount.ProductId;
            AccountNumber = productAccount.AccountNumber;
            CreatedAt = productAccount.CreatedAt;
            UpdatedAt = productAccount.UpdatedAt;
            DeletedAt = productAccount.DeletedAt;
        }

        public override string ToString()
        {
            return $"Id: {Id}, ProductId: {ProductId}, AccountNumber: {AccountNumber}";
        }
    }


    public class ProductAccountEagerDTO : ProductAccountDTO
    {
        public ProductSimpleDTO? Product { get; set; }
        public AccountSimpleDTO? Account { get; set; }

        public ProductAccountEagerDTO(ProductAccountModel productAccount, ProductSimpleDTO? product, AccountSimpleDTO? account)
        {
            Id = productAccount.Id;
            ProductId = productAccount.ProductId;
            AccountNumber = productAccount.AccountNumber;
            Product = product;
            Account = account;
            CreatedAt = productAccount.CreatedAt;
            UpdatedAt = productAccount.UpdatedAt;
            DeletedAt = productAccount.DeletedAt;
        }
    }


    public class ProductAccountEagerV2DTO : ProductAccountDTO
    {
        public string? ProductName { get; set; } = null!;
        public string? SubProduct { get; set; } = null!;
        public string? AccountName { get; set; } = null!;
        public string? AccountType { get; set; } = null!;
        public string? BankCurrency { get; set; } = null!;

        public ProductAccountEagerV2DTO(ProductAccountModel productAccount, ProductSimpleDTO? product, AccountSimpleDTO? account)
        {
            Id = productAccount.Id;
            ProductId = productAccount.ProductId;
            if (product != null)
            {
                ProductName = product.ProductName;
                SubProduct = product.SubProduct;
            }

            if (account != null)
            {
                AccountNumber = productAccount.AccountNumber;
                AccountName = account.AccountName;
                AccountType = account.AccountType;
                BankCurrency = account.BankCurrency;
            }

            CreatedAt = productAccount.CreatedAt;
            UpdatedAt = productAccount.UpdatedAt;
            DeletedAt = productAccount.DeletedAt;
        }
    }
}