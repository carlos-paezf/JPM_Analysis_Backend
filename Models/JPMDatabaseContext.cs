using Microsoft.EntityFrameworkCore;


namespace BackendJPMAnalysis.Models
{
    public partial class JPMDatabaseContext : DbContext
    {
        public JPMDatabaseContext()
        {
        }

        public JPMDatabaseContext(DbContextOptions<JPMDatabaseContext> options)
            : base(options)
        {
        }


        public async Task DropTablesAsync()
        {
            await Database.ExecuteSqlRawAsync(
                "DROP TABLE IF EXISTS accounts, app_history, app_users, company_users, functions, " +
                "products, products_accounts, profiles, profiles_functions, report_history, user_entitlements;"
            );
        }

        public virtual DbSet<AccountModel> Accounts { get; set; } = null!;
        public virtual DbSet<AppHistoryModel> AppHistories { get; set; } = null!;
        public virtual DbSet<AppUserModel> AppUsers { get; set; } = null!;
        public virtual DbSet<ProductAccountModel> ProductsAccounts { get; set; } = null!;
        public virtual DbSet<CompanyUserModel> CompanyUsers { get; set; } = null!;
        public virtual DbSet<FunctionModel> Functions { get; set; } = null!;
        public virtual DbSet<ProductModel> Products { get; set; } = null!;
        public virtual DbSet<ProfileModel> Profiles { get; set; } = null!;
        public virtual DbSet<ProfileFunctionModel> ProfilesFunctions { get; set; } = null!;
        public virtual DbSet<ReportHistoryModel> ReportHistories { get; set; } = null!;
        public virtual DbSet<UserEntitlementModel> UserEntitlements { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySQL("Name=ConnectionStrings:JPMMySQL");
                // optionsBuilder.UseMySQL("Server=localhost;Uid=root;Pwd=1234;Database=jpm_analysis_database");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountModel>(entity =>
            {
                entity.HasKey(e => e.AccountNumber)
                    .HasName("PRIMARY");

                entity.ToTable("accounts");

                entity.HasIndex(e => new { e.AccountNumber, e.AccountName }, "accounts_index_2")
                    .IsUnique();

                entity.Property(e => e.AccountNumber)
                    .HasColumnName("account_number")
                    .HasComment("Los valores que sean null se reemplazan por un 0");

                entity.Property(e => e.AccountName).HasColumnName("account_name");

                entity.Property(e => e.AccountType)
                    .HasMaxLength(255)
                    .HasColumnName("account_type");

                entity.Property(e => e.BankCurrency)
                    .HasMaxLength(255)
                    .HasColumnName("bank_currency");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<AppHistoryModel>(entity =>
            {
                entity.ToTable("app_history");

                entity.HasIndex(e => e.AppUserId, "app_user_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("Autoincremental");

                entity.Property(e => e.AppFunction)
                    .HasColumnType("enum('upload_fill','upload_update','add','delete','compare')")
                    .HasColumnName("app_function");

                entity.Property(e => e.AppTable)
                    .HasColumnType("enum('profiles','sheets','functions','profiles_functions','company_users','user_entitlements','products_accounts','accounts','tim_cash','tim_fx','tim_listed_sec','tim_import_template')")
                    .HasColumnName("app_table");

                entity.Property(e => e.AppUserId).HasColumnName("app_user_id");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.AppHistories)
                    .HasForeignKey(d => d.AppUserId)
                    .HasConstraintName("app_history_ibfk_1");
            });

            modelBuilder.Entity<AppUserModel>(entity =>
            {
                entity.ToTable("app_users");

                entity.HasIndex(e => e.Username, "username")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("Autoincremental");

                entity.Property(e => e.AppRole)
                    .HasColumnType("enum('admin','basic')")
                    .HasColumnName("app_role")
                    .HasDefaultValueSql("'_utf8mb4\\\\''basic\\\\'''");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Username).HasColumnName("username");
            });

            modelBuilder.Entity<ProductAccountModel>(entity =>
            {
                entity.ToTable("products_accounts");

                entity.HasIndex(e => e.AccountNumber, "account_number");

                entity.HasIndex(e => new { e.ProductId, e.AccountNumber }, "products_accounts_index_1")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("Autoincremental");

                entity.Property(e => e.AccountNumber).HasColumnName("account_number");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.ProductsAccounts)
                    .HasForeignKey(d => d.AccountNumber)
                    .HasConstraintName("products_accounts_ibfk_1");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductsAccounts)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("products_accounts_ibfk_2");
            });

            modelBuilder.Entity<CompanyUserModel>(entity =>
            {
                entity.HasKey(e => e.AccessId)
                    .HasName("PRIMARY");

                entity.ToTable("company_users");

                entity.HasIndex(e => e.ProfileId, "profile_id");

                entity.Property(e => e.AccessId).HasColumnName("access_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.EmailAddress)
                    .HasMaxLength(255)
                    .HasColumnName("email_address");

                entity.Property(e => e.EmployeeId)
                    .HasMaxLength(255)
                    .HasColumnName("employee_id");

                entity.Property(e => e.ProfileId).HasColumnName("profile_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCountry)
                    .HasMaxLength(255)
                    .HasColumnName("user_country");

                entity.Property(e => e.UserGroupMembership)
                    .HasMaxLength(255)
                    .HasColumnName("user_group_membership");

                entity.Property(e => e.UserLastLogonDt)
                    .HasColumnType("datetime")
                    .HasColumnName("user_last_logon_dt")
                    .HasComment("Se debe hacer una conversión");

                entity.Property(e => e.UserLocation)
                    .HasMaxLength(255)
                    .HasColumnName("user_location");

                entity.Property(e => e.UserLogonStatus)
                    .HasMaxLength(255)
                    .HasColumnName("user_logon_status");

                entity.Property(e => e.UserLogonType)
                    .HasMaxLength(255)
                    .HasColumnName("user_logon_type")
                    .HasComment("RSA Token o Password");

                entity.Property(e => e.UserName)
                    .HasMaxLength(255)
                    .HasColumnName("user_name");

                entity.Property(e => e.UserRole)
                    .HasMaxLength(255)
                    .HasColumnName("user_role");

                entity.Property(e => e.UserStatus).HasColumnName("user_status");

                entity.Property(e => e.UserType)
                    .HasMaxLength(255)
                    .HasColumnName("user_type");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.CompanyUsers)
                    .HasForeignKey(d => d.ProfileId)
                    .HasConstraintName("company_users_ibfk_1");
            });

            modelBuilder.Entity<FunctionModel>(entity =>
            {
                entity.ToTable("functions");

                entity.HasIndex(e => e.FunctionName, "function_name")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("Function name in snake_case");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.FunctionName).HasColumnName("function_name");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<ProductModel>(entity =>
            {
                entity.ToTable("products");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("Product name in snake_case");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(255)
                    .HasColumnName("product_name");

                entity.Property(e => e.SubProduct)
                    .HasMaxLength(255)
                    .HasColumnName("sub_product");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<ProfileModel>(entity =>
            {
                entity.ToTable("profiles");

                entity.HasIndex(e => e.ProfileName, "profile_name")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("Profile name in snake_case");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ProfileName).HasColumnName("profile_name");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<ProfileFunctionModel>(entity =>
            {
                entity.ToTable("profiles_functions");

                entity.HasIndex(e => e.FunctionId, "function_id");

                entity.HasIndex(e => new { e.ProfileId, e.FunctionId }, "profiles_functions_index_0")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("Autoincremental");

                entity.Property(e => e.FunctionId).HasColumnName("function_id");

                entity.Property(e => e.ProfileId).HasColumnName("profile_id");

                entity.HasOne(d => d.Function)
                    .WithMany(p => p.ProfilesFunctions)
                    .HasForeignKey(d => d.FunctionId)
                    .HasConstraintName("profiles_functions_ibfk_2");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfilesFunctions)
                    .HasForeignKey(d => d.ProfileId)
                    .HasConstraintName("profiles_functions_ibfk_1");
            });

            modelBuilder.Entity<ReportHistoryModel>(entity =>
            {
                entity.ToTable("report_history");

                entity.HasIndex(e => e.AppUserId, "app_user_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.AppUserId).HasColumnName("app_user_id");

                entity.Property(e => e.ReportComments)
                    .HasMaxLength(500)
                    .HasColumnName("report_comments");

                entity.Property(e => e.ReportName)
                    .HasMaxLength(255)
                    .HasColumnName("report_name");

                entity.Property(e => e.RunReportDate)
                    .HasColumnType("datetime")
                    .HasColumnName("run_report_date");

                entity.Property(e => e.ReportUploadDate)
                    .HasColumnType("datetime")
                    .HasColumnName("report_upload_date")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.ReportHistories)
                    .HasForeignKey(d => d.AppUserId)
                    .HasConstraintName("report_history_ibfk_1");
            });

            modelBuilder.Entity<UserEntitlementModel>(entity =>
            {
                entity.ToTable("user_entitlements");

                entity.HasIndex(e => e.AccessId, "access_id");

                entity.HasIndex(e => e.AccountNumber, "account_number");

                entity.HasIndex(e => e.FunctionId, "function_id");

                entity.HasIndex(e => e.ProductId, "product_id");

                entity.Property(e => e.Id)
                    .HasMaxLength(255)
                    .HasColumnName("id");

                entity.Property(e => e.AccessId).HasColumnName("access_id");

                entity.Property(e => e.AccountNumber).HasColumnName("account_number");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.FunctionId).HasColumnName("function_id");

                entity.Property(e => e.FunctionType)
                    .HasMaxLength(255)
                    .HasColumnName("function_type");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.CompanyUser)
                    .WithMany(p => p.UserEntitlements)
                    .HasForeignKey(d => d.AccessId)
                    .HasConstraintName("user_entitlements_ibfk_1");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.UserEntitlements)
                    .HasForeignKey(d => d.AccountNumber)
                    .HasConstraintName("user_entitlements_ibfk_2");

                entity.HasOne(d => d.Function)
                    .WithMany(p => p.UserEntitlements)
                    .HasForeignKey(d => d.FunctionId)
                    .HasConstraintName("user_entitlements_ibfk_4");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.UserEntitlements)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("user_entitlements_ibfk_3");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
