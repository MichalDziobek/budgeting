using Application.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApi.Services;

namespace WebApi;

public static class ConfigureServices
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        
        var authority = configuration["Authentication:Schemes:Bearer:Authority"];
        var audience = configuration["Authentication:Schemes:Bearer:Audience"];
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;

            });

        services.AddSwaggerService(authority, audience);

        services.AddHttpContextAccessor();
        services.AddTransient<ICurrentUserService, CurrentUserService>();
        
        // services.AddSwaggerGen();
        return services;
    }
}