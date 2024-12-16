using ApiCursosCias.Models.Entities;
using ApiCursosCias.Services.Helpers.Authorization;
using ApiCursosCias.Services.Helpers.CacheMemory;
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
public class AgendasController : ControllerBase
{
	private readonly ICacheService _cacheService;

	public AgendasController(ICacheService cacheService)
	{
		_cacheService = cacheService;
	}

	// Método para obtener el listado de agenda de una sede en especifico
	// [HDU] Crear Servicio Obtener Listado Agendamiento por Día
	[HttpPost("ObtenerAgendaSede")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Administrativo, TypeRole.Agendador)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<List<GetAgenda>>))]
	public async Task<IActionResult> ObtenerAgendaSede(Request<Usuario> request, [FromServices] IValidator<Usuario> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("IdUsuario"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new AgendaService();
		var result = await _service.ObtenerAgendaSede(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	// Método para registrar una agenda, asociado a un comparendo y persona
	// [HDU] Crear Servicio Registrar Inscripción
	[HttpPost("RegistrarAgenda")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Administrativo, TypeRole.Agendador)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<string>))]
	public async Task<IActionResult> RegistrarAgenda(Request<SetAgenda> request, [FromServices] IValidator<SetAgenda> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("IdUsuario", "NumeroComparendo"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new AgendaService(_cacheService);
		var result = await _service.RegistrarAgenda(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	// Método para registrar el ingreso a una agenda
	// [HDU] Crear Servicio Registrar Ingreso de Personas
	[HttpPost("RegistrarIngresoAgenda")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Administrativo, TypeRole.Agendador)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<string>))]
	public async Task<IActionResult> RegistrarIngresoAgenda(Request<SetAgenda> request, [FromServices] IValidator<SetAgenda> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("IdUsuario", "IdAgendaCurso"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new AgendaService(_cacheService);
		var result = await _service.RegistrarIngresoAgenda(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	// Método para registrar la Salida a una agenda
	// [HDU] Crear Servicio Registrar Salida
	[HttpPost("RegistrarSalidaAgenda")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Administrativo, TypeRole.Agendador)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<string>))]
	public async Task<IActionResult> RegistrarSalidaAgenda(Request<SetAgenda> request, [FromServices] IValidator<SetAgenda> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("IdUsuario", "IdAgendaCurso"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new AgendaService(_cacheService);
		var result = await _service.RegistrarSalidaAgenda(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	// Método para registrar la Salida a una agenda
	// [HDU] Crear Servicio Registrar Salida
	[HttpPost("LiberarFupas")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Administrativo, TypeRole.Agendador)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<string>))]
	public async Task<IActionResult> LiberarFupas()
	{
		using var _service = new AgendaService();
		var result = await _service.LiberarFupas();
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}
}