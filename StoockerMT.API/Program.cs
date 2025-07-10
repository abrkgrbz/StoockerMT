using Microsoft.AspNetCore.HttpsPolicy;
using StoockerMT.API.Extensions;
using StoockerMT.Application;
using StoockerMT.Identity;
using StoockerMT.Identity.Extensions;
using StoockerMT.Identity.Middleware;
using StoockerMT.Infrastructure;
using StoockerMT.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Docker.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);

    if (!builder.Environment.IsEnvironment("Docker") && !IsRunningInDocker())
    {
        options.ListenAnyIP(443, listenOptions =>
        {
            listenOptions.UseHttps();
        });
    }
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();  
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{ 
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration); // This adds tenant services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddHealthChecksExtension();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
var app = builder.Build();
 
if (app.Environment.IsDevelopment()  )
{ 
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}


app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.TenantMiddlewareExtensions();

app.UseJwtMiddleware();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHealthChecksExtension();
 
app.Run();
bool IsRunningInDocker()
{
    return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" ||
           File.Exists("/.dockerenv");
}