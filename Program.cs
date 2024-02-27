using BackendJPMAnalysis.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);


// Configure the naming strategy for properties in response and request
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


//  Enable middleware to serve swagger-ui. Configure metadata and documentation for each endpoint.
builder.Services.AddSwaggerGen(
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
builder.Services.AddDbContext<JPMDatabaseContext>(
    options => options.UseMySQL(builder.Configuration.GetConnectionString("JPMMySQL"))
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
