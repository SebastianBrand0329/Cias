using ApiCursosCias.Models.Entities;
using FluentValidation;

namespace ApiCursosCias.Services.Helpers.Validator;

public class ConsultaValidator : AbstractValidator<Consulta>
{
    public ConsultaValidator()
    {
        RuleFor(agenda => agenda.NumeroDocumento)
        .NotNull()
        .NotEmpty().WithMessage("El número de documento es obligatorio.");
    }
}