using EduSphere.Domain.Interfaces;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register configuration
builder.Services.AddSingleton(builder.Configuration);

// Register TenantDbContextResolver
builder.Services.AddScoped<ITenantDbContextResolver, TenantDbContextResolver>();

// Register DbContext factory for tenant resolution
builder.Services.AddDbContextFactory<TenantDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
    options.UseNpgsql(connectionString);
});

// Register repositories and UnitOfWork
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();