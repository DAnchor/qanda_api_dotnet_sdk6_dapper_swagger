using QandA.DTOs.Payload.Request;
using QandA.DTOs.Payload.Response;

namespace QandA.DataAccess.Repositories;

public interface IDataRepository
{
    Task<IEnumerable<QuestionGetManyResponse>> GetQuestionsAsync(int pageNumber, int pageSize);
    Task<IEnumerable<QuestionGetManyResponse>> GetQuestionsWithAnswersAsync();
    Task<IEnumerable<QuestionGetManyResponse>> GetQuestionBySearchAsync
    (
        string search,
        int pageNumber,
        int pageSize
    );
    Task<IEnumerable<QuestionGetManyResponse>> GetUnansweredQuestionsAsync
    (
        int pageNumber,
        int pageSize
    );
    Task<QuestionGetSingleResponse> GetQuestionAsync(int questionId);
    Task<bool> QuestionExistsAsync(int questionId);
    Task<AnswerGetResponse> GetAnswerAsync(int answerId);
    Task<QuestionGetSingleResponse> PostFullQuestionAsync(QuestionPostFullRequest question);
    Task<QuestionGetSingleResponse> PutQuestionAsync(int questionId, QuestionPutRequest question);
    Task DeleteQuestionAsync(int questionId);
    Task<AnswerGetResponse> PostFullAnswerAsync(AnswerPostFullRequest answer);
}
