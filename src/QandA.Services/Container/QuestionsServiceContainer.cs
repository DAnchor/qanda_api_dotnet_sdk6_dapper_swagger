using Microsoft.Extensions.Logging;
using QandA.DataAccess.Repositories;
using QandA.DTOs.Payload.Request;
using QandA.DTOs.Payload.Response;

namespace QandA.Services.Container;

public interface IQuestionsServiceContainer : IDataRepository
{

}

public class QuestionsServiceContainer : IQuestionsServiceContainer
{
    private readonly IDataRepository _repository;
    private readonly ILogger<QuestionsServiceContainer> _logger;

    public QuestionsServiceContainer(IDataRepository repository, ILogger<QuestionsServiceContainer> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<QuestionGetManyResponse>> GetQuestionsAsync
    (
        int pageNumber,
        int pageSize
    )
    {
        try
        {
            return await _repository.GetQuestionsAsync(pageNumber, pageSize);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionGetManyResponse>> GetQuestionsWithAnswersAsync()
    {
        return await _repository.GetQuestionsWithAnswersAsync();
    }

    public async Task<IEnumerable<QuestionGetManyResponse>> GetQuestionBySearchAsync
    (
        string search,
        int pageNumber,
        int pageSize
    )
    {
        try
        {
            return await _repository.GetQuestionBySearchAsync(search, pageNumber, pageSize);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionGetManyResponse>> GetUnansweredQuestionsAsync
    (
        int pageNumber,
        int pageSize
    )
    {
        try
        {
            return await _repository.GetUnansweredQuestionsAsync(pageNumber, pageSize);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<QuestionGetSingleResponse> GetQuestionAsync(int quesitonId)
    {
        try
        {
            return await _repository.GetQuestionAsync(quesitonId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<bool> QuestionExistsAsync(int quesitonId)
    {
        try
        {
            return await _repository.QuestionExistsAsync(quesitonId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<AnswerGetResponse> GetAnswerAsync(int answerId)
    {
        try
        {
            return await _repository.GetAnswerAsync(answerId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<QuestionGetSingleResponse> PostFullQuestionAsync(QuestionPostFullRequest request)
    {
        try
        {
            return await _repository.PostFullQuestionAsync(request);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<QuestionGetSingleResponse> PutQuestionAsync(int questionId, QuestionPutRequest question)
    {
        try
        {
            return await _repository.PutQuestionAsync(questionId, question);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task DeleteQuestionAsync(int questionId)
    {
        try
        {
            await _repository.DeleteQuestionAsync(questionId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<AnswerGetResponse> PostFullAnswerAsync(AnswerPostFullRequest request)
    {
        try
        {
            return await _repository.PostFullAnswerAsync(request);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}