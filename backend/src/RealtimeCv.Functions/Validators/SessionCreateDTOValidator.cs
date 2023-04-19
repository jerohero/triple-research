using FluentValidation;
using FluentValidation.Results;
using RealtimeCv.Functions.Models;
using RealtimeCv.Infrastructure.Data.Config;

namespace RealtimeCv.Functions.Validators;

public class SessionCreateDtoValidator : AbstractValidator<SessionCreateDto>
{
    public SessionCreateDtoValidator()
    {
        RuleFor(x => x.Source).MinimumLength(1).MaximumLength(Constants.DefaultMaxStringLength);
    }

    protected override bool PreValidate(ValidationContext<SessionCreateDto> context, ValidationResult result)
    {
        if (context.InstanceToValidate is not null)
        {
            return true;
        }

        result.Errors.Add(
          new ValidationFailure("Missing DTO", "No valid JSON format supplied")
        );

        return false;
    }
}
