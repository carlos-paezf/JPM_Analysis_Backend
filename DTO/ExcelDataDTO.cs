using BackendJPMAnalysis.Models;

namespace BackendJPMAnalysis.DTO
{
    public class ExcelDataDTO
    {
        public string FileName { get; set; } = null!;
        public DateTime? RunReportDate { get; set; }
        public List<AccountModel> Accounts { get; set; } = null!;
        public List<ProductModel> Products { get; set; } = null!;
        public List<FunctionModel> Functions { get; set; } = null!;
        public List<ProfileModel> Profiles { get; set; } = null!;
        public List<CompanyUserModel> CompanyUsers { get; set; } = null!;
        public List<ProductAccountModel> ProductsAccounts { get; set; } = null!;
        public List<UserEntitlementModel> UsersEntitlements { get; set; } = null!;
        public List<ProfileFunctionModel> ProfilesFunctions { get; set; } = null!;


        public ExcelDataDTO(
            string fileName,
            DateTime? runReportDate,
            List<AccountModel> accounts,
            List<ProductModel> products,
            List<FunctionModel> functions,
            List<ProfileModel> profiles,
            List<CompanyUserModel> companyUsers,
            List<ProductAccountModel> productsAccounts,
            List<UserEntitlementModel> usersEntitlements,
            List<ProfileFunctionModel> profilesFunctions
        )
        {
            FileName = fileName;
            RunReportDate = runReportDate;
            Accounts = accounts;
            Products = products;
            Functions = functions;
            Profiles = profiles;
            CompanyUsers = companyUsers;
            ProductsAccounts = productsAccounts;
            UsersEntitlements = usersEntitlements;
            ProfilesFunctions = profilesFunctions;
        }
    }
}