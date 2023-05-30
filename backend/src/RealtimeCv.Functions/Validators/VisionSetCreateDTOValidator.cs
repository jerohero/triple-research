using FluentValidation;
using FluentValidation.Results;
using RealtimeCv.Functions.Models;
using RealtimeCv.Infrastructure.Data.Config;

namespace RealtimeCv.Functions.Validators;

public class VisionSetCreateDtoValidator : AbstractValidator<VisionSetCreateDto>
{
    public VisionSetCreateDtoValidator()
    {
        RuleFor(x => x.Name).MinimumLength(1).MaximumLength(Constants.DefaultMaxStringLength);
        
        RuleFor(x => x.Sources).NotEmpty().WithMessage("Sources cannot be empty");
        
        RuleFor(x => x.ContainerImage).MinimumLength(1).MaximumLength(Constants.DefaultMaxStringLength);

        RuleFor(x => x.TrainedModelId).GreaterThan(0);
    }

    protected override bool PreValidate(ValidationContext<VisionSetCreateDto> context, ValidationResult result)
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
