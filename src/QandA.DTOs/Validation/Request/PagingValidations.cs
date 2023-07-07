using FluentValidation;

namespace QandA.DTOs.Payload.Validation.Request;

public class PagingValidations : AbstractValidator<int>
{
    public PagingValidations()
    {
        RuleFor(x => x).Custom((num, context) =>
        {
            if (nameof(num) == "pageNumber" && num > 1)
            {
                context.AddFailure("Only 1 page is allowed per view.");
            }
            if (nameof(num) == "pageNumber" && num < 0)
            {
                context.AddFailure("Value of PageNumber  should be greater than -1.");
            }
            if (nameof(num) == "PageNumber" && string.IsNullOrEmpty(num.ToString()))
            {
                context.AddFailure("Page number must not be null.");
            }
            if (nameof(num) == "pageSize" && num > 20)
            {
                context.AddFailure("Only 20 records per page are allowed.");
            }
            if (nameof(num) == "pageSize" && num < 0)
            {
                context.AddFailure("Value of PageSize size should be greater than -1.");
            }
            if (nameof(num) == "pageSize" && string.IsNullOrEmpty(num.ToString()))
            {
                context.AddFailure("PageSize must not be null.");
            }
        });
    }
}