using ApiCursosCias.Models.Context;
using ApiCursosCias.Models.Context.Interfaces;
using ApiCursosCias.Models.Entities;
using ApiCursosCias.Services.Helpers.Authorization;
using ApiCursosCias.Services.Helpers.Extension;
using ApiCursosCias.Services.Helpers.Integration;
using ApiCursosCias.Services.Helpers.Settings;
using AutoMapper;
using IM.Encrypt.Access.Models;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text;

namespace ApiCursosCias.Services.Services;

public class UsuarioService : IDisposable
{
	#region IDisposable

	private bool disposed;

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
			}
			disposed = true;
		}
	}

	// Destructor
	~UsuarioService()
	{
		Dispose(false);
	}

	#endregion IDisposable

	private readonly AppSettings _AppSettings = AppSettings.Settings;
	private readonly IMapper _mapper = MapperBootstrapper.Instance;

	private readonly DaoFactory _factory;
	private readonly ITransactionDao _objTransactionDao;

	public UsuarioService()
	{
		_factory = DaoFactory.GetDaoFactory(_AppSettings.Connection);
		_objTransactionDao = _factory.GetTransactionDao();
	}

	public Response<string> Get()
	{
		return new Response<string>().Success();
	}

	public async Task<Response<UsuarioExtend>> AutenticarUsuario(Request<Usuario> request, string ip)
	{
		var response = new Response<UsuarioExtend>();
		try
		{
			request.Data.FechaIngreso = DateTime.Now;
			var user = await _objTransactionDao.AutenticarUsuario(request);

			if (user != null)
			{
				var usuarioLogin = await _objTransactionDao.GetUsuarioLogin(new UsuarioLogin { CodeLogin = user.UserName });
				response.Warn(usuarioLogin);
				var refreshToken = usuarioLogin?.Where(x => !x.IsExpired)
												.OrderByDescending(x => DateTime.Parse(x.Expires))
												.FirstOrDefault();

				UsuarioLoginExtended newRefreshToken;
				if (refreshToken != null)
				{
					user.RefreshTokens.Add(refreshToken);
					// replace old refresh token with a new one (rotate token)
					newRefreshToken = Security.RotateRefreshToken(refreshToken, ip);
				}
				else
					// authentication successful so generate jwt and refresh tokens
					newRefreshToken = Security.GenerateRefreshToken(ip);

				newRefreshToken.CodeLogin = user.UserName;
				user.RefreshTokens.Add(newRefreshToken);

				// remove old refresh tokens from user
				user.RemoveOldRefreshTokens();

				user.RefreshToken = newRefreshToken.Token;

				var userLogin = _mapper.Map<List<UsuarioLogin>>(user.RefreshTokens);
				await _objTransactionDao.SetUsuarioLogin(userLogin);

				user.Token = Security.GetToken(user.UserName.ToString(), (int)user.IdRol, newRefreshToken.Token);

				response.Result = user;
				response.Success(request);
			}
			else
			{
				response.StatusMessage = "Usuario [null] en la respuesta";
				response.Warn(request);
			}
		}
		catch (ResultException ex)
		{
			response.Error(ex, request);
		}
		catch (Exception ex)
		{
			response.Exception(ex, request);
		}
		return response;
	}

	public async Task<Response<bool>> ActulizarClave(Request<Usuario> request)
	{
		var response = new Response<bool>();
		try
		{
			response.Result = await _objTransactionDao.ActulizarClave(request);
			response.Success(request);
		}
		catch (ResultException ex)
		{
			response.Error(ex, request);
		}
		catch (Exception ex)
		{
			response.Exception(ex, request);
		}
		return response;
	}

	public async Task<Response<Usuario>> RegistrarUsuario(Request<Usuario> request)
	{
		var response = new Response<Usuario>();
		try
		{
			request.Data.Clave = GenerarPasswordAleatorio();
			request.Data.Clave = Encryt.Encrypt(request.Data.Clave, _AppSettings.Secret);
			request.Data.UserName = request.Data.Identificacion;
			request.Data.IdRol = (int?)TypeRole.Agendador;
			response.Result = await _objTransactionDao.RegistrarUsuario(request);
			response.Result.Clave = Encryt.Decrypt(response.Result.Clave, _AppSettings.Secret);
			if (!string.IsNullOrEmpty(response.Result.Notificacion.Plantilla))
			{
				var email = new EmailServer.EmailParams
				{
					TypeTemplate = EmailServer.TypeTemplate.BienvenidaAgendador,
					EmailTo = new List<string> { request.Data.Correo },
					Subject = response.Result.Notificacion.Asunto,
					Template = response.Result.Notificacion.Plantilla,
					Data = response.Result,
				}.ReplacePlantilla();
				await EmailServer.SendEmail(email).ConfigureAwait(false);
			}
			response.Success(request);
		}
		catch (ResultException ex)
		{
			response.Error(ex, request);
		}
		catch (Exception ex)
		{
			response.Exception(ex, request);
		}
		return response;
	}

	public async Task<Response<Usuario>> ActualizarUsuario(Request<Usuario> request)
	{
		var response = new Response<Usuario>();
		try
		{
			response.Result = await _objTransactionDao.ActualizarUsuario(request);
			response.Success(request);
		}
		catch (ResultException ex)
		{
			response.Error(ex, request);
		}
		catch (Exception ex)
		{
			response.Exception(ex, request);
		}
		return response;
	}

	public async Task<Response<List<UsuarioAgendador>>> ObtenerUsuariosAgendadores(Request<Usuario> request)
	{
		var response = new Response<List<UsuarioAgendador>>();
		try
		{
			response.Result = await _objTransactionDao.ObtenerUsuariosAgendadores(request);
			response.Success(request);
		}
		catch (ResultException ex)
		{
			response.Error(ex, request);
		}
		catch (Exception ex)
		{
			response.Exception(ex, request);
		}
		return response;
	}

	#region Métodos Privados

	private string GenerarPasswordAleatorio()
	{
		const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
		var random = new Random();
		var contraseña = new StringBuilder();

		for (int i = 0; i < 8; i++)
		{
			contraseña.Append(caracteres[random.Next(caracteres.Length)]);
		}

		return contraseña.ToString();
	}

	#endregion Métodos Privados
}