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
public class ConsultasController : ControllerBase
{
	// [HDU] Crear Servicio Consultar comparendos (consumo api simit)
	[HttpPost("ConsultaSimit")]
	[Authorize(TypeRole.Agendador, TypeRole.Administrativo)]
	[MapToApiVersion("1.0")]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<InformacionSimit>))]
	public async Task<IActionResult> ConsultaSimit([FromBody] Request<Consulta> request, [FromServices] IValidator<Consulta> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("NumeroDocumento"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new ConsultaService();
		var result = await _service.ConsultaSimit(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}
}