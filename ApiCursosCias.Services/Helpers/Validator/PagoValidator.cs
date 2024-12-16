using ApiCursosCias.Models.Entities;
using FluentValidation;

namespace ApiCursosCias.Services.Helpers.Validator;

public class PagoValidator : AbstractValidator<Pago>
{
    public PagoValidator()
    {
        RuleFor(agenda => agenda.IdUsuario)
            .NotNull()
            .NotEmpty()
            .NotEqual(0)
            .WithMessage("El Usuario es obligatorio.");

        RuleFor(curso => curso.IdAgendaCurso)
            .NotNull()
            .NotEmpty()
            .NotEqual(0).WithMessage("La agenda es obligatoria.");

        RuleFor(curso => curso.ValorComparendo)
            .NotNull()
            .NotEmpty()
            .NotEqual(0).WithMessage("El valor del comparendo es obligatorio.");

        RuleFor(curso => curso.PorcentajeDescuento)
            .NotNull()
            .NotEmpty()
            .NotEqual(0).WithMessage("El porcentaje de descuento es obligatorio");

        RuleFor(curso => curso.ValorApagar)
            .NotNull()
            .NotEmpty()
            .NotEqual(0).WithMessage("El valor a pagar es obligatorio.");

        RuleFor(curso => curso.CodigoInfraccion)
            .NotNull()
            .NotEmpty()
            .WithMessage("El código de infracción es obligatorio.");

        RuleFor(curso => curso.ValorCurso)
            .NotNull()
            .NotEmpty()
            .NotEqual(0).WithMessage("El valor del curso es obligatorio.");

        RuleFor(curso => curso.MunicipioInfraccion)
            .NotNull()
            .NotEmpty()
            .WithMessage("El municipio es obligatorio.");
    }
}