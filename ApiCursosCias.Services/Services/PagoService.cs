using ApiCursosCias.Models.Context.Interfaces;
using ApiCursosCias.Models.Context;
using ApiCursosCias.Models.Entities;
using ApiCursosCias.Services.Helpers.Settings;
using AutoMapper;
using ApiCursosCias.Services.Helpers.Extension;
using ApiCursosCias.Services.Helpers.Integration;
using Microsoft.AspNetCore.Http;

namespace ApiCursosCias.Services.Services;

public class PagoService : IDisposable
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
    ~PagoService()
    {
        Dispose(false);
    }

    #endregion IDisposable

    private readonly AppSettings _AppSettings = AppSettings.Settings;
    private readonly IMapper _mapper = MapperBootstrapper.Instance;

    private readonly DaoFactory _factory;
    private readonly ITransactionDao _objTransactionDao;

    public PagoService()
    {
        _factory = DaoFactory.GetDaoFactory(_AppSettings.Connection);
        _objTransactionDao = _factory.GetTransactionDao();
    }

    public async Task<Response<string>> RegistrarPago(Request<Pago> request)
    {
        var response = new Response<string>();
        try
        {
            var pago = await _objTransactionDao.RegistrarPago(request);
            if (!string.IsNullOrEmpty(pago.Notificacion.Plantilla))
            {
                _ = Task.Run(async () =>
                {
                    List<string> attach = new();
                    string pdf = string.Empty;
                    try
                    {
                        var email = new EmailServer.EmailParams
                        {
                            TypeTemplate = EmailServer.TypeTemplate.PagoCurso,
                            Subject = $"Recibo de Pago Curso {pago.NombreCurso}",
                            EmailTo = new List<string> { pago.CorreoElectronico },
                            Template = pago.Notificacion?.Plantilla,
                            TemplateAttach = pago.Notificacion?.PlantillaAdjunto,
                            Data = pago
                        };

                        if (!string.IsNullOrEmpty(email.TemplateAttach))
                        {
                            email.ReplacePlantilla();

                            pdf = ExtensionMethods.HtmlTemplateToPDF(email.TemplateAttach, "pago", pago.IdConsecutivoPago.ToString());
                            attach.Add(pdf);

                            email.PathAttachment = attach.Count > 0 ? attach : null;
                            await EmailServer.SendEmail(email);
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Warn(ex, request);
                    }
                    finally
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(pdf) && File.Exists(pdf))
                            {
                                File.Delete(pdf);
                            }

                            var carpeta = Path.GetDirectoryName(pdf);
                            if (!string.IsNullOrEmpty(carpeta) && Directory.Exists(carpeta))
                            {
                                Directory.Delete(carpeta, true);
                            }
                        }
                        catch (IOException ioEx)
                        {
                            response.Warn(ioEx, request);
                        }
                        catch (Exception ex)
                        {
                            response.Warn(ex, request);
                        }
                    }
                }).ConfigureAwait(false);
            }
            else
            {
                response.StatusMessage = "No se encuentra plantilla para envío de correo";
                response.Warn(request);
            }
            response.Success(pago);
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