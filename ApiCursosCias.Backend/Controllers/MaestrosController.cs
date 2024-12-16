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
public class MaestrosController : ControllerBase
{
	private readonly ICacheService _cacheService;

	public MaestrosController(ICacheService cacheService)
	{
		_cacheService = cacheService;
	}

	[HttpPost("GetMaestroRoles")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Administrativo, TypeRole.Agendador)]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<List<Maestro>>))]
	public async Task<IActionResult> GetMaestroRoles()
	{
		using var _service = new MaestroService(_cacheService);
		var result = await _service.GetMaestroRoles();
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	[HttpPost("GetMaestroSedes")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Administrativo, TypeRole.Agendador)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<List<Maestro>>))]
	public async Task<IActionResult> GetMaestroSedes(Request<Usuario> request)
	{
		using var _service = new MaestroService(_cacheService);
		var result = await _service.GetMaestroSedes(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	[HttpPost("GetMaestroTipoDocumento")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Administrativo, TypeRole.Agendador)]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<List<Maestro>>))]
	public async Task<IActionResult> GetMaestroTipoDocumento()
	{
		using var _service = new MaestroService(_cacheService);
		var result = await _service.GetMaestroTipoDocumento();
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	[HttpPost("GetMaestroDescuentos")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Administrativo, TypeRole.Agendador)]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<List<Maestro>>))]
	public async Task<IActionResult> GetMaestroDescuentos()
	{
		using var _service = new MaestroService(_cacheService);
		var result = await _service.GetMaestroDescuentos();
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	[HttpPost("GetMaestroDocentes")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Administrativo, TypeRole.Agendador)]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<List<Maestro>>))]
	public async Task<IActionResult> GetMaestroDocentes(Request<Usuario> request, [FromServices] IValidator<Usuario> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("IdUsuario"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new MaestroService(_cacheService);
		var result = await _service.GetMaestroDocentes(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}
}