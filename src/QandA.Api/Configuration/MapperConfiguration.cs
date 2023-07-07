using Microsoft.Extensions.DependencyInjection;
using QandA.Api.Configuration.MapperOptions;

namespace QandA.Api.Configuration;

public static class MapperConfiguration
{
    public static void AddMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(QuestionsProfile).Assembly);
    }
}