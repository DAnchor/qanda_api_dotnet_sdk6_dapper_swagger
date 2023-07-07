using FluentValidation;
using QandA.DTOs.Payload.Request;

namespace QandA.DTOs.Payload.Validation.Request;

public class QuestionPutRequestValidation : AbstractValidator<QuestionPutRequest>
{
    public QuestionPutRequestValidation()
    {
        RuleFor(x => x.Title).Custom((str, context) =>
        {
            if (string.IsNullOrEmpty(str))
            {
                context.AddFailure("Title field should not be empty.");
            }
            else if (str.Length > 50)
            {
                context.AddFailure("Title field should not exceed 500 characters.");
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