using ApiCursosCias.Models.Entities;
using FluentValidation;

namespace ApiCursosCias.Services.Helpers.Validator;

public class AgendaValidator : AbstractValidator<SetAgenda>
{
	public AgendaValidator()
	{
		RuleFor(agenda => agenda.NumeroComparendo)
			.NotEmpty().WithMessage("El número del comparendo es obligatorio.")
			.MaximumLength(20).WithMessage("El comparendo debe ser de máximo 20 caracteres.");

		RuleFor(agenda => agenda.NumeroDocumento)
			.NotNull()
			.NotEmpty().WithMessage("El número de documento es obligatorio.");

		RuleFor(curso => curso.IdCurso)
				.NotNull()
				.NotEmpty()
				.NotEqual(0).WithMessage("La curso es obligatorio");

		RuleFor(curso => curso.FechaComparendo)
			.NotNull().WithMessage("La fecha de comparendo es obligatoria.")
			.Must(date => date != default).WithMessage("La fecha de comparendo no puede ser la predeterminada.");

		RuleFor(curso => curso.IdUsuario)
		.NotNull()
		.NotEmpty()
		.NotEqual(0).WithMessage("El usuario es obligatorio");

		RuleFor(agenda => agenda.FechaComparendo)
			.NotNull()
			.NotEmpty().WithMessage("La fecha del comparendo es obligatoria.");

		RuleFor(curso => curso.IdAgendaCurso)
		.NotNull()
		.NotEmpty()
		.NotEqual(0).WithMessage("El Id de la Agenda es obligatoria");

		RuleFor(curso => curso.IdCurso)
			.NotNull()
			.NotEmpty()
			.NotEqual(0).WithMessage("El curso es obligatorio");
	}
}