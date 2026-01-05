using Infrastructure.Persistence;
using Infrastructure.Security;
using Infrastructure.Authorization;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Application.Customers;
using Application.Auth;
using Application.Portfolios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Domain.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger to support JWT
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your JWT token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add Controllers
builder.Services.AddControllers();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Configure JWT Settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
if (jwtSettings == null)
    throw new InvalidOperationException("JWT Settings are not configured properly");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ClockSkew = TimeSpan.Zero // Remove default 5 minute clock skew
    };
});

// Add Authorization with Role-Based Policies
builder.Services.AddAuthorization(options =>
{
    // Exact role policies
    options.AddPolicy(RolePolicies.CustomerOnly, policy =>
        policy.Requirements.Add(new RoleRequirement(new[] { Role.Customer }, isHierarchical: false)));
    
    options.AddPolicy(RolePolicies.AdminOnly, policy =>
        policy.Requirements.Add(new RoleRequirement(new[] { Role.Admin }, isHierarchical: false)));
    
    options.AddPolicy(RolePolicies.SuperAdminOnly, policy =>
        policy.Requirements.Add(new RoleRequirement(new[] { Role.SuperAdmin }, isHierarchical: false)));
    
    // Hierarchical policies (user must be at least this role)
    options.AddPolicy(RolePolicies.RequireCustomer, policy =>
        policy.Requirements.Add(new RoleRequirement(new[] { Role.Customer }, isHierarchical: true)));
    
    options.AddPolicy(RolePolicies.RequireAdmin, policy =>
        policy.Requirements.Add(new RoleRequirement(new[] { Role.Admin }, isHierarchical: true)));
    
    options.AddPolicy(RolePolicies.RequireSuperAdmin, policy =>
        policy.Requirements.Add(new RoleRequirement(new[] { Role.SuperAdmin }, isHierarchical: true)));
});

// Register Authorization Handler
builder.Services.AddSingleton<IAuthorizationHandler, RoleRequirementHandler>();

// Add HttpContextAccessor for authorization service
builder.Services.AddHttpContextAccessor();

// Add Application Services
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IPortfolioService, PortfolioService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<Infrastructure.Authorization.IAuthorizationService, Infrastructure.Authorization.AuthorizationService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"=== CONNECTION STRING: {connectionString} ===");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.EnableSensitiveDataLogging(); // For debugging
    options.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information); // Log SQL
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add Authentication & Authorization middleware (ORDER MATTERS!)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "Hello World");

app.Run();
