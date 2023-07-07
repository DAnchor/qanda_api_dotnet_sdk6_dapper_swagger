using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using QandA.Security.Authorization.Handlers;
using QandA.Security.Authorization.Requirements;

namespace QandA.Security.Configuration;

public static class AuthorizationPoliciesConfiguration
{
    public static void AddAuthorizationPoliciesConfiguration(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddAuthorization(options =>
            options.AddPolicy("MustBeQuestionAuthor", policy =>
                policy.Requirements.Add(new MustBeQuestionAuthorRequirement())));
        services.AddScoped<IAuthorizationHandler, MustBeQuestionAuthorHandler>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    }
}