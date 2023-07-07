using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using QandA.Security.Authorization.Requirements;
using QandA.Services.Container;

namespace QandA.Security.Authorization.Handlers;

public class MustBeQuestionAuthorHandler : AuthorizationHandler<MustBeQuestionAuthorRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IQuestionsServiceContainer _service;
    public MustBeQuestionAuthorHandler(IHttpContextAccessor contextAccessor, IQuestionsServiceContainer service)
    {
        _httpContextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MustBeQuestionAuthorRequirement requirement)
    {
        if (!context.User.Identity.IsAuthenticated)
        {
            context.Fail();
            return;
        }

        int questionId = Convert.ToInt32(_httpContextAccessor.HttpContext?.Request.RouteValues["questionId"]);
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var question = await _service.GetQuestionAsync(questionId);

        // let QuestionsController decide what http code to output (404 notFound) rather than (401 unauthorized)
        if (question == null)
        {
            context.Succeed(requirement);
            return;
        }

        if (question.UserId != userId)
        {
            context.Fail();

            return;
        }

        context.Succeed(requirement);
    }
}