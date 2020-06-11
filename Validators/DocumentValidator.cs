using Actionstep.API.WebClient.View_Models;
using FluentValidation;

namespace Actionstep.API.WebClient.Validators
{
    public class DocumentValidator : AbstractValidator<DocumentAddEditViewModel>
    {
        public DocumentValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.ActionId).NotNull().WithMessage("You must select a Matter to associate with this document.");
            When(p => p.ActionId != null, () =>
            {
                RuleFor(p => p.ActionId).NotEqual("-1").WithMessage("You must select a Matter to associate with this document.");
            });

            RuleFor(p => p.Name).NotEmpty().WithMessage("You must provide a name for this document.");

            When(p => p.DocumentId <= 0, () =>
            {
                RuleFor(p => p.UploadFile).NotNull().WithMessage("You must select a file to upload for this document.");
            });
        }
    }
}