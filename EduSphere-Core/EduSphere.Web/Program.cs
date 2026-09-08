using System.Text;
using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.Interfaces;
using EduSphere.Application.Validators;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;
using EduSphere.Infrastructure;
using EduSphere.Web.Authorization;
using EduSphere.Web.Data;
using EduSphere.Web.Middleware;
using EduSphere.Web.Security;
using EduSphere.Web.Services;
using EduSphere.Web.Swagger;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var dataProtectionKeysPath = builder.Configuration["DataProtection:KeysPath"];
if (builder.Environment.IsDevelopment() || !string.IsNullOrWhiteSpace(dataProtectionKeysPath))
{
    dataProtectionKeysPath = string.IsNullOrWhiteSpace(dataProtectionKeysPath)
        ? Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtectionKeys")
        : dataProtectionKeysPath;

    if (!Path.IsPathRooted(dataProtectionKeysPath))
        dataProtectionKeysPath = Path.Combine(builder.Environment.ContentRootPath, dataProtectionKeysPath);

    Directory.CreateDirectory(dataProtectionKeysPath);
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath))
        .SetApplicationName("EduSphere");
}

// ---- Presentation ----
builder.Services.AddRazorPages();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

// Wrap model-state validation failures in the standard ApiResponse envelope.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(kvp => kvp.Value?.Errors.Count > 0)
            .SelectMany(kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage))
            .ToList();
        return new BadRequestObjectResult(ApiResponse<object>.Fail(errors));
    };
});

// ---- Persistence, application services, tenancy ----
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();
builder.Services.AddScoped<IBranchAccessService, BranchAccessService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

// ---- Validation ----
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateTenantRequestValidator>();

// ---- API versioning ----
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ---- Identity (cookie) ----
builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
        // Existing seeded/demo users remain usable; new admin-created users are
        // blocked explicitly by RequiresActivation + EmailConfirmed checks.
        options.SignIn.RequireConfirmedAccount = false;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    })
    .AddEntityFrameworkStores<TenantDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromDays(7);
});

// ---- JWT bearer (for the /api/v1 surface) ----
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwt["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured."))),
            ValidateLifetime = true,
            RoleClaimType = "role",
            NameClaimType = "email",
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.SuperAdmin, p => p.RequireRole(Roles.SuperAdmin));
    options.AddPolicy(AuthorizationPolicies.TenantAdmin, p => p.RequireRole(Roles.SuperAdmin, Roles.TenantAdmin));
    options.AddPolicy(AuthorizationPolicies.BranchAdmin, p => p.RequireRole(
        Roles.SuperAdmin,
        Roles.TenantAdmin,
        Roles.BranchAdmin));
    options.AddPolicy(AuthorizationPolicies.AttendanceMarker, p => p.RequireRole(
        Roles.SuperAdmin,
        Roles.TenantAdmin,
        Roles.BranchAdmin,
        Roles.Principal,
        Roles.Teacher));
    options.AddPolicy(AuthorizationPolicies.FinanceManager, p => p.RequireRole(
        Roles.SuperAdmin,
        Roles.TenantAdmin,
        Roles.BranchAdmin,
        Roles.Accountant));
    options.AddPolicy(AuthorizationPolicies.TransportManager, p => p.RequireRole(
        Roles.SuperAdmin,
        Roles.TenantAdmin,
        Roles.BranchAdmin,
        Roles.TransportManager));
    options.AddPolicy(AuthorizationPolicies.LibraryManager, p => p.RequireRole(
        Roles.SuperAdmin,
        Roles.TenantAdmin,
        Roles.BranchAdmin,
        Roles.Librarian));
    options.AddPolicy(AuthorizationPolicies.HostelManager, p => p.RequireRole(
        Roles.SuperAdmin,
        Roles.TenantAdmin,
        Roles.BranchAdmin,
        Roles.HostelManager));
    options.AddPolicy(AuthorizationPolicies.CommunicationManager, p => p.RequireRole(
        Roles.SuperAdmin,
        Roles.TenantAdmin,
        Roles.BranchAdmin,
        Roles.Principal,
        Roles.StaffAdmin));
    options.AddPolicy(AuthorizationPolicies.UserManager, p => p.RequireRole(
        Roles.SuperAdmin,
        Roles.TenantAdmin,
        Roles.BranchAdmin,
        Roles.Principal,
        Roles.DepartmentAdmin,
        Roles.StaffAdmin));
});

// ---- OpenAPI / Swagger ----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "EduSphere API", Version = "v1" });

    var bearerScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste the JWT returned by POST /api/v1/auth/login.",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    options.AddSecurityDefinition("Bearer", bearerScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { { bearerScheme, Array.Empty<string>() } });
    options.OperationFilter<TenantHeaderOperationFilter>();
});

var app = builder.Build();

await app.Services.ApplyDatabaseMigrationStrategyAsync();

// ---- Pipeline ----
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "EduSphere API v1"));
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

// Resolve the tenant once per request (after auth, before authorization).
app.UseMiddleware<TenantResolutionMiddleware>();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

// Optional first-run seeding (roles + a SuperAdmin). Off by default and guarded so a
// missing database never blocks startup; enable with "SeedData": true and a reachable DB.
if (app.Configuration.GetValue<bool>("SeedData"))
{
    using var scope = app.Services.CreateScope();
    try
    {
        await IdentityDataSeeder.SeedAsync(scope.ServiceProvider, app.Configuration);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Identity data seeding failed.");
    }
}

app.Run();
