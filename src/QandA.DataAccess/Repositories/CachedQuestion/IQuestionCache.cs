using QandA.DTOs.Payload.Response;

namespace QandA.DataAccess.Repositories.CachedQuestion;

public interface IQuestionCache
{
    Task<QuestionGetSingleResponse?> Get(int questionId);
    Task Remove(int questionId);
    Task Set(QuestionGetSingleResponse question);
}