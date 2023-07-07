using Microsoft.AspNetCore.Authorization;

namespace QandA.Security.Authorization.Requirements;

public class MustBeQuestionAuthorRequirement : IAuthorizationRequirement
{
    public MustBeQuestionAuthorRequirement() { }
}