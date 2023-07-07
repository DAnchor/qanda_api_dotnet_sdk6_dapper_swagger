using Microsoft.Extensions.DependencyInjection;
using QandA.DataAccess.Repositories.CachedQuestion;

namespace QandA.DataAccess.Configurations;

public static class MemoryCacheConfiguration
{
    public static void AddMemoryCacheConfiguration(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<IQuestionCache, QuestionCache>();
    }
}