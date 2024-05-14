using BackendJPMAnalysis.Models;

namespace BackendJPMAnalysis.DTO
{
    public class ExcelCompareDataReportDTO
    {
        public DateTime? RunReportDate { get; }
        public string FileName { get; }

        public string? Observations { get; }
        public int TotalRowsInDatabase { get; }
        public int TotalRowsInExcel { get; }
        public int TotalNewEntities { get; }
        public int TotalToDeleteEntities { get; }
        public int TotalEntitiesWithDifferences { get; }

        public DiffExcelVSDatabaseDTO<AccountModel> AccountsReport { get; set; }
        public DiffExcelVSDatabaseDTO<ProductModel> ProductsReport { get; set; }
        public DiffExcelVSDatabaseDTO<FunctionModel> FunctionsReport { get; set; }
        public DiffExcelVSDatabaseDTO<ProfileModel> ProfilesReport { get; set; }
        public DiffExcelVSDatabaseDTO<CompanyUserModel> CompanyUsersReport { get; set; }
        public DiffExcelVSDatabaseDTO<ProductAccountModel> ProductsAccountsReport { get; set; }
        public DiffExcelVSDatabaseDTO<UserEntitlementModel> UsersEntitlementsReport { get; set; }
        public DiffExcelVSDatabaseDTO<ProfileFunctionModel> ProfilesFunctionsReport { get; set; }


        public ExcelCompareDataReportDTO(
            DateTime? runReportDate,
            string fileName,
            DiffExcelVSDatabaseDTO<AccountModel> accountsReport,
            DiffExcelVSDatabaseDTO<ProductModel> productsReport,
            DiffExcelVSDatabaseDTO<FunctionModel> functionsReport,
            DiffExcelVSDatabaseDTO<ProfileModel> profilesReport,
            DiffExcelVSDatabaseDTO<CompanyUserModel> companyUsersReport,
            DiffExcelVSDatabaseDTO<ProductAccountModel> productsAccountsReport,
            DiffExcelVSDatabaseDTO<UserEntitlementModel> usersEntitlementsReport,
            DiffExcelVSDatabaseDTO<ProfileFunctionModel> profilesFunctionsReport,
            string? observations
        )
        {
            RunReportDate = runReportDate;
            FileName = fileName;
            TotalRowsInDatabase = accountsReport.CountRowsInDatabase
                    + productsAccountsReport.CountRowsInDatabase
                    + functionsReport.CountRowsInDatabase
                    + profilesReport.CountRowsInDatabase
                    + companyUsersReport.CountRowsInDatabase
                    + productsAccountsReport.CountRowsInDatabase
                    + usersEntitlementsReport.CountRowsInDatabase
                    + profilesFunctionsReport.CountRowsInDatabase;
            TotalRowsInExcel = accountsReport.CountRowsInExcel
                    + productsAccountsReport.CountRowsInExcel
                    + functionsReport.CountRowsInExcel
                    + profilesReport.CountRowsInExcel
                    + companyUsersReport.CountRowsInExcel
                    + productsAccountsReport.CountRowsInExcel
                    + usersEntitlementsReport.CountRowsInExcel
                    + profilesFunctionsReport.CountRowsInExcel; ;
            TotalNewEntities = accountsReport.CountNewEntities
                    + productsAccountsReport.CountNewEntities
                    + functionsReport.CountNewEntities
                    + profilesReport.CountNewEntities
                    + companyUsersReport.CountNewEntities
                    + productsAccountsReport.CountNewEntities
                    + usersEntitlementsReport.CountNewEntities
                    + profilesFunctionsReport.CountNewEntities;
            TotalToDeleteEntities = accountsReport.CountToDeleteEntities
                    + productsAccountsReport.CountToDeleteEntities
                    + functionsReport.CountToDeleteEntities
                    + profilesReport.CountToDeleteEntities
                    + companyUsersReport.CountToDeleteEntities
                    + productsAccountsReport.CountToDeleteEntities
                    + usersEntitlementsReport.CountToDeleteEntities
                    + profilesFunctionsReport.CountToDeleteEntities;
            TotalEntitiesWithDifferences = accountsReport.CountEntitiesWithDifferences
                    + productsAccountsReport.CountEntitiesWithDifferences
                    + functionsReport.CountEntitiesWithDifferences
                    + profilesReport.CountEntitiesWithDifferences
                    + companyUsersReport.CountEntitiesWithDifferences
                    + productsAccountsReport.CountEntitiesWithDifferences
                    + usersEntitlementsReport.CountEntitiesWithDifferences
                    + profilesFunctionsReport.CountEntitiesWithDifferences;

            AccountsReport = accountsReport;
            ProductsReport = productsReport;
            FunctionsReport = functionsReport;
            ProfilesReport = profilesReport;
            CompanyUsersReport = companyUsersReport;
            ProductsAccountsReport = productsAccountsReport;
            UsersEntitlementsReport = usersEntitlementsReport;
            ProfilesFunctionsReport = profilesFunctionsReport;
            Observations = observations;
        }
    }
}