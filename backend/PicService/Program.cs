using Microsoft.EntityFrameworkCore;
using PicService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Register API controllers
builder.Services.AddControllers();

// Load environment variables from .env file
DotNetEnv.Env.Load(); 

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Configure PostgreSQL DbContext
builder.Services.AddDbContext<PicContext>(opt =>
    opt.UseNpgsql(
        Environment.GetEnvironmentVariable("Db__Connection")
        ?? throw new Exception("Missing database connection string"))
); 

builder.Services.AddDbContext<CommentContext>(opt =>
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

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseAuthentication();   // Enable authentication middleware
app.UseAuthorization();    // Enable authorization middleware
app.MapControllers();      // Map controller routes
app.Run();                 // Start the application
