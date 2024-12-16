using ApiCursosCias.Models.Context;
using ApiCursosCias.Models.Context.Interfaces;
using ApiCursosCias.Models.Entities;
using ApiCursosCias.Services.Helpers.Extension;
using ApiCursosCias.Services.Helpers.Settings;
using AutoMapper;

namespace ApiCursosCias.Services.Services;

public class CursoService : IDisposable
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
	~CursoService()
	{
		Dispose(false);
	}

	#endregion IDisposable

	private readonly AppSettings _AppSettings = AppSettings.Settings;
	private readonly IMapper _mapper = MapperBootstrapper.Instance;

	private readonly DaoFactory _factory;
	private readonly ITransactionDao _objTransactionDao;

	public CursoService()
	{
		_factory = DaoFactory.GetDaoFactory(_AppSettings.Connection);
		_objTransactionDao = _factory.GetTransactionDao();
	}

	public async Task<Response<Curso>> RegistrarCurso(Request<Curso> request)
	{
		var response = new Response<Curso>();
		try
		{
			response.Result = await _objTransactionDao.RegistrarCurso(request);
			response.Success(response.Result);
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

	public async Task<Response<Curso>> ActualizarCurso(Request<Curso> request)
	{
		var response = new Response<Curso>();
		try
		{
			response.Result = await _objTransactionDao.ActualizarCurso(request);
			response.Success(response.Result);
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

	public async Task<Response<List<GetCurso>>> ObtenerCursos(Request<Curso> request)
	{
		var response = new Response<List<GetCurso>>();
		try
		{
			response.Result = await _objTransactionDao.ObtenerCursos(request);
			response.Success(response.Result);
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

	public async Task<Response<List<GetCurso>>> ObtenerCursosDia(Request<Curso> request)
	{
		var response = new Response<List<GetCurso>>();
		try
		{
			response.Result = await _objTransactionDao.ObtenerCursosDia(request);
			response.Success(response.Result);
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

	public async Task<Response<List<GetListadoCurso>>> ObtenerAgendamientoCursos(Request<Curso> request)
	{
		var response = new Response<List<GetListadoCurso>>();
		try
		{
			response.Result = await _objTransactionDao.ObtenerAgendamientoCursos(request);
			response.Success(response.Result);
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

	public async Task<Response<List<GetListadoCurso>>> ObtenerCursosUsuario(Request<SetAgenda> request)
	{
		var response = new Response<List<GetListadoCurso>>();
		try
		{
			response.Result = await _objTransactionDao.ObtenerCursosUsuario(request);
			response.Success(response.Result);
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

	public async Task<Response<List<GetCurso>>> ObtenerCursosFecha(Request<SetAgenda> request)
	{
		var response = new Response<List<GetCurso>>();
		try
		{
			response.Result = await _objTransactionDao.ObtenerCursosFecha(request);
			response.Success(response.Result);
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

	public async Task<Response<List<GetListadoCurso>>> ObtenerCursosIdentificacion(Request<SetAgenda> request)
	{
		var response = new Response<List<GetListadoCurso>>();
		try
		{
			response.Result = await _objTransactionDao.ObtenerCursosIdentificacion(request);
			response.Success(response.Result);
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

	public async Task<Response<List<GetListadoCurso>>> ObtenerCursosEstado(Request<SetAgenda> request)
	{
		var response = new Response<List<GetListadoCurso>>();
		try
		{
			response.Result = await _objTransactionDao.ObtenerCursosEstado(request);
			response.Success(response.Result);
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
}