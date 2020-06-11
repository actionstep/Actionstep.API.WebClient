using Actionstep.API.WebClient.View_Models;
using FluentValidation;

namespace Actionstep.API.WebClient.Validators
{
    public class FilenoteValidator : AbstractValidator<FilenoteAddEditViewModel>
    {
        public FilenoteValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.ActionId).NotNull().WithMessage("You must select a Matter to associate with this file note.");
            When(p => p.ActionId != null, () =>
            {
                RuleFor(p => p.ActionId).NotEqual("-1").WithMessage("You must select a Matter to associate with this file note.");
            });

            RuleFor(p => p.Content).NotEmpty().WithMessage("You must provide some content for this file note.");
        }
    }
}