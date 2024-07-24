using System.Data.Entity;
using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using Microsoft.EntityFrameworkCore;


namespace BackendJPMAnalysis.Services
{
    public class BulkSeedService
    {
        private readonly JPMDatabaseContext _context;

        private readonly ReportValidationsService _reportValidationsService;


        public BulkSeedService(
            JPMDatabaseContext context,
            ReportValidationsService reportValidationsService
        )
        {
            _context = context;
            _reportValidationsService = reportValidationsService;
        }


        /// <summary>
        /// The ClearDatabase method clears all data from multiple tables in a database while
        /// temporarily disabling foreign key checks.
        /// </summary>
        public async Task ClearDatabase()
        {
            await _context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0;");

            var tables = new string[] {
                    "user_entitlements",
                    "products_accounts",
                    "profiles_functions",
                    "accounts",
                    "company_users",
                    "departments",
                    "functions",
                    "profiles",
                    "products",
                };

            foreach (var table in tables)
            {
                await _context.Database.ExecuteSqlRawAsync($"DELETE FROM {table};");
            }
        }


        /// <summary>
        /// Asynchronously posts seed data to the database, including accounts, products, functions, 
        /// profiles, company users, products accounts, users entitlements, and profiles functions.
        /// </summary>
        /// <param name="data">The ExcelDataDTO object containing seed data to post.</param>
        public async Task PostSeedInDatabase(ExcelDataDTO data)
        {
            await ClearDatabase();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await BulkPost(data.Accounts, "accounts");
                await BulkPost(data.Products, "products");
                await BulkPost(data.Functions, "functions");
                await BulkPost(data.Profiles, "profiles");
                await BulkPost(data.CompanyUsers, "companyUsers", _reportValidationsService.ValidateProfiles);
                await BulkPost(data.ProductsAccounts, "productsAccounts", _reportValidationsService.ValidateProductsAndAccounts);
                await BulkPost(data.UsersEntitlements, "usersEntitlements", _reportValidationsService.ValidateRelationsForUserEntitlement);
                await BulkPost(data.ProfilesFunctions, "profiles_functions", _reportValidationsService.ValidateProfilesAndFunctions);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        /// <summary>
        /// Approves the changes specified in the provided data, including deletion, update, and addition of entities, and returns the total number of affected rows.
        /// </summary>
        /// <param name="data">The data containing entities to be deleted, updated, or added.</param>
        /// <returns>The total number of affected rows after approving the changes.</returns>
        public async Task<int> ApproveChanges(EntitiesToChangesDTO data)
        {
            int affectedRows = 0;

            affectedRows += await BulkDelete<ProfileFunctionModel>(data.ProfilesFunctions.ToDeleteEntities);
            affectedRows += await BulkDelete<UserEntitlementModel>(data.UsersEntitlements.ToDeleteEntities);
            affectedRows += await BulkDelete<ProductAccountModel>(data.ProductsAccounts.ToDeleteEntities);
            affectedRows += await BulkDelete<CompanyUserModel>(data.CompanyUsers.ToDeleteEntities);
            affectedRows += await BulkDelete<ProfileModel>(data.Profiles.ToDeleteEntities);
            affectedRows += await BulkDelete<FunctionModel>(data.Functions.ToDeleteEntities);
            affectedRows += await BulkDelete<ProductModel>(data.Products.ToDeleteEntities);
            affectedRows += await BulkDelete<AccountModel>(data.Accounts.ToDeleteEntities);


            affectedRows += await BulkUpdate(data.Accounts.ToChangesEntities);
            affectedRows += await BulkUpdate(data.Products.ToChangesEntities);
            affectedRows += await BulkUpdate(data.Functions.ToChangesEntities);
            affectedRows += await BulkUpdate(data.Profiles.ToChangesEntities);
            affectedRows += await BulkUpdate(data.CompanyUsers.ToChangesEntities);
            affectedRows += await BulkUpdate(data.ProductsAccounts.ToChangesEntities);
            affectedRows += await BulkUpdate(data.UsersEntitlements.ToChangesEntities);
            affectedRows += await BulkUpdate(data.ProfilesFunctions.ToChangesEntities);


            affectedRows += await BulkPost(data.Accounts.NewEntities, "accounts");
            affectedRows += await BulkPost(data.Products.NewEntities, "products");
            affectedRows += await BulkPost(data.Functions.NewEntities, "functions");
            affectedRows += await BulkPost(data.Profiles.NewEntities, "profiles");
            affectedRows += await BulkPost(data.CompanyUsers.NewEntities, "companyUsers", _reportValidationsService.ValidateProfiles);
            affectedRows += await BulkPost(data.ProductsAccounts.NewEntities, "productsAccounts", _reportValidationsService.ValidateProductsAndAccounts);
            affectedRows += await BulkPost(data.UsersEntitlements.NewEntities, "usersEntitlements", _reportValidationsService.ValidateRelationsForUserEntitlement);
            affectedRows += await BulkPost(data.ProfilesFunctions.NewEntities, "profiles_functions", _reportValidationsService.ValidateProfilesAndFunctions);

            return affectedRows;
        }


        /// <summary>
        /// Updates the report history in the database with the specified details.
        /// </summary>
        /// <param name="fileName">The name of the report file.</param>
        /// <param name="appUserId">The ID of the user who uploaded the report (optional).</param>
        /// <param name="observations">Observations or comments related to the report (optional).</param>
        /// <param name="runReportDate">The date when the report was run (optional).</param>
        public async Task UpdateReportHistory(string fileName, string appUserId, string? observations, DateTime? runReportDate)
        {
            _context.ReportHistories.Add(
                new ReportHistoryModel
                {
                    Id = (fileName + DateTime.Now.ToString()).Replace(' ', '_').Replace('.', '_').Replace('/', '-').Replace(':', '-'),
                    AppUserId = appUserId,
                    // AppUserId = null,
                    ReportName = fileName,
                    ReportComments = (observations != "" && observations != null) ? observations : null,
                    ReportUploadDate = DateTime.Now,
                    RunReportDate = runReportDate
                }
            );

            await _context.SaveChangesAsync();
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
        private async Task<int> BulkPost<T>(
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

            /* var duplicateEntries = await FindDuplicateEntities(collectionBody);
            if (duplicateEntries.Any())
            {
                throw new BadRequestException($"Se encontraron {duplicateEntries.Count} registros duplicados en la base de datos para '{entityName}'. Estos registros ya existen y no pueden ser agregados.");
            } */

            await _context.Set<T>().AddRangeAsync(collectionBody);
            int affectedRows = await _context.SaveChangesAsync();

            return affectedRows;
        }


        /// <summary>
        /// Deletes entities of type T from the database in bulk based on the provided primary keys (PKs) and returns the total number of affected rows.
        /// </summary>
        /// <typeparam name="T">The type of entities to delete.</typeparam>
        /// <param name="pks">The collection of primary keys (PKs) of entities to delete.</param>
        /// <returns>The total number of affected rows after deleting the entities.</returns>
        private async Task<int> BulkDelete<T>(ICollection<string> pks) where T : class
        {
            var pksList = pks.ToList();

            var entities = await _context.Set<T>().ToListAsync();

            var entitiesToDelete = entities
                .Where(entity => pksList.Contains(entity.GetId()))
                .ToList();

            var nonExistentPks = pksList.Except(entitiesToDelete.Select(pf => pf.GetId()));
            if (nonExistentPks.Any())
            {
                throw new BadRequestException($"Los siguientes PKs no existen en la base de datos: {string.Join(", ", nonExistentPks)}");
            }

            _context.Set<T>().RemoveRange(entitiesToDelete);
            int affectedRows = await _context.SaveChangesAsync();

            return affectedRows;
        }


        /// <summary>
        /// Updates entities of type T in bulk in the database based on the provided collection of entities and returns the total number of affected rows.
        /// </summary>
        /// <typeparam name="T">The type of entities to update.</typeparam>
        /// <param name="collectionBody">The collection of entities containing the updated data.</param>
        /// <returns>The total number of affected rows after updating the entities.</returns>
        private async Task<int> BulkUpdate<T>(ICollection<T> collectionBody) where T : class
        {
            foreach (var updatedEntity in collectionBody)
            {
                var pk = updatedEntity.GetId();
                var existingEntity = await _context.Set<T>().FindAsync(pk);

                if (existingEntity != null)
                {
                    _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
                }
            }

            int affectedRows = await _context.SaveChangesAsync();

            return affectedRows;
        }
    }
}