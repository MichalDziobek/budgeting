﻿using Application.Abstractions;
using Application.Abstractions.Persistence;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Postgres")), ServiceLifetime.Transient,
            ServiceLifetime.Singleton);

        services.AddTransient<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddTransient<ApplicationDbContextInitializer>();
        services.AddTransient<IUsersRepository, UsersRepository>();
        services.AddTransient<IBudgetsRepository, BudgetsRepository>();
        services.AddTransient<IBudgetEntriesRepository, BudgetEntriesRepository>();
        services.AddTransient<ICategoriesRepository, CategoriesRepository>();
        
        
        return services;
    }
}