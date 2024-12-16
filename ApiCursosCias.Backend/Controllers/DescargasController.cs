using ApiCursosCias.Models.Entities;
using ApiCursosCias.Services.Helpers.Authorization;
using ApiCursosCias.Services.Helpers.Extension;
using ApiCursosCias.Services.Helpers.Settings;
using ApiCursosCias.Services.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiCursosCias.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
public class DescargasController : ControllerBase
{
    // Método para Obtener La información del pago de un curso
    // [HDU] Crear Servicio Generar Información Pago
    [HttpPost("DescargarPagoCurso")]
    [MapToApiVersion("1.0")]
    [Authorize(TypeRole.Agendador, TypeRole.Administrativo)]
    [ValidateParamFilter()]
    [ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
    [Produces(typeof(Response<PagoPdf>))]
    public async Task<IActionResult> DescargarPagoCurso([FromBody] Request<SetAgenda> request, [FromServices] IValidator<SetAgenda> validator)
    {
        var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("IdAgendaCurso"));
        if (!validate.IsValid)
            return BadRequest(new ResponseProblem().Error(validate));

        using var _service = new DescargaService();
        var result = await _service.DescargarPagoCurso(request);
        return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
    }

    // Método para Obtener el pago informativo de un curso
    // [HDU] Crear Servicio Generar Información de factura
    [HttpPost("DescargarPagoInformativoCurso")]
    [MapToApiVersion("1.0")]
    [Authorize(TypeRole.Agendador, TypeRole.Administrativo)]
    [ValidateParamFilter()]
    [ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
    [Produces(typeof(Response<PagoPdf>))]
    public async Task<IActionResult> DescargarPagoInformativoCurso([FromBody] Request<SetAgenda> request, [FromServices] IValidator<SetAgenda> validator)
    {
        var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("IdAgendaCurso"));
        if (!validate.IsValid)
            return BadRequest(new ResponseProblem().Error(validate));

        using var _service = new DescargaService();
        var result = await _service.DescargarPagoInformativoCurso(request);
        return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
    }

    // Método para Obtener el Certificado de un curso
    // [HDU] Crear Servicio Generar Información de certificados
    [HttpPost("DescargarCertificadoCurso")]
    [MapToApiVersion("1.0")]
    [Authorize(TypeRole.Agendador, TypeRole.Administrativo)]
    [ValidateParamFilter()]
    [ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
    [Produces(typeof(Response<Certificado>))]
    public async Task<IActionResult> DescargarCertificadoCurso([FromBody] Request<SetAgenda> request, [FromServices] IValidator<SetAgenda> validator)
    {
        var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("IdAgendaCurso"));
        if (!validate.IsValid)
            return BadRequest(new ResponseProblem().Error(validate));

        using var _service = new DescargaService();
        var result = await _service.DescargarCertificadoCurso(request);
        return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
    }
}