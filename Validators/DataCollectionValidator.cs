using Actionstep.API.WebClient.View_Models;
using FluentValidation;

namespace Actionstep.API.WebClient.Validators
{
    public class DataCollectionValidator : AbstractValidator<DataCollectionAddEditViewModel>
    {
        public DataCollectionValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.MatterTypeId).NotNull().WithMessage("You must associate this data collection with a matter type.");
            When(p => p.MatterTypeId != null, () =>
            {
                RuleFor(p => p.MatterTypeId).NotEqual("-1").WithMessage("You must associate this data collection with a matter type.");
            });

            RuleFor(p => p.Name).NotEmpty().WithMessage("You must provide a name for this data collection.");
            RuleFor(p => p.Label).NotEmpty().WithMessage("You must provide a label for this data collection.");
            RuleFor(p => p.Description).NotEmpty().WithMessage("You must provide a description for this data collection.");

            RuleFor(p => p.Label).Length(5, 25).WithMessage("The label must contain between 5 and 25 characters.");
        }
    }
}