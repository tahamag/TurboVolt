using FluentValidation;
using TurboVolt.DTOs;

namespace TurboVolt.Validators;

public class BreceptionFilterValidator : AbstractValidator<BreceptionFilterDto>
{
    public BreceptionFilterValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Le numéro de page doit être supérieur ou égal à 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("La taille de la page doit être comprise entre 1 et 100.");

        RuleFor(x => x)
            .Must(x => !x.DateDebut.HasValue || !x.DateFin.HasValue || x.DateDebut <= x.DateFin)
            .WithMessage("La date de début ne peut pas être supérieure à la date de fin.");
    }
}