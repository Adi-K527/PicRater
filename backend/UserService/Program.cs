using Microsoft.EntityFrameworkCore;
using UserService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Register API controllers
builder.Services.AddControllers();

// Add health checks
builder.Services.AddHealthChecks();

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        // Allow all origins in container environment, can be restricted later
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Load environment variables from .env file (only in development)
if (builder.Environment.IsDevelopment())
{
    DotNetEnv.Env.Load(); 
}

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Configure PostgreSQL DbContext
builder.Services.AddDbContext<UserContext>(opt =>
    opt.UseNpgsql(
        Environment.GetEnvironmentVariable("Db__Connection") 
        ?? throw new Exception("Missing database connection string"))
); 

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,          // Not validating issuer
        ValidateAudience = false,        // Not validating audience
        ValidateLifetime = true,         // Validate token expiration
        ValidateIssuerSigningKey = true, // Validate signature
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Secret"] 
                ?? Environment.GetEnvironmentVariable("Jwt__Secret") 
                ?? throw new Exception("Missing JWT secret"))
        )
    };
});

builder.Services.AddAuthorization(); // Add authorization services

var app = builder.Build();

// Enable CORS middleware
app.UseCors("AllowReactApp");

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();   // Enable authentication middleware
app.UseAuthorization();    // Enable authorization middleware

// Add health check endpoint
app.MapHealthChecks("/health");

app.MapControllers();      // Map controller routes
app.Run();                 // Start the application
