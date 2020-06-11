using Actionstep.API.WebClient.View_Models;
using FluentValidation;

namespace Actionstep.API.WebClient.Validators
{
    public class SettingsValidator : AbstractValidator<SettingsViewModel>
    {
        public SettingsValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.ClientId).NotEmpty().WithMessage("You must supply your Client ID.");
            RuleFor(p => p.ClientSecret).NotEmpty().WithMessage("You must supply your Client Secret.");
            RuleFor(p => p.AccessTokenUrl).NotEmpty().WithMessage("You must supply a valid Access Token Url.");
            RuleFor(p => p.AuthorizeUrl).NotEmpty().WithMessage("You must supply a valid Authorization Url.");
            RuleFor(p => p.Scopes).NotEmpty().WithMessage("You must supply at least one valid scope.");
            RuleFor(p => p.RedirectUrl).NotEmpty().WithMessage("You must supply a Redirect Url for the Authentication server to callback.");
        }
    }
}