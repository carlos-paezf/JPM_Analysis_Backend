using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.Services
{
    public class NormalizeEntitiesService
    {
        /// <summary>
        /// Normalizes profile names by converting them to snake case and constructs a list of ProfileModel objects.
        /// </summary>
        /// <returns>A list of ProfileModel objects with normalized profile names.</returns>
        public List<ProfileModel> NormalizeProfiles()
        {
            var profiles = new List<Dictionary<string, string>>
            {
                new() {{"profile", "Capture"}},
                new() {{"profile", "Authorization"}},
                new() {{"profile", "Verification"}},
                new() {{"profile", "Administrator"}},
                new() {{"profile", "Viewer"}},
            };

            return profiles
                .Select(
                    item => new ProfileModel
                    {
                        ProfileName = item["profile"]
                    }
                )
                .ToList();
        }


        /// <summary>
        /// Normalizes account information by filtering out invalid entries and constructing a list of AccountModel objects.
        /// </summary>
        /// <param name="originalUsersEntitlements">List of dictionaries containing user entitlement information.</param>
        /// <param name="originalProductsAccounts">List of dictionaries containing product account information.</param>
        /// <returns>A list of AccountModel objects with normalized account information.</returns>
        public List<AccountModel> NormalizeAccounts(List<Dictionary<string, string?>> originalUsersEntitlements, List<Dictionary<string, string?>> originalProductsAccounts)
        {
            var data = originalUsersEntitlements.Concat(originalProductsAccounts).ToList();

            return data
                .Where(
                    item => item.TryGetValue("account_name", out var accountName)
                            && item.TryGetValue("account_type", out var accountType)
                            && !string.IsNullOrEmpty(accountName)
                            && !string.IsNullOrEmpty(accountType))
                .Select(
                    item =>
                    {
                        item.TryGetValue("account_number", out string? accountNumber);
                        accountNumber = !string.IsNullOrEmpty(accountNumber)
                            ? accountNumber : "Interno";

                        return new AccountModel
                        {
                            AccountNumber = accountNumber,
                            AccountName = item["account_name"]!,
                            AccountType = item["account_type"]!,
                            BankCurrency = string.IsNullOrEmpty(item["bank_currency"])
                                ? null : item["bank_currency"]
                        };
                    })
                .Distinct(new AccountComparer())
                .ToList();
        }


        /// <summary>
        /// Normalizes product information by filtering out invalid entries and constructing a list of ProductModel objects.
        /// </summary>
        /// <param name="originalUsersEntitlements">List of dictionaries containing user entitlement information.</param>
        /// <param name="originalProductsAccounts">List of dictionaries containing product account information.</param>
        /// <returns>A list of ProductModel objects with normalized product information.</returns>
        public List<ProductModel> NormalizeProducts(List<Dictionary<string, string?>> originalUsersEntitlements, List<Dictionary<string, string?>> originalProductsAccounts)
        {
            var data = originalUsersEntitlements.Concat(originalProductsAccounts).ToList();

            return data
                .Where(item => item.TryGetValue("product", out var productName)
                                && !string.IsNullOrEmpty(productName))
                .Select(
                    item =>
                    {
                        var product = item["product"]!.Trim().Split(", ");
                        var productName = product[0];
                        var subProduct = product.Length >= 2 ? product[1] : null;

                        return new ProductModel
                        {
                            ProductName = productName,
                            SubProduct = subProduct
                        };
                    }
                )
                .Distinct(new ProductComparer())
                .ToList();
        }


        /// <summary>
        /// Normalizes function information by filtering out invalid entries and constructing a list of FunctionModel objects.
        /// </summary>
        /// <param name="originalUsersEntitlements">List of dictionaries containing user entitlement information.</param>
        /// <returns>A list of FunctionModel objects with normalized function information.</returns>
        public List<FunctionModel> NormalizeFunctions(List<Dictionary<string, string?>> originalUsersEntitlements)
        {
            return originalUsersEntitlements
                .Where(item => item.TryGetValue("function_name", out var functionName)
                                && !string.IsNullOrEmpty(functionName))
                .Select(
                    item =>
                    {
                        string functionName = item["function_name"]!.Trim();

                        return new FunctionModel
                        {
                            FunctionName = functionName
                        };
                    }
                )
                .Distinct(new FunctionComparer())
                .ToList();
        }


        /// <summary>
        /// Normalizes raw company user data by filtering out invalid entries and constructing a list of CompanyUserModel objects.
        /// </summary>
        /// <param name="originalCompanyUsers">List of dictionaries containing raw company user information.</param>
        /// <returns>A list of CompanyUserModel objects with normalized company user information.</returns>
        public List<CompanyUserModel> NormalizeCompanyUsersRaw(List<Dictionary<string, string?>> originalCompanyUsers)
        {
            return originalCompanyUsers
                .Where(item => item.TryGetValue("accessid", out var accessId)
                                && !string.IsNullOrEmpty(accessId))
                .Select(
                    item =>
                    {
                        return new CompanyUserModel
                        {
                            AccessId = item["accessid"]!,
                            UserName = StringUtil.RemoveUnnecessarySpaces(item["user_name"]!.Trim()),
                            UserStatus = item["user_status"] == "Active",
                            UserType = item["user_type"]!,
                            EmployeeId = string.IsNullOrEmpty(item["employee_id"])
                                ? null : item["employee_id"],
                            EmailAddress = item["email_address"]!,
                            UserLocation = string.IsNullOrEmpty(item["user_location"])
                                ? null : item["user_location"],
                            UserCountry = item["user_country"]!,
                            UserLogonType = item["user_logon_type"]!,
                            UserLastLogonDt = StringUtil.ParseDateTime(item["user_last_logon_dt"]),
                            UserLogonStatus = item["user_logon_status"]!,
                            UserGroupMembership = item["user_group_membership"],
                            UserRole = item["user_role"],
                            ProfileId = ""
                        };
                    }
                )
                .Distinct(new CompanyUserComparer())
                .ToList();
        }


        /// <summary>
        /// Normalizes product accounts data and constructs a list of ProductAccountModel objects.
        /// </summary>
        /// <param name="originalProductsAccounts">List of dictionaries containing product account information.</param>
        /// <returns>A list of ProductAccountModel objects with normalized product account information.</returns>
        public List<ProductAccountModel> NormalizeProductsAccounts(List<Dictionary<string, string?>> originalProductsAccounts)
        {
            return originalProductsAccounts
                .Select(
                    item =>
                    {
                        string? accountNumber = !string.IsNullOrEmpty(item["account_number"])
                            ? item["account_number"]
                            : (item["account_type"] == "Portfolio" ? "Interno" : null);

                        return new ProductAccountModel
                        {
                            ProductId = string.IsNullOrEmpty(item["product"])
                                ? null : StringUtil.SnakeCase(item["product"]!),
                            AccountNumber = accountNumber
                        };
                    }
                )
                .Distinct(new ProductAccountComparer())
                .ToList();
        }


        /// <summary>
        /// Normalizes user entitlement data by filtering out invalid entries and constructing a list of UserEntitlementModel objects.
        /// </summary>
        /// <param name="originalUsersEntitlements">List of dictionaries containing user entitlement information.</param>
        /// <returns>A list of UserEntitlementModel objects with normalized user entitlement information.</returns>
        public List<UserEntitlementModel> NormalizeUsersEntitlements(List<Dictionary<string, string?>> originalUsersEntitlements)
        {
            return originalUsersEntitlements
                .Where(
                    item => item.TryGetValue("product", out var productId)
                            && !string.IsNullOrEmpty(productId)
                            && item.TryGetValue("accessid", out var accessId)
                            && !string.IsNullOrEmpty(accessId)
                )
                .Select(
                    item =>
                    {
                        string? accountNumber = !string.IsNullOrEmpty(item["account_number"])
                            ? item["account_number"]
                            : (item["account_type"] == "Portfolio" ? "Interno" : null);

                        return new UserEntitlementModel
                        {
                            AccessId = item["accessid"]!,
                            AccountNumber = accountNumber,
                            ProductId = StringUtil.SnakeCase(item["product"]!),
                            FunctionId = string.IsNullOrEmpty(item["function_name"])
                                ? null : StringUtil.SnakeCase(item["function_name"]!),
                            FunctionType = item["function_type"]
                        };
                    }
                )
                .Distinct(new UserEntitlementComparer())
                .ToList();
        }


        /// <summary>
        /// Retrieves user functions and associated products from normalized user entitlement data and organizes them into a dictionary.
        /// </summary>
        /// <param name="normalizedUsersEntitlements">List of normalized user entitlement data.</param>
        /// <returns>A dictionary where keys are access IDs and values are lists of FunctionProductDTO objects.</returns>
        private static Dictionary<string, List<FunctionProductDTO>> GetUsersFunctionsProducts(List<UserEntitlementModel> normalizedUsersEntitlements)
        {
            return normalizedUsersEntitlements
                .Where(item => item.AccessId != null
                                && item.ProductId != null
                                && item.FunctionId != null
                                && item.FunctionType != null)
                .GroupBy(item => item.AccessId)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(
                            item => new FunctionProductDTO
                            {
                                FunctionId = StringUtil.SnakeCase(item.FunctionId!),
                                FunctionType = item.FunctionType!.Trim(),
                                ProductId = item.ProductId.Trim()
                            }
                        )
                        .Distinct(new FunctionProductComparer())
                        .ToList()
                );
        }


        /// <summary>
        /// Retrieves the index of a company user within a list of CompanyUserModel objects based on the access ID.
        /// </summary>
        /// <param name="companyUsers">List of CompanyUserModel objects.</param>
        /// <param name="accessId">Access ID of the company user to search for.</param>
        /// <returns>The index of the company user if found; otherwise, returns -1.</returns>
        private static int GetCompanyUserIndex(List<CompanyUserModel> companyUsers, string accessId)
        {
            for (int i = 0; i < companyUsers.Count; i++)
            {
                if (companyUsers[i].AccessId == accessId)
                {
                    return i;
                }
            }
            return -1;
        }


        /// <summary>
        /// Defines user profiles for company users based on their entitlements and updates the normalized company user data accordingly.
        /// </summary>
        /// <param name="normalizedUsersEntitlements">List of normalized user entitlement data.</param>
        /// <param name="normalizedCompanyUsers">List of normalized company user data.</param>
        /// <returns>The updated list of CompanyUserModel objects with defined user profiles.</returns>
        public List<CompanyUserModel> DefineUserProfile(List<UserEntitlementModel> normalizedUsersEntitlements, List<CompanyUserModel> normalizedCompanyUsers)
        {
            HashSet<string> captureActions = new() { "cancel", "input", "amend" };
            HashSet<string> verificationActions = new() { "approve" };
            HashSet<string> authorizationActions = new() { "release" };

            Dictionary<string, List<FunctionProductDTO>> userFunctionsProducts = GetUsersFunctionsProducts(normalizedUsersEntitlements);

            foreach (var element in userFunctionsProducts)
            {
                var accessId = element.Key;
                var functionsProducts = element.Value;

                var companyUserIndex = GetCompanyUserIndex(normalizedCompanyUsers, accessId);
                if (companyUserIndex == -1)
                {
                    continue;
                }

                var companyUser = normalizedCompanyUsers[companyUserIndex];
                var functionsIds = functionsProducts.Select(fp => fp.FunctionId).ToList();
                var productsIds = functionsProducts.Select(fp => fp.ProductId).ToList();

                if (productsIds.Contains("administration"))
                {
                    companyUser.ProfileId = "administrator";
                }
                else if (captureActions.All(action => functionsIds.Contains(action)))
                {
                    companyUser.ProfileId = "capture";
                }
                else if (verificationActions.All(action => functionsIds.Contains(action)))
                {
                    companyUser.ProfileId = "verification";
                }
                else if (authorizationActions.All(action => functionsIds.Contains(action)))
                {
                    companyUser.ProfileId = "authorization";
                }
                else
                {
                    companyUser.ProfileId = "viewer";
                }
            }

            return normalizedCompanyUsers;
        }


        /// <summary>
        /// Defines the mappings between user profiles and functions based on user entitlements and updates the normalized data accordingly.
        /// </summary>
        /// <param name="normalizedUsersEntitlements">List of normalized user entitlement data.</param>
        /// <param name="normalizedCompanyUsers">List of normalized company user data.</param>
        /// <returns>The list of ProfileFunctionModel objects representing the mappings between user profiles and functions.</returns>
        public List<ProfileFunctionModel> DefineProfilesFunctions(List<UserEntitlementModel> normalizedUsersEntitlements, List<CompanyUserModel> normalizedCompanyUsers)
        {
            var userFunctionsProducts = GetUsersFunctionsProducts(normalizedUsersEntitlements);

            Dictionary<string, HashSet<string>> profilesFunctionsRaw = new()
            {
                {"capture", new HashSet<string>()},
                {"authorization", new HashSet<string>()},
                {"verification", new HashSet<string>()},
                {"administrator", new HashSet<string>()},
                {"viewer", new HashSet<string>()}
            };

            foreach (var (accessId, functionsProducts) in userFunctionsProducts)
            {
                var companyUserIndex = GetCompanyUserIndex(normalizedCompanyUsers, accessId);
                if (companyUserIndex == -1)
                {
                    continue;
                }

                var profileId = normalizedCompanyUsers[companyUserIndex].ProfileId;
                List<string> functions = functionsProducts.Select(fp => fp.FunctionId).ToList();

                foreach (var functionId in functions)
                {
                    profilesFunctionsRaw[profileId].Add(functionId);
                }
            }

            var profilesFunctionsDTO = (
                from profileId in profilesFunctionsRaw.Keys
                from functionId in profilesFunctionsRaw[profileId]
                select new ProfileFunctionModel
                {
                    ProfileId = profileId,
                    FunctionId = functionId
                }
            )
            .Distinct()
            .ToList();

            return profilesFunctionsDTO;
        }
    }
}