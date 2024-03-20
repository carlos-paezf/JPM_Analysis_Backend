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
}