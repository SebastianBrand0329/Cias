using ApiCursosCias.Models.Context;
using ApiCursosCias.Models.Context.Interfaces;
using ApiCursosCias.Models.Entities;
using ApiCursosCias.Services.Helpers.Extension;
using ApiCursosCias.Services.Helpers.Integration;
using ApiCursosCias.Services.Helpers.Settings;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ApiCursosCias.Services.Services;

public class ConsultaService : IDisposable
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
	~ConsultaService()
	{
		Dispose(false);
	}

	#endregion IDisposable

	private readonly AppSettings _AppSettings = AppSettings.Settings;
	private readonly DaoFactory _factory;
	private readonly ITransactionDao _objTransactionDao;

	public ConsultaService()
	{
		_factory = DaoFactory.GetDaoFactory(_AppSettings.Connection);
		_objTransactionDao = _factory.GetTransactionDao();
	}

	public async Task<Response<InformacionSimit>> ConsultaSimit(Request<Consulta> request)
	{
		var response = new Response<InformacionSimit>();
		try
		{
			var _service = _AppSettings.Integration.Services.Where(x => x.Name.Equals("SIMIT")).FirstOrDefault();
			request.Data.ReCaptchaDTO = new()
			{
				Response = _service.Authentication.Pass,
				Consumidor = _service.Authentication.User
			};
			request.Data.IdTipoDocumento = 0;
			if (request.Data.IdTipoDocumento.Equals(0))
			{
				var consulta = new ConsultaInfo { Filtro = request.Data.NumeroDocumento, ReCaptchaDTO = request.Data.ReCaptchaDTO };
				var result = await RequestHttp.CallMethod<InformacionSimit>("SIMIT", "consulta", consulta, HttpMethod.Post, RequestHttp.TypeBody.Body);

				if (result.codigo == 0)
				{
					if (result.personasMismoDocumento != null)
					{
						response.StatusCode = result.personasMismoDocumento.Count > 0 ? 99 : 1;
						var multas = ValidarTipoComparendo(result);
						response.Result = await _objTransactionDao.GetInformacionSimit(multas);
						response.Success(result);
					}
				}
				else
					throw new Exception(result.descripcion);
			}
			else
			{
				var documento = await _objTransactionDao.GetMaestroTipoDocumento();
				if (!documento.Exists(x => x.Id == request.Data.IdTipoDocumento))
					throw new Exception("El tipo de documento ingresado no es válido.");

				var result = await RequestHttp.CallMethod<InformacionSimit>("SIMIT", "consultaPorInfractor", request.Data, HttpMethod.Post, RequestHttp.TypeBody.Body);
				if (result.codigo == 0)
				{
					var multas = ValidarTipoComparendo(result);
					response.Result = await _objTransactionDao.GetInformacionSimit(multas);
					response.StatusCode = 1;
					response.Success(result);
				}
				else
					throw new Exception(result.descripcion);
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

	private InformacionSimit ValidarTipoComparendo(InformacionSimit informacion)
	{
		informacion.multas.ForEach(x =>
		  {
			  if (x.comparendoElectronico == true)
				  x.IdtipoComparendo = 2;
			  else
				  x.IdtipoComparendo = 1;
		  });

		return informacion;
	}
}