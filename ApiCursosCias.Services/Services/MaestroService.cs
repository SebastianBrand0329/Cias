using ApiCursosCias.Models.Context.Interfaces;
using ApiCursosCias.Models.Context;
using ApiCursosCias.Models.Entities;
using ApiCursosCias.Services.Helpers.Settings;
using AutoMapper;
using ApiCursosCias.Services.Helpers.CacheMemory;
using ApiCursosCias.Services.Helpers.Extension;

namespace ApiCursosCias.Services.Services;

public class MaestroService : IDisposable
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
	~MaestroService()
	{
		Dispose(false);
	}

	#endregion IDisposable

	private readonly AppSettings _AppSettings = AppSettings.Settings;
	private readonly DaoFactory _factory;
	private readonly ITransactionDao _objTransactionDao;
	private readonly ICacheService _cacheService;

	public MaestroService(ICacheService cacheService = null)
	{
		_factory = DaoFactory.GetDaoFactory(_AppSettings.Connection);
		_objTransactionDao = _factory.GetTransactionDao();
		_cacheService = cacheService;
	}

	public async Task<Response<List<Maestro>>> GetMaestroRoles()
	{
		var response = new Response<List<Maestro>>();

		try
		{
			var cache = _cacheService.GetData<List<Maestro>>();
			if (cache != null & cache?.Count > 0)
				response.Result = cache;
			else
			{
				response.Result = await _objTransactionDao.GetMaestroRoles();
				_cacheService.SetData(response.Result);
			}

			if (response.Result != null & response.Result.Count > 0)
				response.Success(response.Result);
			else
				throw new ResultException("No se encontraron datos con el parametro solicitado.");
		}
		catch (ResultException ex)
		{
			response.Error(ex, response.Result);
		}
		catch (Exception ex)
		{
			response.Exception(ex, response.Result);
		}
		return response;
	}

	public async Task<Response<List<Maestro>>> GetMaestroSedes(Request<Usuario> request)
	{
		var response = new Response<List<Maestro>>();

		try
		{
			var cache = _cacheService.GetData<List<Maestro>>();
			if (cache != null & cache?.Count > 0)
				response.Result = cache;
			else
			{
				response.Result = await _objTransactionDao.GetMaestroSedes(request);
				_cacheService.SetData(response.Result);
			}

			if (response.Result != null & response.Result.Count > 0)
				response.Success(response.Result);
			else
				throw new ResultException("No se encontraron datos con el id empresa solicitado.");
		}
		catch (ResultException ex)
		{
			response.Error(ex, response.Result);
		}
		catch (Exception ex)
		{
			response.Exception(ex, response.Result);
		}
		return response;
	}

	public async Task<Response<List<Maestro>>> GetMaestroTipoDocumento()
	{
		var response = new Response<List<Maestro>>();

		try
		{
			var cache = _cacheService.GetData<List<Maestro>>();
			if (cache != null & cache?.Count > 0)
				response.Result = cache;
			else
			{
				response.Result = await _objTransactionDao.GetMaestroTipoDocumento();
				_cacheService.SetData(response.Result);
			}

			if (response.Result != null & response.Result.Count > 0)
				response.Success(response.Result);
			else
				throw new ResultException("No se encontraron datos con el id empresa solicitado.");
		}
		catch (ResultException ex)
		{
			response.Error(ex, response.Result);
		}
		catch (Exception ex)
		{
			response.Exception(ex, response.Result);
		}
		return response;
	}

	public async Task<Response<List<Maestro>>> GetMaestroDescuentos()
	{
		var response = new Response<List<Maestro>>();

		try
		{
			var cache = _cacheService.GetData<List<Maestro>>();
			if (cache != null & cache?.Count > 0)
				response.Result = cache;
			else
			{
				response.Result = await _objTransactionDao.GetMaestroDescuentos();
				_cacheService.SetData(response.Result);
			}

			if (response.Result != null & response.Result.Count > 0)
				response.Success(response.Result);
			else
				throw new ResultException("No se encontraron datos con el id empresa solicitado.");
		}
		catch (ResultException ex)
		{
			response.Error(ex, response.Result);
		}
		catch (Exception ex)
		{
			response.Exception(ex, response.Result);
		}
		return response;
	}

	public async Task<Response<List<Maestro>>> GetMaestroDocentes(Request<Usuario> request)
	{
		var response = new Response<List<Maestro>>();

		try
		{
			response.Result = await _objTransactionDao.GetMaestroDocentes(request);
			response.Success(response.Result);
		}
		catch (ResultException ex)
		{
			response.Error(ex, response.Result);
		}
		catch (Exception ex)
		{
			response.Exception(ex, response.Result);
		}
		return response;
	}
}