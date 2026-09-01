using EduSphere.Application.Interfaces;
using EduSphere.Application.Services;
using EduSphere.Domain.Interfaces;
using EduSphere.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EduSphere.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IBranchService, BranchService>();
        services.AddScoped<IUserService, UserService>();
        
        // Identity services
        services.AddScoped<IPasswordHasher<Domain.Entities.User>, PasswordHasher<Domain.Entities.User>>();
        
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
    {
        // Register DbContext factory
        services.AddDbContextFactory<TenantDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
        
        // Register repositories and UnitOfWork
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Register tenant context resolver
        services.AddScoped<ITenantDbContextResolver, TenantDbContextResolver>();
        
        return services;
    }
}
