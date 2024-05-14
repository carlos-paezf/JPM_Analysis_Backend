using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using Microsoft.EntityFrameworkCore;


namespace BackendJPMAnalysis.Services
{
    public class CompareSeedService
    {
        private readonly JPMDatabaseContext _context;

        public CompareSeedService(
            JPMDatabaseContext context
        )
        {
            _context = context;
        }


        /// <summary>
        /// Compares the data from the Excel file with the data in the database and generates a report containing the comparison results for each entity type.
        /// </summary>
        /// <param name="excelData">The data extracted from the Excel file.</param>
        /// <param name="fileName">The name of the Excel file being compared.</param>
        /// <param name="observations">Observations or comments related to the comparison process (optional).</param>
        /// <returns>An Excel comparison data report containing the comparison results for each entity type.</returns>
        public async Task<ExcelCompareDataReportDTO> CompareSeedWithDBData(ExcelDataDTO excelData, string fileName, string? observations)
        {
            var accountsReport = await CompareEntities(excelData.Accounts, () => new AccountComparer());
            var productsReport = await CompareEntities(excelData.Products, () => new ProductComparer());
            var functionsReport = await CompareEntities(excelData.Functions, () => new FunctionComparer());
            var profilesReport = await CompareEntities(excelData.Profiles, () => new ProfileComparer());
            var companyUsersReport = await CompareEntities(excelData.CompanyUsers, () => new CompanyUserComparer());
            var productsAccountsReport = await CompareEntities(excelData.ProductsAccounts, () => new ProductAccountComparer());
            var usersEntitlementsReport = await CompareEntities(excelData.UsersEntitlements, () => new UserEntitlementComparer());
            var profilesFunctionsReport = await CompareEntities(excelData.ProfilesFunctions, () => new ProfileFunctionComparer());

            return new ExcelCompareDataReportDTO
            (
                excelData.RunReportDate,
                fileName,
                accountsReport,
                productsReport,
                functionsReport,
                profilesReport,
                companyUsersReport,
                productsAccountsReport,
                usersEntitlementsReport,
                profilesFunctionsReport,
                observations
            );
        }


        /// <summary>
        /// Compares entities extracted from an Excel file with entities from the database using a custom comparer and generates a report containing the comparison results.
        /// </summary>
        /// <typeparam name="Model">The type of entities being compared.</typeparam>
        /// <typeparam name="Comparer">The type of comparer used to compare entities.</typeparam>
        /// <param name="excelEntities">The list of entities extracted from the Excel file.</param>
        /// <param name="comparerFactory">A factory method for creating an instance of the comparer.</param>
        /// <returns>A data transfer object (DTO) containing the comparison results between Excel entities and database entities.</returns>
        public async Task<DiffExcelVSDatabaseDTO<Model>> CompareEntities<Model, Comparer>(
            List<Model> excelEntities, Func<Comparer> comparerFactory
        ) where Model : class where Comparer : IEqualityComparer<Model>
        {
            var dbEntities = await _context.Set<Model>().ToListAsync();

            var newEntities = excelEntities
                .Where(excel => !dbEntities.Any(db => db.GetId() == excel.GetId())).ToList();

            var toDeleteEntities = dbEntities
                .Where(db => !excelEntities.Any(excel => excel.GetId() == db.GetId())).ToList();

            var comparer = comparerFactory();

            var entitiesWithDifferences = dbEntities
                .Join(
                    excelEntities,
                    db => db.GetId(),
                    excel => excel.GetId(),
                    (db, excel) => new DifferenceEntity<Model> { ExcelEntity = excel, DatabaseEntity = db }
                )
                .Where(pair => !comparer.Equals(pair.ExcelEntity, pair.DatabaseEntity))
                .ToList();

            return new DiffExcelVSDatabaseDTO<Model>
            (
                countRowsInDatabase: dbEntities.Count,
                countRowsInExcel: excelEntities.Count,
                newEntities,
                toDeleteEntities,
                entitiesWithDifferences
            );
        }
    }
}