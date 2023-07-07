using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QandA.DTOs.Payload.Request;
using QandA.DTOs.Payload.Response;

namespace QandA.DataAccess.Repositories;

public class DataRepository : IDataRepository
{
    private readonly ILogger<DataRepository> _logger;
    private readonly string _connectionString;

    public DataRepository(IConfiguration configuration, ILogger<DataRepository> logger)
    {
        _connectionString = configuration.GetSection("ConnectionStrings:0:DefaultConenction").Value
            ?? throw new ArgumentNullException(nameof(configuration));
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
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<QuestionGetManyResponse>
                (
                    @"EXEC dbo.Question_GetMany_WithPaging
                    @PageNumber = @PageNumber, 
                    @PageSize = @PageSize",
                    new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                );
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionGetManyResponse>> GetQuestionsWithAnswersAsync()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var questionsDictionary = new Dictionary<int, QuestionGetManyResponse>();

            var result = connection
                .QueryAsync<QuestionGetManyResponse, AnswerGetResponse, QuestionGetManyResponse>
                (
                    @"EXEC dbo.Question_GetMany_WithAnswers",
                    map: (q, a) =>
                    {
                        QuestionGetManyResponse question;

                        if (!questionsDictionary.TryGetValue(q.QuestionId, out question))
                        {
                            question = q;
                            question.Answers = new List<AnswerGetResponse>();

                            questionsDictionary.Add(question.QuestionId, question);
                        }

                        question.Answers.Add(a);

                        return question;
                    },

                    splitOn: "QuestionId"
                ).Result.Distinct().ToList();

            return await Task.Run(() => result);
        }
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
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<QuestionGetManyResponse>
                (
                    @"EXEC dbo.Question_GetMany_BySearch_WithPaging 
                    @Search = @Search,  
                    @PageNumber = @PageNumber, 
                    @PageSize = @PageSize",
                    new
                    {
                        Search = search,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                );
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionGetManyResponse>> GetUnansweredQuestionsAsync(int pageNumber, int pageSize)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<QuestionGetManyResponse>
                (
                    @"EXEC dbo.Question_GetUnanswered_WithPaging
                    @PageNumber = @PageNumber, 
                    @PageSize = @PageSize",
                    new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                );
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<QuestionGetSingleResponse> GetQuestionAsync(int questionId)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var results = await connection.QueryMultipleAsync
                (
                    @"EXEC dbo.Question_GetSingle
                    @QuestionId = @QuestionId
                    EXEC dbo.Answer_Get_ByQuestionId
                    @QuestionId = @QuestionId",
                    new { QuestionId = questionId }
                ))
                {
                    var question = results
                        .Read<QuestionGetSingleResponse>()
                        .FirstOrDefault();

                    if (question != null)
                    {
                        question.Answers = results.Read<AnswerGetResponse>().ToList();
                    }

                    return question;
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<bool> QuestionExistsAsync(int questionId)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryFirstAsync<bool>
                (
                    @"EXEC dbo.Question_Exists @QuestionId = @QuestionId",
                    new { QuestionId = questionId }
                );
            }
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
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryFirstOrDefaultAsync<AnswerGetResponse>
                (
                    @"EXEC dbo.Answer_Get_ByAnswerId @AnswerId = @AnswerId",
                    new { AnswerId = answerId }
                );
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public virtual async Task<QuestionGetSingleResponse> PostFullQuestionAsync(QuestionPostFullRequest question)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var questionId = await connection.QueryFirstAsync<int>
                (
                    @"EXEC dbo.Question_Post
                    @Title = @Title,
                    @Content = @Content,
                    @UserName = @UserName,
                    @UserId = @UserId,
                    @Created = @Created",
                    question
                );

                return await GetQuestionAsync(questionId);
            }
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
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                await connection.ExecuteAsync
                (
                    @"EXEC dbo.Question_Put 
                    @QuestionId = @QuestionId,
                    @Title = @Title,
                    @Content = @Content",
                    new
                    {
                        QuestionId = questionId,
                        question.Title,
                        question.Content,
                    }
                );

                var requestedQuestion = await GetQuestionAsync(questionId);

                // var updatedQuestion = _mapper.Map<QuestionGetSingleResponse, QuestionGetSingleResponse>(requestedQuestion);

                // return updatedQuestion;
                return requestedQuestion;
            };
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
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                await connection.ExecuteAsync
                (
                    @"EXEC dbo.Question_Delete
                    @QuestionId = @QuestionId",
                    new { QuestionId = questionId }
                );
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<AnswerGetResponse> PostFullAnswerAsync(AnswerPostFullRequest answer)
    {
        try
        {
            // var answerPostFullRequest = _mapper.Map<AnswerPostRequest, AnswerPostFullRequest>(answer);
            // answerPostFullRequest.UserId = "1";
            // answerPostFullRequest.UserName = "bob.test@test.com";
            // answerPostFullRequest.Created = DateTime.UtcNow;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryFirstAsync<AnswerGetResponse>
                (
                    @"EXEC dbo.Answer_Post 
                    @QuestionId = @QuestionId,
                    @Content = @Content,
                    @UserId = @UserId,
                    @UserName = @UserName,
                    @Created = @Created",
                    answer
                );
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}