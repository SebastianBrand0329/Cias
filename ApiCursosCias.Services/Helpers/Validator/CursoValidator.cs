using ApiCursosCias.Models.Entities;
using FluentValidation;

namespace ApiCursosCias.Services.Helpers.Validator;

public class CursoValidator : AbstractValidator<Curso>
{
    public CursoValidator()
    {
        RuleFor(curso => curso.NombreCurso)
                .NotNull()
                .NotEmpty().WithMessage("El Nombre del curso es obligatorio.")
                .MaximumLength(30).WithMessage("El Nombre del curso debe tener un máximo de 30 caracteres.");

        RuleFor(curso => curso.CapacidadCurso)
                .NotNull()
                .NotEmpty()
                .NotEqual(0).WithMessage("La capacidad del curso debe ser mayor a 0")
                .Must(numero => numero >= 1 && numero <= 999).WithMessage("La capacidad del curso debe ser menor o igual a 999");

        RuleFor(curso => curso.IdCurso)
                .NotNull()
                .NotEmpty()
                .NotEqual(0).WithMessage("El id del curso de obligatorio");

        RuleFor(curso => curso.IdUsuario)
                .NotNull()
                .NotEmpty()
                .NotEqual(0).WithMessage("El id del usuario es obligatorio");

        RuleFor(curso => curso.IdSede)
        .NotNull()
        .NotEmpty()
        .NotEqual(0).WithMessage("La sede es obligatoria");
    }
}