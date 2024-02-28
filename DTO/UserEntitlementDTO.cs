using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.DTO
{
    public abstract class UserEntitlementDTO : BaseDTO
    {
        public int Id { get; set; }
        public string AccessId { get; set; } = null!;
        public string? FunctionType { get; set; }
        public string? AccountNumber { get; set; }
        public string ProductId { get; set; } = null!;
        public string? FunctionId { get; set; }
    }


    public class UserEntitlementSimpleDTO : UserEntitlementDTO
    {
        public UserEntitlementSimpleDTO(UserEntitlement userEntitlement)
        {
            Id = userEntitlement.Id;
            AccessId = userEntitlement.AccessId;
            FunctionType = userEntitlement.FunctionType;
            AccountNumber = userEntitlement.AccountNumber;
            ProductId = userEntitlement.ProductId;
            FunctionId = userEntitlement.FunctionId;
        }
    }


    public class UserEntitlementEagerDTO : UserEntitlementDTO
    {
        public CompanyUserSimpleDTO? CompanyUser { get; set; }
        public AccountSimpleDTO? AccountNumberNavigation { get; set; }
        public FunctionSimpleDTO? Function { get; set; }
        public ProductSimpleDTO? Product { get; set; }

        public UserEntitlementEagerDTO(
            UserEntitlement userEntitlement, CompanyUserSimpleDTO companyUserDTO,
            AccountSimpleDTO accountDTO, FunctionSimpleDTO functionDTO, ProductSimpleDTO productDTO
            )
        {
            Id = userEntitlement.Id;
            AccessId = userEntitlement.AccessId;
            FunctionType = userEntitlement.FunctionType;
            AccountNumber = userEntitlement.AccountNumber;
            ProductId = userEntitlement.ProductId;
            FunctionId = userEntitlement.FunctionId;
            CompanyUser = companyUserDTO;
            AccountNumberNavigation = accountDTO;
            Function = functionDTO;
            Product = productDTO;
        }
    }
}