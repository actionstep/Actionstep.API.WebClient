using Actionstep.API.WebClient.View_Models;
using FluentValidation;

namespace Actionstep.API.WebClient.Validators
{
    public class ResthookValidator : AbstractValidator<ResthookAddEditViewModel>
    {
        public ResthookValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.EventName).NotNull().WithMessage("You must select an event for this REST hook.");
            RuleFor(p => p.TargetUrl).NotEmpty().WithMessage("You must specify a target url for this REST hook.");
        }
    }
}