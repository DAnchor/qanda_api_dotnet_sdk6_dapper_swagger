using Microsoft.Extensions.Caching.Memory;
using QandA.DTOs.Payload.Response;

namespace QandA.DataAccess.Repositories.CachedQuestion;

public class QuestionCache : IQuestionCache
{
    private MemoryCache _cache { get; set; }

    public QuestionCache()
    {
        _cache = new MemoryCache(new MemoryCacheOptions
        {
            SizeLimit = 100
        });
    }

    public async Task<QuestionGetSingleResponse?> Get(int questionId)
    {
        QuestionGetSingleResponse question;

        _cache.TryGetValue<QuestionGetSingleResponse>(GetCacheKey(questionId), out question);

        return await Task.Run(() => question);
    }

    public Task Remove(int questionId)
    {
        _cache.Remove(GetCacheKey(questionId));

        return Task.CompletedTask;
    }

    public Task Set(QuestionGetSingleResponse question)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1);

        _cache.Set<QuestionGetSingleResponse>(GetCacheKey(question.QuestionId), question, cacheEntryOptions);

        return Task.CompletedTask;
    }

    private string GetCacheKey(int questionId) => $"Question-{questionId}";
}
