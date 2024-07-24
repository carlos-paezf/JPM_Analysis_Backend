using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using BackendJPMAnalysis.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


public class Startup
{
    public IConfiguration Configuration { get; }

    public string CORSOrigins = "AllowAnyOriginPolicy";

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }


    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(
            options =>
            {
                options.AddPolicy(
                    CORSOrigins,
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                );
            }
        );

        // Configure the naming strategy for properties in response and request
        services.AddControllers()
                .AddJsonOptions(
                    options =>
                        {
                            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                        }
                );

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();

        //  Enable middleware to serve swagger-ui. Configure metadata and documentation for each endpoint.
        services.AddSwaggerGen(
                    options =>
                    {
                        options.SwaggerDoc(
                            "v1", new OpenApiInfo
                            {
                                Version = "v1",
                                Title = "JPM Analysis - Backend",
                                Description = "Endpoints Guide",
                                Contact = new OpenApiContact
                                {
                                    Name = "Carlos David Páez Ferreira",
                                    Url = new Uri("https://github.com/carlos-paezf")
                                }
                            }
                        );

                        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                    }
                );

        // Register de database connection context. Initially, it will be with MySQL, then with SQL Server
        services.AddDbContext<JPMDatabaseContext>(
                    options => options.UseMySQL(Configuration.GetConnectionString("JPMMySQL"))
                );

        // Add Mapper configuration between models and DTOs
        services.AddAutoMapper(typeof(AutoMapperProfile));

        // Register custom services
        services.AddScoped<ErrorHandlingService>();
        services.AddScoped<AccountService>();
        services.AddScoped<ProductAccountService>();
        services.AddScoped<CompanyUserService>();
        services.AddScoped<FunctionService>();
        services.AddScoped<ProductService>();
        services.AddScoped<ProfileService>();
        services.AddScoped<ProfileFunctionService>();
        services.AddScoped<UserEntitlementService>();
        services.AddScoped<DataAccessService>();
        services.AddScoped<ExcelProcessingService>();
        services.AddScoped<NormalizeEntitiesService>();
        services.AddScoped<BulkSeedService>();
        services.AddScoped<CompareSeedService>();
        services.AddScoped<ReportValidationsService>();
        services.AddScoped<AdminAppService>();
        services.AddScoped<DepartmentService>();
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors(CORSOrigins);

        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}