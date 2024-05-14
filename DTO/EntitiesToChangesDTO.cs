using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.DTO
{
    public class EntitiesToChangesDTO
    {
        public ChangesStructureDTO<AccountModel> Accounts { get; set; } = null!;
        public ChangesStructureDTO<CompanyUserModel> CompanyUsers { get; set; } = null!;
        public ChangesStructureDTO<FunctionModel> Functions { get; set; } = null!;
        public ChangesStructureDTO<ProductModel> Products { get; set; } = null!;
        public ChangesStructureDTO<ProductAccountModel> ProductsAccounts { get; set; } = null!;
        public ChangesStructureDTO<ProfileModel> Profiles { get; set; } = null!;
        public ChangesStructureDTO<ProfileFunctionModel> ProfilesFunctions { get; set; } = null!;
        public ChangesStructureDTO<UserEntitlementModel> UsersEntitlements { get; set; } = null!;
    }


    public class ChangesStructureDTO<Model> where Model : class
    {
        public List<Model> NewEntities { get; set; } = null!;
        public List<Model> ToChangesEntities { get; set; } = null!;
        public List<string> ToDeleteEntities { get; set; } = null!;
    }
}