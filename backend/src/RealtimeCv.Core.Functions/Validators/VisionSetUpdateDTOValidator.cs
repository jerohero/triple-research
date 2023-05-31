using FluentValidation;
using FluentValidation.Results;
using RealtimeCv.Core.Models.Dto;

namespace RealtimeCv.Core.Functions.Validators;

public class VisionSetUpdateDtoValidator : AbstractValidator<VisionSetUpdateDto>
{
    public VisionSetUpdateDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.Name).MinimumLength(1).MaximumLength(100);
        
        RuleFor(x => x.Sources).NotEmpty().WithMessage("Sources cannot be empty");
        
        RuleFor(x => x.ContainerImage).MinimumLength(1).MaximumLength(100);

        RuleFor(x => x.TrainedModelId).GreaterThan(0);
    }

    protected override bool PreValidate(ValidationContext<VisionSetUpdateDto> context, ValidationResult result)
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
