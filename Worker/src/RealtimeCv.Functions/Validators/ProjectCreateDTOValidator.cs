using FluentValidation;
using FluentValidation.Results;
using RealtimeCv.Functions.Models;

namespace RealtimeCv.Functions.Validators;

public class ProjectCreateDtoValidator : AbstractValidator<ProjectCreateDto>
{
  public ProjectCreateDtoValidator()
  {
    RuleFor(x => x.Name).MinimumLength(1).MaximumLength(1000);
  }

  protected override bool PreValidate(ValidationContext<ProjectCreateDto> context, ValidationResult result)
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
