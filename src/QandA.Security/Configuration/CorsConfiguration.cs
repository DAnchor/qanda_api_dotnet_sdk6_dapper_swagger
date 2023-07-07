using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace QandA.Security.Configuration;

public static class CorsSonfiguration
{
    public static void AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var corsDomain = configuration.GetValue<string>("Frontend:Localhost");

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .WithOrigins(corsDomain)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }
}