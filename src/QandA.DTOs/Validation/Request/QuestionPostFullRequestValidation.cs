using FluentValidation;
using QandA.DTOs.Payload.Request;

namespace QandA.DTOs.Payload.Validation.Request;

public class QuestionPostFullRequestValidation : AbstractValidator<QuestionPostFullRequest>
{
    public QuestionPostFullRequestValidation()
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
        RuleFor(x => x.UserName).Custom((str, context) =>
        {
            if (string.IsNullOrEmpty(str))
            {
                context.AddFailure("UserName field should not be empty.");
            }
            else if (str.Length > 50)
            {
                context.AddFailure("UserName field should not exceed 50 characters.");
            }
        });
        RuleFor(x => x.UserId).Custom((str, context) =>
        {
            if (string.IsNullOrEmpty(str))
            {
                context.AddFailure("UserId field should not be empty.");
            }
            else if (str.Length > 50)
            {
                context.AddFailure("UserId field should not exceed 50 characters.");
            }
        });
        RuleFor(x => x.Created).Custom((str, context) =>
        {
            if (string.IsNullOrEmpty(str.ToString()))
            {
                context.AddFailure("Created field should not be empty.");
            }
        });
    }
}