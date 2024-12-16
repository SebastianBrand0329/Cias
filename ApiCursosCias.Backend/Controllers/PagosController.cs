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
public class PagosController : ControllerBase
{
	// [HDU] Crear Servicio Registrar Pago
	// [HDU] Crear Servicio Generación de PDF factura

	[HttpPost("RegistrarPago")]
	[Authorize(TypeRole.Agendador, TypeRole.Administrativo)]
	[MapToApiVersion("1.0")]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<InformacionSimit>))]
	public async Task<IActionResult> RegistrarPago([FromBody] Request<Pago> request, [FromServices] IValidator<Pago> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("NumeroDocumento"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new PagoService();
		var result = await _service.RegistrarPago(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}
}