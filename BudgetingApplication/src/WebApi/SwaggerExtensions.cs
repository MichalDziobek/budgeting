using Microsoft.OpenApi.Models;

namespace WebApi;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerService(this IServiceCollection services, string? authority, string? audience)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Budgeting Demo app",
                Version = "v1.0.0"
            });

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                BearerFormat = "JWT",
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode  = new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri($"{authority}oauth/token"),
                        AuthorizationUrl = new Uri($"{authority}authorize?audience={audience}"),
                        // Scopes = new Dictionary<string, string>
                        // {
                        //     { "openid", "OpenId" },
                        // }
                    }
                }
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                    },
                    new[] { "openid" }
                }
            });
        });

        return services;
    }
}