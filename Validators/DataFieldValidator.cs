using Actionstep.API.WebClient.View_Models;
using FluentValidation;

namespace Actionstep.API.WebClient.Validators
{
    public class DataFieldValidator : AbstractValidator<DataFieldAddEditViewModel>
    {
        public DataFieldValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.DataCollectionId).NotNull().WithMessage("You must associate this data field with a data collection.");
            When(p => p.DataCollectionId != null, () =>
            {
                RuleFor(p => p.DataCollectionId).NotEqual("-1").WithMessage("You must associate this data field with a data collection.");
            });

            RuleFor(p => p.DataType).NotNull().WithMessage("You must choose a data type for this data field.");
            When(p => p.DataType != null, () =>
            {
                RuleFor(p => p.DataType).NotEqual("none").WithMessage("You must choose a data type for this data field.");
            });

            RuleFor(p => p.Name).NotEmpty().WithMessage("You must provide a name for this data field.");
            RuleFor(p => p.Label).NotEmpty().WithMessage("You must provide a label for this data field.");
            RuleFor(p => p.Label).Length(5, 25).WithMessage("The label must contain between 5 and 25 characters.");
        }
    }
}