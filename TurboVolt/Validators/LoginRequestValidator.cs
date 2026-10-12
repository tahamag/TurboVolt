using FluentValidation;
using TurboVolt.DTOs;

namespace TurboVolt.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Le nom d'utilisateur est obligatoire.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Le mot de passe est obligatoire.");
        }
    }
}
