using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using Microsoft.EntityFrameworkCore;


namespace BackendJPMAnalysis.Services
{
    public class BulkSeedService
    {
        private readonly JPMDatabaseContext _context;

        private readonly DataAccessService _dataAccessService;


        public BulkSeedService(
            JPMDatabaseContext context,
            DataAccessService dataAccessService
        )
        {
            _context = context;
            _dataAccessService = dataAccessService;
        }


        /// <summary>
        /// Asynchronously posts seed data to the database, including accounts, products, functions, 
        /// profiles, company users, products accounts, users entitlements, and profiles functions.
        /// </summary>
        /// <param name="data">The ExcelDataDTO object containing seed data to post.</param>
        public async Task PostSeedInDatabase(ExcelDataDTO data)
        {
            await BulkPost(data.Accounts, "accounts");
            await BulkPost(data.Products, "products");
            await BulkPost(data.Functions, "functions");
            await BulkPost(data.Profiles, "profiles");
            await BulkPost(data.CompanyUsers, "companyUsers", ValidateProfiles);
            await BulkPost(data.ProductsAccounts, "productsAccounts", ValidateProductsAndAccounts);
            await BulkPost(data.UsersEntitlements, "usersEntitlements", ValidateRelationsForUserEntitlement);
            await BulkPost(data.ProfilesFunctions, "profiles_functions", ValidateProfilesAndFunctions);
        }


        /// <summary>
        /// Asynchronously finds duplicate entities within the provided collection based on existing entity IDs 
        /// in the database.
        /// </summary>
        /// <typeparam name="T">The type of entities within the collection.</typeparam>
        /// <param name="collectionBody">The collection of entities to search for duplicates.</param>
        /// <returns>A list of duplicate entities found within the collection.</returns>
        private async Task<List<T>> FindDuplicateEntities<T>(ICollection<T> collectionBody) where T : class
        {
            var existingEntityIds = await _context.Set<T>().Select(e => e.GetId()).ToListAsync();
            return collectionBody.Where(e => existingEntityIds.Contains(e.GetId())).ToList();
        }


        /// <summary>
        /// Asynchronously performs a bulk insert of entities of type T into the database, with optional 
        /// duplicate validation.
        /// </summary>
        /// <typeparam name="T">The type of entities to insert.</typeparam>
        /// <param name="collectionBody">The collection of entities to insert.</param>
        /// <param name="entityName">The name of the entity being inserted.</param>
        /// <param name="validateDuplicates">An optional method to validate duplicate entities 
        /// within the collection.</param>
        private async Task BulkPost<T>(
            ICollection<T> collectionBody, string entityName,
            Func<ICollection<T>, Task<List<string>>>? validateDuplicates = null
        ) where T : class
        {
            if (validateDuplicates != null)
            {
                var invalidIds = await validateDuplicates(collectionBody);
                if (invalidIds.Any())
                {
                    throw new BadRequestException($"Propiedades invalidas: {string.Join(", ", invalidIds)}");
                }
            }

            var duplicateEntries = await FindDuplicateEntities(collectionBody);
            if (duplicateEntries.Any())
            {
                throw new BadRequestException($"Se encontraron {duplicateEntries.Count} registros duplicados en la base de datos para '{entityName}'. Estos registros ya existen y no pueden ser agregados.");
            }

            await _context.Set<T>().AddRangeAsync(collectionBody);
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Asynchronously validates the profiles referenced in the collection of CompanyUserModel entities.
        /// </summary>
        /// <param name="collectionBody">The collection of CompanyUserModel entities to validate profiles for.</param>
        /// <returns>A list of invalid profile IDs found in the collection.</returns>
        private async Task<List<string>> ValidateProfiles(ICollection<CompanyUserModel> collectionBody)
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
        private async Task<List<string>> ValidateProductsAndAccounts(ICollection<ProductAccountModel> collectionBody)
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
        private async Task<List<string>> ValidateRelationsForUserEntitlement(ICollection<UserEntitlementModel> collectionBody)
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
        private async Task<List<string>> ValidateProfilesAndFunctions(ICollection<ProfileFunctionModel> collectionBody)
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