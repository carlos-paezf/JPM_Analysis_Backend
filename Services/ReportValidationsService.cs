using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.Services
{
    public class ReportValidationsService
    {
        private readonly JPMDatabaseContext _context;

        private readonly DataAccessService _dataAccessService;

        public ReportValidationsService(
            JPMDatabaseContext context,
            DataAccessService dataAccessService
        )
        {
            _context = context;
            _dataAccessService = dataAccessService;
        }


        /// <summary>
        /// Asynchronously validates the profiles referenced in the collection of CompanyUserModel entities.
        /// </summary>
        /// <param name="collectionBody">The collection of CompanyUserModel entities to validate profiles for.</param>
        /// <returns>A list of invalid profile IDs found in the collection.</returns>
        public async Task<List<string>> ValidateProfiles(ICollection<CompanyUserModel> collectionBody)
        {
            var profilesIds = collectionBody.Select(pf => pf.ProfileId).Distinct().ToList();

            var existingProfiles = await _dataAccessService.CheckExistingEntitiesAsync<ProfileModel>(profilesIds);

            var invalidProfilesIds = profilesIds.Where(id => !existingProfiles.Contains(id)).ToList();

            return invalidProfilesIds.Distinct().ToList();
        }


        /// <summary>
        /// Asynchronously validates the products and accounts referenced in the collection of 
        /// ProductAccountModel entities.
        /// </summary>
        /// <param name="collectionBody">The collection of ProductAccountModel entities to validate 
        /// products and accounts for.</param>
        /// <returns>A list of invalid product IDs and account numbers found in the collection.</returns>
        public async Task<List<string>> ValidateProductsAndAccounts(ICollection<ProductAccountModel> collectionBody)
        {
            var productsIds = collectionBody
                .Where(p => p.ProductId != null)
                .Select(p => p.ProductId).Distinct().ToList();
            var accountsNumber = collectionBody
                .Where(a => a.AccountNumber != null)
                .Select(a => a.AccountNumber).Distinct().ToList();

            var existingProducts = await _dataAccessService.CheckExistingEntitiesAsync<ProductModel>(productsIds!);
            var existingAccounts = await _dataAccessService.CheckExistingEntitiesAsync<AccountModel>(accountsNumber!);

            var invalidProductsIds = productsIds.Where(id => !existingProducts.Contains(id!)).ToList();
            var invalidAccountsNumbers = accountsNumber.Where(id => !existingAccounts.Contains(id!)).ToList();

            return invalidProductsIds.Concat(invalidAccountsNumbers).Distinct().ToList()!;
        }


        /// <summary>
        /// Asynchronously validates the relations (company users, accounts, products, and functions) 
        /// referenced in the collection of UserEntitlementModel entities.
        /// </summary>
        /// <param name="collectionBody">The collection of UserEntitlementModel entities to validate 
        /// relations for.</param>
        /// <returns>A list of invalid IDs found in the collection.</returns>
        public async Task<List<string>> ValidateRelationsForUserEntitlement(ICollection<UserEntitlementModel> collectionBody)
        {
            var companyUsersIds = collectionBody.Select(ue => ue.AccessId).Distinct().ToList();
            var accountsNumber = collectionBody
                .Where(ue => ue.AccountNumber != null)
                .Select(ue => ue.AccountNumber).Distinct().ToList();
            var productsIds = collectionBody
                .Where(ue => ue.ProductId != null)
                .Select(ue => ue.ProductId).Distinct().ToList();
            var functionsIds = collectionBody
                .Where(ue => ue.FunctionId != null)
                .Select(ue => ue.FunctionId).Distinct().ToList();

            var existingCompanyUsers = await _dataAccessService.CheckExistingEntitiesAsync<CompanyUserModel>(companyUsersIds);
            var existingAccounts = await _dataAccessService.CheckExistingEntitiesAsync<AccountModel>(accountsNumber!);
            var existingProducts = await _dataAccessService.CheckExistingEntitiesAsync<ProductModel>(productsIds!);
            var existingFunctions = await _dataAccessService.CheckExistingEntitiesAsync<FunctionModel>(functionsIds!);

            var invalidCompanyUsers = companyUsersIds.Where(id => !existingCompanyUsers.Contains(id!)).ToList();
            var invalidAccountsNumbers = accountsNumber.Where(id => !existingAccounts.Contains(id!)).ToList();
            var invalidProductsIds = productsIds.Where(id => !existingProducts.Contains(id!)).ToList();
            var invalidFunctionsIds = functionsIds.Where(id => !existingFunctions.Contains(id!)).ToList();

            return invalidCompanyUsers
                .Concat(invalidAccountsNumbers)
                .Concat(invalidProductsIds)
                .Concat(invalidFunctionsIds)
                .Distinct().ToList()!;
        }


        /// <summary>
        /// Asynchronously validates the profiles and functions referenced in the collection of 
        /// ProfileFunctionModel entities.
        /// </summary>
        /// <param name="collectionBody">The collection of ProfileFunctionModel entities to validate 
        /// profiles and functions for.</param>
        /// <returns>A list of invalid profile and function IDs found in the collection.</returns>
        public async Task<List<string>> ValidateProfilesAndFunctions(ICollection<ProfileFunctionModel> collectionBody)
        {
            var profilesIds = collectionBody.Select(pf => pf.ProfileId).Distinct().ToList();
            var functionsIds = collectionBody.Select(pf => pf.FunctionId).Distinct().ToList();

            var existingProfiles = await _dataAccessService.CheckExistingEntitiesAsync<ProfileModel>(profilesIds);
            var existingFunctions = await _dataAccessService.CheckExistingEntitiesAsync<FunctionModel>(functionsIds);

            var invalidProfilesIds = profilesIds.Where(id => !existingProfiles.Contains(id)).ToList();
            var invalidFunctionsIds = functionsIds.Where(id => !existingFunctions.Contains(id)).ToList();

            return invalidProfilesIds.Concat(invalidFunctionsIds).Distinct().ToList();
        }
    }
}