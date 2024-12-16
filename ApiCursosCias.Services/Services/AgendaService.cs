using ApiCursosCias.Models.Context;
using ApiCursosCias.Models.Context.Interfaces;
using ApiCursosCias.Models.Entities;
using ApiCursosCias.Services.Helpers.CacheMemory;
using ApiCursosCias.Services.Helpers.Extension;
using ApiCursosCias.Services.Helpers.Integration;
using ApiCursosCias.Services.Helpers.Settings;
using Azure.Core;

namespace ApiCursosCias.Services.Services;

public class AgendaService : IDisposable
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
	~AgendaService()
	{
		Dispose(false);
	}

	#endregion IDisposable

	private readonly AppSettings _AppSettings = AppSettings.Settings;
	private readonly DaoFactory _factory;
	private readonly ITransactionDao _objTransactionDao;

	public AgendaService(ICacheService cacheService = null)
	{
		_factory = DaoFactory.GetDaoFactory(_AppSettings.Connection);
		_objTransactionDao = _factory.GetTransactionDao();
	}

	public async Task<Response<List<GetAgenda>>> ObtenerAgendaSede(Request<Usuario> request)
	{
		var response = new Response<List<GetAgenda>>();
		try
		{
			response.Result = await _objTransactionDao.ObtenerAgendaSede(request);
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

	public async Task<Response<string>> RegistrarAgenda(Request<SetAgenda> request)
	{
		var response = new Response<string>();
		try
		{
			response.Result = await _objTransactionDao.RegistrarAgenda(request);

			var consulta = new SetConsulta
			{
				NumeroComparendo = request.Data.NumeroComparendo,
				Consulta = request.Data.Consulta,
			};

			await _objTransactionDao.RegistrarConsulta(consulta);

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

	public async Task<Response<string>> RegistrarIngresoAgenda(Request<SetAgenda> request)
	{
		var response = new Response<string>();
		try
		{
			response.Result = await _objTransactionDao.RegistrarIngresoAgenda(request);
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

	public async Task<Response<string>> RegistrarSalidaAgenda(Request<SetAgenda> request)
	{
		var response = new Response<string>();
		try
		{
			// Registrar certificado y obtener pago
			var certificado = await _objTransactionDao.RegistrarSalidaAgenda(request);
			var pago = await _objTransactionDao.ObtenerPago(request);

			if (string.IsNullOrEmpty(certificado.Notificacion.Plantilla))
			{
				response.StatusMessage = "No se encuentra plantilla para envío de correo";
				response.Warn(request);
				return response;
			}

			// Preparar correos para enviar
			var emailPago = new EmailServer.EmailParams
			{
				TypeTemplate = EmailServer.TypeTemplate.PagoFinalCurso,
				Subject = $"Recibo de Pago Curso {pago.NombreCurso}",
				EmailTo = new List<string> { pago.CorreoElectronico },
				TemplateAttach = pago.Notificacion?.Plantilla,
				Data = pago
			};

			var emailCertificado = new EmailServer.EmailParams
			{
				TypeTemplate = EmailServer.TypeTemplate.CertificadoCurso,
				Subject = "Certificado Curso",
				EmailTo = new List<string> { pago.CorreoElectronico },
				Template = certificado.Notificacion?.Plantilla,
				TemplateAttach = certificado.Notificacion?.PlantillaAdjunto,
				Data = certificado
			};

			if (!string.IsNullOrEmpty(emailCertificado.TemplateAttach))
			{
				try
				{
					// Reemplazar plantillas y generar PDFs
					emailCertificado.ReplacePlantilla();
					emailPago.ReplacePlantilla();

					var attach = new List<string>
				{
					ExtensionMethods.HtmlTemplateToPDF(emailPago.TemplateAttach, "pago", pago.IdConsecutivoPago.ToString()),
					ExtensionMethods.HtmlTemplateToPDF(emailCertificado.TemplateAttach, "pago", pago.IdConsecutivoPago.ToString()+"-1")
				};

					emailPago.PathAttachment = attach.Any() ? attach : null;

					// Enviar email
					await EmailServer.SendEmail(emailPago);

					// Eliminar los archivos adjuntos después de enviar el correo
					DeleteAttachments(attach);
				}
				catch (Exception ex)
				{
					// Manejo de errores al procesar plantillas o correos
					response.Warn(ex, request);
				}
			}
			else
			{
				response.StatusMessage = "Plantilla adjunta no encontrada para el certificado";
				response.Warn(request);
			}

			response.Result = "Ok";
			response.Success(certificado);
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

	public async Task<Response<bool>> LiberarFupas()
	{
		var response = new Response<bool>();
		try
		{
			response.Result = await _objTransactionDao.LiberarFupas();
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

	#region Métodos Privados

	private void DeleteAttachments(List<string> attachments)
	{
		foreach (var attachment in attachments)
		{
			try
			{
				if (!string.IsNullOrEmpty(attachment) && File.Exists(attachment))
				{
					File.Delete(attachment);
				}

				var carpeta = Path.GetDirectoryName(attachment);
				if (!string.IsNullOrEmpty(carpeta) && Directory.Exists(carpeta))
				{
					Directory.Delete(carpeta, true);
				}
			}
			catch (IOException ioEx)
			{
				// Manejo de excepciones relacionadas con la eliminación de archivos
				Console.WriteLine($"Error al eliminar archivo: {ioEx.Message}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error desconocido al eliminar archivo: {ex.Message}");
			}
		}
	}

	#endregion Métodos Privados
}