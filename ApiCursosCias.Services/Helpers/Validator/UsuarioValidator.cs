using ApiCursosCias.Models.Entities;
using FluentValidation;

namespace ApiCursosCias.Services.Helpers.Validator;

public class UsuarioValidator : AbstractValidator<Usuario>
{
	public UsuarioValidator()
	{
		RuleFor(user => user.IdUsuario)
			.NotNull()
			.NotEmpty()
			.NotEqual(0).WithMessage("El Id del usuario es obligatorio");

		RuleFor(user => user.IdUsuarioRegistra)
		   .NotNull()
		   .NotEmpty()
		   .NotEqual(0).WithMessage("El Id del usuario que registra es obligatorio");

		RuleFor(c => c.UserName)
		.NotEmpty().WithMessage("El usuario es obligatorio.")
		.NotEqual(0).WithMessage("Debe ingresar un número de cédula válido")
		.Must(numero => numero >= 1 && numero <= 99999999999).WithMessage("El usuario debe tener hasta 11 dígitos.");

		RuleFor(c => c.Identificacion)
			.NotEmpty().WithMessage("La Identificación del usuario es obligatoria.");

		RuleFor(user => user.Clave)
			.NotNull()
			.NotEmpty().WithMessage("La contraseña es obligatoria.");
		//.MaximumLength(20).WithMessage("La contraseña debe tener un máximo de 20 caracteres.");

		RuleFor(user => user.Correo)
			.NotNull()
			.NotEmpty()
			.EmailAddress()
			.WithMessage("invalid {PropertyName}");

		RuleFor(user => user.IdTipoDocumento)
		.NotNull()
		.NotEmpty()
		.NotEqual(0).WithMessage("EL tipo de documento es obligatorio");
	}
}