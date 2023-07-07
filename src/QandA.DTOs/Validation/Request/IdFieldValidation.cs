using FluentValidation;

namespace QandA.DTOs.Payload.Validation.Request;

public class IdFieldValidation : AbstractValidator<int>
{
    public IdFieldValidation()
    {
        RuleFor(x => x).Custom((num, context) =>
        {
            if (num < 1)
            {
                context.AddFailure("Id value must be greater than 0.");
            }
            if (string.IsNullOrEmpty(num.ToString()))
            {
                context.AddFailure("Id must not be null.");
            }
        });
    }
}