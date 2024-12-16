using ApiCursosCias.Models.Context.Interfaces;
using ApiCursosCias.Models.Context;
using ApiCursosCias.Services.Helpers.CacheMemory;
using ApiCursosCias.Services.Helpers.Settings;
using ApiCursosCias.Models.Entities;
using ApiCursosCias.Services.Helpers.Extension;

namespace ApiCursosCias.Services.Services;

public class DescargaService : IDisposable
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
    ~DescargaService()
    {
        Dispose(false);
    }

    #endregion IDisposable

    private readonly AppSettings _AppSettings = AppSettings.Settings;
    private readonly DaoFactory _factory;
    private readonly ITransactionDao _objTransactionDao;

    public DescargaService()
    {
        _factory = DaoFactory.GetDaoFactory(_AppSettings.Connection);
        _objTransactionDao = _factory.GetTransactionDao();
    }

    public async Task<Response<PagoPdf>> DescargarPagoCurso(Request<SetAgenda> request)
    {
        var response = new Response<PagoPdf>();
        try
        {
            response.Result = await _objTransactionDao.DescargarPagoCurso(request);
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

    public async Task<Response<PagoPdf>> DescargarPagoInformativoCurso(Request<SetAgenda> request)
    {
        var response = new Response<PagoPdf>();
        try
        {
            response.Result = await _objTransactionDao.DescargarPagoInformativoCurso(request);
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

    public async Task<Response<Certificado>> DescargarCertificadoCurso(Request<SetAgenda> request)
    {
        var response = new Response<Certificado>();
        try
        {
            response.Result = await _objTransactionDao.DescargarCertificadoCurso(request);
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