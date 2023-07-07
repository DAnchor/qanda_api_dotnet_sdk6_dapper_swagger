using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using QandA.DTOs.Payload.Request;

namespace QandA.DTOs.Configuration;

public static class ValidationConfiguration
{
    public static void AddValidationConfiguration(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<AnswerPostRequest>();
        services.AddValidatorsFromAssemblyContaining<AnswerPostFullRequest>();
        services.AddValidatorsFromAssemblyContaining<QuestionPostFullRequest>();
        services.AddValidatorsFromAssemblyContaining<QuestionPostRequest>();
        services.AddValidatorsFromAssemblyContaining<QuestionPutRequest>();
    }
}