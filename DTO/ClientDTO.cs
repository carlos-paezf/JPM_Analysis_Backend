using System.ComponentModel.DataAnnotations;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.DTO
{
    public abstract class ClientDTO : BaseDTO
    {
        [Key]
        public int Id { get; set; }
        public string? ProductId { get; set; }
        public string? AccountNumber { get; set; }
    }


    public class ClientSimpleDTO : ClientDTO
    {
        public ClientSimpleDTO() { }

        public ClientSimpleDTO(ClientModel client)
        {
            Id = client.Id;
            ProductId = client.ProductId;
            AccountNumber = client.AccountNumber;
            CreatedAt = client.CreatedAt;
            UpdatedAt = client.UpdatedAt;
            DeletedAt = client.DeletedAt;
        }
    }


    public class ClientEagerDTO : ClientDTO
    {
        public ProductSimpleDTO Product { get; set; }
        public AccountSimpleDTO Account { get; set; }

        public ClientEagerDTO(ClientModel client, ProductSimpleDTO product, AccountSimpleDTO account)
        {
            Id = client.Id;
            ProductId = client.ProductId;
            AccountNumber = client.AccountNumber;
            Product = product;
            Account = account;
            CreatedAt = client.CreatedAt;
            UpdatedAt = client.UpdatedAt;
            DeletedAt = client.DeletedAt;
        }
    }
}