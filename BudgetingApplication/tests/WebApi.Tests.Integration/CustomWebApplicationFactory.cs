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

namespace WebApi.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<IApiMarker>
{
    public HttpClient HttpClient { get; private set; } = default!;
    public ICurrentUserService CurrentUserService { get; private set; } = default!;
    
    private string _connectionString = string.Empty;
    private Respawner _respawner = default!;
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
                        optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Database.EnsureDeleted();
            dbContext.Database.Migrate();
            _dbConnection = new NpgsqlConnection(_connectionString);
            _dbConnection.Open();
            
            _respawner = Respawner.CreateAsync(_dbConnection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" }
            }).GetAwaiter().GetResult();
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
        await _dbConnection.CloseAsync();
        await _dbConnection.DisposeAsync();
    }

}