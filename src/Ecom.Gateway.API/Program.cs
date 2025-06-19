using Serilog;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/gateway-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Health Checks with dependencies
var healthChecks = builder.Services.AddHealthChecks()
    .AddCheck(
        "self",
        () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy()
    );

// Add service health checks only in production or if services are available
if (!builder.Environment.IsDevelopment())
{
    healthChecks
        .AddUrlGroup(new Uri("http://ecom.core.api:8080/health"), "core-api", tags: new[] { "services" })
        .AddUrlGroup(new Uri("http://ecom.users.api:8080/health"), "users-api", tags: new[] { "services" });
}
else
{
    // In development, try to connect to local services if available
    try
    {
        healthChecks
            .AddUrlGroup(new Uri("http://localhost:5001/health"), "core-api-local", tags: new[] { "services" })
            .AddUrlGroup(new Uri("http://localhost:5002/health"), "users-api-local", tags: new[] { "services" });
    }
    catch (Exception ex)
    {
        Log.Warning("Could not add service health checks: {Message}", ex.Message);
    }
}

// Add HTTP client
builder.Services.AddHttpClient();

// Add YARP
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "your-256-bit-secret-key-here-make-it-secure";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "moclaw-gateway";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "moclaw-api";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Map health check endpoint
app.MapHealthChecks("/health");

// Map controllers
app.MapControllers();

// Map YARP routes
app.MapReverseProxy();

try
{
    var urls = app.Configuration["ASPNETCORE_URLS"] ?? "http://localhost:5000;https://localhost:5001";
    Log.Information("Starting Ecommerce Gateway API with URLs: {Urls}", urls);
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Gateway API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
