using System.Data.Common;
using Application.Abstractions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Respawn;
using WebApi.Tests.Integration.Common.Abstractions;

namespace WebApi.Tests.Integration.Common;

public class CustomWebApplicationFactory : WebApplicationFactory<IApiMarker>
{
    public ICurrentUserService CurrentUserService { get; private set; } = default!;
    
    private string _connectionString = string.Empty;
    private DbConnection _dbConnection = default!;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            var integrationConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            configurationBuilder.AddConfiguration(integrationConfig);
        });

        builder.ConfigureServices((webHostBuilderContext, services) =>
        {
            CurrentUserService = Substitute.For<ICurrentUserService>();
            CurrentUserService.UserId.ReturnsNull();
            services
                .Remove<ICurrentUserService>()
                .AddTransient(_ => CurrentUserService);
            
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                    options.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });

            _connectionString = webHostBuilderContext.Configuration.GetConnectionString("Postgres") ?? string.Empty;
            services
                .Remove<DbContextOptions<ApplicationDbContext>>()
                .AddDbContext<ApplicationDbContext>((sp, options) =>
                    options.UseNpgsql(_connectionString,
                        optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)),
                    ServiceLifetime.Transient, ServiceLifetime.Singleton);

            var serviceProvider = services.BuildServiceProvider();
            
            using var scope = serviceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            

            dbContext.Database.EnsureDeleted();
            dbContext.Database.Migrate();
            _dbConnection = new NpgsqlConnection(_connectionString);
            _dbConnection.Open();
            
            var respawner = Respawner.CreateAsync(_dbConnection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" }
            }).GetAwaiter().GetResult();

            services.AddTransient(_ => respawner);
            services.AddTransient<ITestDatabase, TestDatabase>();
        });
    }

    public ITestDatabase GetTestRepository() => Services.GetRequiredService<ITestDatabase>();
}