
using System.ComponentModel.DataAnnotations;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;

namespace BackendJPMAnalysis.DTO
{
    public abstract class ProductDTO : BaseDTO
    {
        [Key]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `productName` es requerida")]
        public string ProductName { get; set; } = null!;

        public string? SubProduct { get; set; }
    }


    public class ProductSimpleDTO : ProductDTO
    {
        public ProductSimpleDTO() { }

        public ProductSimpleDTO(ProductModel product)
        {
            Id = product.Id;
            ProductName = product.ProductName;
            SubProduct = product.SubProduct;
            CreatedAt = product.CreatedAt;
            UpdatedAt = product.UpdatedAt;
            DeletedAt = product.DeletedAt;
        }
    }


    public class ProductEagerDTO : ProductDTO
    {
        public ICollection<ProductAccountSimpleDTO> ProductsAccounts { get; set; }
        public ICollection<UserEntitlementSimpleDTO> UserEntitlements { get; set; }

        public ProductEagerDTO(ProductModel product, ICollection<ProductAccountSimpleDTO> productsAccountsDTOs, ICollection<UserEntitlementSimpleDTO> userEntitlementDTOs)
        {
            Id = product.Id;
            ProductName = product.ProductName;
            SubProduct = product.SubProduct;
            ProductsAccounts = productsAccountsDTOs;
            UserEntitlements = userEntitlementDTOs;
            CreatedAt = product.CreatedAt;
            UpdatedAt = product.UpdatedAt;
            DeletedAt = product.DeletedAt;
        }
    }
}