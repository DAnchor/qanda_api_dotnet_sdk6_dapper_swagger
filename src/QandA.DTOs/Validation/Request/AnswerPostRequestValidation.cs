using FluentValidation;
using QandA.DTOs.Payload.Request;

namespace QandA.DTOs.Payload.Validation.Request;

public class AnswerPostRequestValidation : AbstractValidator<AnswerPostRequest>
{
    public AnswerPostRequestValidation()
    {
        RuleFor(x => x.QuestionId).Custom((str, context) =>
        {
            if (string.IsNullOrEmpty(str.ToString()))
            {
                context.AddFailure("QuestionId field should not be empty.");
            }
        });
        RuleFor(x => x.Content).Custom((str, context) =>
        {
            if (string.IsNullOrEmpty(str))
            {
                context.AddFailure("Content field should not be empty.");
            }
            else if (str.Length > 500)
            {
                context.AddFailure("Content field should not exceed 500 characters.");
            }
        });
    }
}