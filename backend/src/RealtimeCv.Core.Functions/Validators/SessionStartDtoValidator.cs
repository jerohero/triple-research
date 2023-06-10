using FluentValidation;
using FluentValidation.Results;
using RealtimeCv.Core.Models.Dto;

namespace RealtimeCv.Core.Functions.Validators;

public class SessionStartDtoValidator : AbstractValidator<SessionStartDto>
{
    public SessionStartDtoValidator()
    {
        RuleFor(x => x.VisionSetId).GreaterThan(0);
        RuleFor(x => x.Source).MinimumLength(1).MaximumLength(100);
    }

    protected override bool PreValidate(ValidationContext<SessionStartDto> context, ValidationResult result)
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
