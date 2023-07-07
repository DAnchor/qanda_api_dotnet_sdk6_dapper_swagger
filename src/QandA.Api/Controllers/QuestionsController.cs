using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QandA.DataAccess.Repositories.CachedQuestion;
using QandA.DTOs.Payload.Request;
using QandA.DTOs.Payload.Response;
using QandA.DTOs.Payload.Validation.Request;
using QandA.Services.Container;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace QandA.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IQuestionCache _cache;
    private readonly IQuestionsServiceContainer _service;
    private readonly IUserAuthServiceContainer _authService;
    private readonly ILogger<QuestionsController> _logger;
    private readonly string _userName = "userNameAuthResponse@example.com";

    public QuestionsController(
        IQuestionCache cache,
        IQuestionsServiceContainer service,
        IUserAuthServiceContainer authService,
        ILogger<QuestionsController> logger,
        IMapper mapper)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuestionGetManyResponse>>> GetQuestions
    (
        int pageNumber = 1,
        int pageSize = 20
    )
    {
        try
        {
            var validationArgs = new List<object>() { pageNumber, pageSize };
            var pagingValidations = new PagingValidations();

            foreach (var x in validationArgs)
            {
                var validationResult = pagingValidations.Validate((int)x);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.First());
                }
            }

            var questions = await _service.GetQuestionsAsync(pageNumber, pageSize);

            return Ok(questions);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(500);
        }
    }

    [AllowAnonymous]
    [HttpGet("/GetQuestionsBySearch")]
    public async Task<ActionResult<IEnumerable<QuestionGetManyResponse>>> GetQuestionsBySearch
    (
        string? search,
        bool includeAnswers,
        int pageNumber = 1,
        int pageSize = 20
    )
    {
        try
        {
            var validationArgs = new List<object>() { pageNumber, pageSize };
            var pagingValidations = new PagingValidations();

            foreach (var x in validationArgs)
            {
                var validationResult = pagingValidations.Validate((int)x);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.First());
                }
            }

            var searchResult = string.IsNullOrEmpty(search)
                ? includeAnswers
                ? await _service.GetQuestionsWithAnswersAsync()
                : await _service.GetQuestionsAsync(pageNumber, pageSize)
                : await _service.GetQuestionBySearchAsync(search, pageNumber, pageSize);

            return Ok(searchResult);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(500);
        }
    }

    [AllowAnonymous]
    [HttpGet("GetUnansweredQuestions")]
    public async Task<ActionResult<IEnumerable<QuestionGetManyResponse>>> GetUnansweredQuestions
    (
        int pageNumber = 1,
        int pageSize = 20
    )
    {
        try
        {
            var validationArgs = new List<object>() { pageNumber, pageSize };
            var pagingValidations = new PagingValidations();

            foreach (var x in validationArgs)
            {
                var validationResult = pagingValidations.Validate((int)x);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.First());
                }
            }

            var questions = await _service.GetUnansweredQuestionsAsync(pageNumber, pageSize);

            return Ok(questions);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(500);
        }
    }

    [AllowAnonymous]
    [HttpGet("GetQuestionById/{questionId}")]
    public async Task<ActionResult<QuestionGetSingleResponse>> GetQuestionById(int questionId)
    {
        try
        {
            var idFieldValidation = new IdFieldValidation();
            var validationResult = idFieldValidation.Validate(questionId);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.First());
            }

            var question = await _cache.Get(questionId);

            if (question == null)
            {
                question = await _service.GetQuestionAsync(questionId);

                if (question == null)
                {
                    return NotFound();
                }

                await _cache.Set(question);
            }

            return Ok(question);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(500);
        }
    }

    [Authorize]
    [HttpPost("PostAnswer")]
    public async Task<ActionResult<AnswerGetResponse>> PostAnswer(AnswerPostRequest request)
    {
        try
        {
            var answerPostRequestValidation = new AnswerPostRequestValidation();
            var validationResult = answerPostRequestValidation.Validate(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.First());
            }

            var questionExists = await _service.QuestionExistsAsync(request.QuestionId);

            if (!questionExists)
            {
                return NotFound();
            }

            var userId = ExtractUserIdFromClaim(User);

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound($"Following UserId:{userId} is not found.");
            }

            var answerPostFullRequest = _mapper.Map<AnswerPostRequest, AnswerPostFullRequest>(request);
            answerPostFullRequest.UserId = userId;
            answerPostFullRequest.UserName = _userName;
            answerPostFullRequest.Created = DateTime.UtcNow;

            var savedAnswer = await _service.PostFullAnswerAsync(answerPostFullRequest);

            await _cache.Remove(request.QuestionId);

            return savedAnswer;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(500);
        }
    }

    [Authorize]
    [HttpPost("PostQuestion")]
    public async Task<ActionResult<QuestionGetSingleResponse>> PostQuestion(QuestionPostRequest request)
    {
        try
        {
            var questionPostRequestValidation = new QuestionPostRequestValidation();
            var validationResult = questionPostRequestValidation.Validate(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.First());
            }

            var userId = ExtractUserIdFromClaim(User);

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound($"Following UserId:{userId} is not found.");
            }

            // var requestHeaderProperty = Request.Headers["Authorization"].First();

            // var userNameAuthResponse = await _authService.GetUserName(requestHeaderProperty);

            // if (string.IsNullOrEmpty(userNameAuthResponse))
            // {
            //     return NotFound($"Following UserName:{userNameAuthResponse} is not found.");
            // }

            var mappedQuestion = _mapper.Map<QuestionPostRequest, QuestionPostFullRequest>(request);
            mappedQuestion.UserId = userId;
            // mappedQuestion.UserName = userNameAuthResponse;
            mappedQuestion.UserName = _userName;
            mappedQuestion.Created = DateTime.UtcNow;

            var savedQuestion = await _service.PostFullQuestionAsync(mappedQuestion);

            return Created
            (
                string.Format("{0}/{1}", nameof(GetQuestionById), savedQuestion.QuestionId),
                savedQuestion
            );
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(500);
        }
    }

    [Authorize(Policy = "MustBeQuestionAuthor")]
    [HttpPut("PutQuestion/{questionId}")]
    public async Task<ActionResult<QuestionGetSingleResponse>> PutQuestion(int questionId, QuestionPutRequest request)
    {
        try
        {
            var questionPutRequestValidation = new QuestionPutRequestValidation();
            var validationResult = questionPutRequestValidation.Validate(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.First());
            }

            var updatedQuestion = await _service.PutQuestionAsync(questionId, request);

            await _cache.Remove(updatedQuestion.QuestionId);

            return Ok(updatedQuestion);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(500);
        }
    }

    [Authorize(Policy = "MustBeQuestionAuthor")]
    [HttpDelete("DeleteQuestion/{questionId}")]
    public async Task<ActionResult> DeleteQuestion(int questionId)
    {
        try
        {
            var question = await _service.GetQuestionAsync(questionId);

            if (question == null)
            {
                return NotFound();
            }

            await _service.DeleteQuestionAsync(question.QuestionId);

            await _cache.Remove(questionId);

            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(500);
        }
    }

    private string ExtractUserIdFromClaim(ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return "";
        }

        return userId;
    }
}
