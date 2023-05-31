using FluentValidation;
using FluentValidation.Results;
using RealtimeCv.Core.Models.Dto;

namespace RealtimeCv.Core.Functions.Validators;

public class ProjectUpdateDtoValidator : AbstractValidator<ProjectUpdateDto>
{
    public ProjectUpdateDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        
        RuleFor(x => x.Name).MinimumLength(1).MaximumLength(100);
    }

    protected override bool PreValidate(ValidationContext<ProjectUpdateDto> context, ValidationResult result)
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
