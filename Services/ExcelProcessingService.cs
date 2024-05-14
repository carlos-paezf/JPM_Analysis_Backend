using OfficeOpenXml;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.DTO;


namespace BackendJPMAnalysis.Services
{
    public class ExcelProcessingService
    {
        private readonly Dictionary<string, string> new_names = new()
        {
            { "Summary", "summary" },
            { "Users", "company-users" },
            { "UserEntitlements", "users-entitlements" },
            { "Client", "products-accounts" },
        };


        private readonly NormalizeEntitiesService _normalizeEntitiesService;


        public ExcelProcessingService(
            NormalizeEntitiesService normalizeEntitiesService
        )
        {
            _normalizeEntitiesService = normalizeEntitiesService;
        }


        /// <summary>
        /// Extracts data from an Excel file stream, processes it, and returns it as an ExcelDataDTO object.
        /// </summary>
        /// <param name="fileStream">The stream of the Excel file to extract data from.</param>
        /// <param name="fileName">The Excel file name.</param>
        /// <returns>An ExcelDataDTO object containing the extracted and normalized data.</returns>
        public ExcelDataDTO ExtractDataFromExcelFile(Stream fileStream, string fileName)
        {
            var result = ExtractsSheetsData(fileStream);

            var runReportDate = StringUtil.ParseDateTime(result["summary"][3][1]);

            result.Remove("summary");

            var customFormatData = ConvertToCustomFormat(result);

            var normalizedProfiles = _normalizeEntitiesService.NormalizeProfiles();

            var normalizedAccounts = _normalizeEntitiesService.NormalizeAccounts(customFormatData["users-entitlements"], customFormatData["products-accounts"]);
            var normalizedProducts = _normalizeEntitiesService.NormalizeProducts(customFormatData["users-entitlements"], customFormatData["products-accounts"]);
            var normalizedFunctions = _normalizeEntitiesService.NormalizeFunctions(customFormatData["users-entitlements"]);
            var normalizedProductsAccounts = _normalizeEntitiesService.NormalizeProductsAccounts(customFormatData["products-accounts"]);
            var normalizedUsersEntitlements = _normalizeEntitiesService.NormalizeUsersEntitlements(customFormatData["users-entitlements"]);

            var companyUsersRaw = _normalizeEntitiesService.NormalizeCompanyUsersRaw(customFormatData["company-users"]);

            var normalizedCompanyUsers = _normalizeEntitiesService.DefineUserProfile(normalizedUsersEntitlements, companyUsersRaw);
            var normalizedProfilesFunctions = _normalizeEntitiesService.DefineProfilesFunctions(normalizedUsersEntitlements, normalizedCompanyUsers);

            var data = new ExcelDataDTO
            (
                fileName,
                runReportDate,
                normalizedAccounts,
                normalizedProducts,
                normalizedFunctions,
                normalizedProfiles,
                normalizedCompanyUsers,
                normalizedProductsAccounts,
                normalizedUsersEntitlements,
                normalizedProfilesFunctions
            );

            return data;
        }


        /// <summary>
        /// Extracts data from the sheets of an Excel file stream.
        /// </summary>
        /// <param name="fileStream">The stream of the Excel file to extract data from.</param>
        /// <returns>A dictionary where keys are sheet names and values are lists of lists representing the data in each sheet.</returns>
        private Dictionary<string, List<List<string?>>> ExtractsSheetsData(Stream fileStream)
        {
            using var package = new ExcelPackage(fileStream);

            return package.Workbook.Worksheets
                .Where(sheet => new_names.ContainsKey(sheet.Name))
                .ToDictionary(sheet => new_names[sheet.Name], ReadSheetData);
        }


        /// <summary>
        /// Reads data from an Excel worksheet and converts it into a list of lists of strings.
        /// </summary>
        /// <param name="sheet">The ExcelWorksheet object representing the worksheet to read data from.</param>
        /// <returns>A list of lists representing the data in the worksheet.</returns>
        private static List<List<string?>> ReadSheetData(ExcelWorksheet sheet)
        {
            var sheetData = new List<List<string?>>();

            var startRow = sheet.Dimension.Start.Row;
            var endRow = sheet.Dimension.End.Row;
            var startCol = sheet.Dimension.Start.Column;
            var endCol = sheet.Dimension.End.Column;

            for (int row = startRow; row <= endRow; row++)
            {
                var rowData = ReadRowData(sheet, row, startCol, endCol);
                sheetData.Add(rowData);
            }

            return sheetData;
        }


        /// <summary>
        /// Reads data from a row in an Excel worksheet and converts it into a list of strings.
        /// </summary>
        /// <param name="sheet">The ExcelWorksheet object representing the worksheet to read data from.</param>
        /// <param name="row">The row number from which to read data.</param>
        /// <param name="startCol">The starting column index from which to read data.</param>
        /// <param name="endCol">The ending column index up to which to read data.</param>
        /// <returns>A list of strings representing the data in the specified row.</returns>
        private static List<string?> ReadRowData(ExcelWorksheet sheet, int row, int startCol, int endCol)
        {
            var rowData = new List<string?>();

            for (int col = startCol; col <= endCol; col++)
            {
                var cellValue = sheet.Cells[row, col].Value?.ToString() ?? null;
                rowData.Add(cellValue);
            }

            return rowData;
        }


        /// <summary>
        /// Converts raw Excel data into a custom format represented by a dictionary of sheets, each containing a list of dictionaries representing rows with column headers as keys.
        /// </summary>
        /// <param name="excelData">The raw Excel data to convert.</param>
        /// <returns>A dictionary where keys are sheet names and values are lists of dictionaries representing rows with column headers as keys.</returns>
        private static Dictionary<string, List<Dictionary<string, string?>>> ConvertToCustomFormat(Dictionary<string, List<List<string?>>> excelData)
        {
            return excelData.ToDictionary(
                sheetData => sheetData.Key, // Define key: Sheet Name
                sheetData => sheetData.Value.Skip(1) // Skip headers rows
                    .Select(row => sheetData.Value[0].Zip(
                                row,
                                (header, value) => new { Header = header, Value = value }))
                    .Select(cells => cells.ToDictionary(
                                cell => StringUtil.SnakeCase(cell.Header!), // Define key: Column Name
                                cell => cell.Value))
                    .ToList()
            );
        }
    }
}