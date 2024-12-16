using ApiCursosCias.Models.Entities;
using ApiCursosCias.Services.Helpers.Authorization;
using ApiCursosCias.Services.Helpers.Extension;
using ApiCursosCias.Services.Helpers.Settings;
using ApiCursosCias.Services.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiCursosCias.Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
public class CursosController : ControllerBase
{
	// Método para registrar un curso
	// [HDU] Crear Servicio Registrar Nuevo Curso
	[HttpPost("RegistrarCurso")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Agendador, TypeRole.Administrativo)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<Curso>))]
	public async Task<IActionResult> RegistrarCurso([FromBody] Request<Curso> request, [FromServices] IValidator<Curso> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("NombreCurso", "CapacidadCurso"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new CursoService();
		var result = await _service.RegistrarCurso(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	// Método para actualizar un curso
	// [HDU] Crear Servicio Eliminar / Editar un curso
	[HttpPost("ActualizarCurso")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Agendador, TypeRole.Administrativo)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<Curso>))]
	public async Task<IActionResult> ActualizarCurso([FromBody] Request<Curso> request, [FromServices] IValidator<Curso> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("IdCurso", "CapacidadCurso"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new CursoService();
		var result = await _service.ActualizarCurso(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	// Método para obtener el listado de cursos
	// [HDU] Crear Servicio Obtener un listado de los cursos creados en estado activo
	[HttpPost("ObtenerCursos")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Agendador, TypeRole.Administrativo)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<List<GetCurso>>))]
	public async Task<IActionResult> ObtenerCursos([FromBody] Request<Curso> request, [FromServices] IValidator<Curso> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("IdUsuario"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new CursoService();
		var result = await _service.ObtenerCursos(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	// Método para obtener los cursos del día, y que se encuentren con la hora vigente
	// [HDU] Crear Servicio Obtener Cursos del Día y/o filtrados por personas
	[HttpPost("ObtenerCursosDia")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Agendador, TypeRole.Administrativo)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<List<GetCurso>>))]
	public async Task<IActionResult> ObtenerCursosDia([FromBody] Request<Curso> request, [FromServices] IValidator<Curso> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("IdUsuario"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new CursoService();
		var result = await _service.ObtenerCursosDia(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	// Método para obtener el listado de cursos por idcurso
	// [HDU] Crear Servicio Obtener Cursos del Día y/o filtrados por personas
	[HttpPost("ObtenerAgendamientoCursos")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Agendador, TypeRole.Administrativo)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<List<GetListadoCurso>>))]
	public async Task<IActionResult> ObtenerAgendamientoCursos([FromBody] Request<Curso> request, [FromServices] IValidator<Curso> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("IdCurso"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new CursoService();
		var result = await _service.ObtenerAgendamientoCursos(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	// Método para obtener el listado de cursos por por usuario
	// [HDU] Crear Servicio Obtener Cursos del Día y/o filtrados por personas
	[HttpPost("ObtenerCursosUsuario")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Agendador, TypeRole.Administrativo)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<List<GetListadoCurso>>))]
	public async Task<IActionResult> ObtenerCursosUsuario([FromBody] Request<SetAgenda> request, [FromServices] IValidator<SetAgenda> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("NumeroDocumento"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new CursoService();
		var result = await _service.ObtenerCursosUsuario(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	// Método para obtener el listado de cursos que cumplen para curso según la fecha de comparendo
	// [HDU] Crear Servicio Obtener cursos por medio de la fecha
	[HttpPost("ObtenerCursosFecha")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Agendador, TypeRole.Administrativo)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<List<GetCurso>>))]
	public async Task<IActionResult> ObtenerCursosFecha([FromBody] Request<SetAgenda> request, [FromServices] IValidator<SetAgenda> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("IdUsuario", "FechaComparendo"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new CursoService();
		var result = await _service.ObtenerCursosFecha(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	// Método para obtener el listado de cursos por identificación de usuario, que se encuentran en estado finalizado
	// [HDU] Crear Servicio Obtener listado cursos x persona
	[HttpPost("ObtenerCursosIdentificacion")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Agendador, TypeRole.Administrativo)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<List<GetCurso>>))]
	public async Task<IActionResult> ObtenerCursosIdentificacion([FromBody] Request<SetAgenda> request, [FromServices] IValidator<SetAgenda> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("IdUsuario", "NumeroDocumento"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new CursoService();
		var result = await _service.ObtenerCursosIdentificacion(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	// Método para obtener el listado de cursos por estado de la agenda
	[HttpPost("ObtenerCursosEstado")]
	// [HDU] N/A
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Agendador, TypeRole.Administrativo)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<List<GetCurso>>))]
	public async Task<IActionResult> ObtenerCursosEstado([FromBody] Request<SetAgenda> request, [FromServices] IValidator<SetAgenda> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("IdCurso", "IdEstadoAgenda"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new CursoService();
		var result = await _service.ObtenerCursosEstado(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}
}