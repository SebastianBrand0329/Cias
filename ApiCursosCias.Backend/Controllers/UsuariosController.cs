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
public class UsuariosController : ControllerBase
{
	// helper methods

	#region helper methods

	private void SetTokenCookie(string token)
	{
		var refreshExpiration = AppSettings.Settings.TokenManagement.RefreshExpiration;
		var expirationType = AppSettings.Settings.TokenManagement.ExpirationType;
		// append cookie with refresh token to the http response
		var cookieOptions = new CookieOptions
		{
			HttpOnly = true,
			Expires = expirationType.Equals("m") ? DateTime.Now.AddMinutes(refreshExpiration) : DateTime.Now.AddDays(refreshExpiration)
		};
		Response.Cookies.Append("refreshToken", token, cookieOptions);
	}

	private string IpAddress()
	{
		// get source ip address for the current request
		if (Request.Headers.ContainsKey("X-Forwarded-For"))
			return Request.Headers["X-Forwarded-For"];
		else
			return HttpContext.Connection.RemoteIpAddress?.ToString();
	}

	#endregion helper methods

	// Método para autenticar el usuario
	// [HDU] Crear Servicio Validar Login
	[HttpPost("AutenticarUsuario")]
	[MapToApiVersion("1.0")]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<UsuarioExtend>))]
	public async Task<IActionResult> AutenticarUsuario([FromBody] Request<Usuario> request, [FromServices] IValidator<Usuario> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("UserName", "Clave"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new UsuarioService();
		var result = await _service.AutenticarUsuario(request, IpAddress());
		if (result.StatusCode.Equals(1))
			SetTokenCookie(result.Result.RefreshToken);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	// Método para actualizar la clave del usuario
	// [HDU] Crear Servicio Actualizar Contraseña
	[HttpPost("ActulizarClave")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Agendador, TypeRole.Administrativo)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<UsuarioExtend>))]
	public async Task<IActionResult> ActulizarClave([FromBody] Request<Usuario> request, [FromServices] IValidator<Usuario> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("UserName", "Clave"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new UsuarioService();
		var result = await _service.ActulizarClave(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	// Método para registrar usuarios agendadores
	// [HDU] Crear Servicio Registrar Agendador
	[HttpPost("RegistrarUsuario")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Agendador, TypeRole.Administrativo)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<Usuario>))]
	public async Task<IActionResult> RegistrarUsuario([FromBody] Request<Usuario> request, [FromServices] IValidator<Usuario> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("UserName", "Identificacion", "Correo", "IdUsuarioRegistra"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new UsuarioService();
		var result = await _service.RegistrarUsuario(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	// Método para actualizar usuarios agendadores
	// [HDU] Crear Servicio Editar y/o eliminar agendador
	[HttpPost("ActualizarUsuario")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Agendador, TypeRole.Administrativo)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<Usuario>))]
	public async Task<IActionResult> ActualizarUsuario([FromBody] Request<Usuario> request, [FromServices] IValidator<Usuario> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("UserName"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new UsuarioService();
		var result = await _service.ActualizarUsuario(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}

	// Método para obtener los usuarios agendadores
	// [HDU] Crear Servicio Obtener listado de usuarios Agendadores por empresa
	[HttpPost("ObtenerUsuariosAgendadores")]
	[MapToApiVersion("1.0")]
	[Authorize(TypeRole.Agendador, TypeRole.Administrativo)]
	[ValidateParamFilter()]
	[ProducesResponseType(typeof(ResponseProblem), (int)HttpStatusCode.BadRequest)]
	[Produces(typeof(Response<List<UsuarioAgendador>>))]
	public async Task<IActionResult> ObtenerUsuariosAgendadores([FromBody] Request<Usuario> request, [FromServices] IValidator<Usuario> validator)
	{
		var validate = await validator.ValidateAsync(request.Data, options => options.IncludeProperties("IdUsuario"));
		if (!validate.IsValid)
			return BadRequest(new ResponseProblem().Error(validate));

		using var _service = new UsuarioService();
		var result = await _service.ObtenerUsuariosAgendadores(request);
		return result.IsSuccess ? Ok(result) : BadRequest(new ResponseProblem().Bad(result));
	}
}