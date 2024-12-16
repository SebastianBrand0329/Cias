using ApiCursosCias.Models.Entities;
using ApiCursosCias.Services.Helpers.Settings;
using System.Net;
using System.Net.Mail;

namespace ApiCursosCias.Services.Helpers.Integration;

public static class EmailServer
{
	private static readonly EmailSettings _emailSettings = AppSettings.Settings.EmailSettings;

	public enum TypeTemplate
	{
		RegistroUsuario,
		PagoCurso,
		BienvenidaAgendador,
		CertificadoCurso,
		PagoFinalCurso
	};

	public class EmailParams
	{
		public TypeTemplate TypeTemplate { set; get; }
		public List<string> EmailTo { set; get; }
		public string Subject { set; get; }
		public string Template { set; get; }
		public string TemplateAttach { set; get; }
		public List<string> PathAttachment { set; get; } = new List<string>();
		public List<Attachment> Attachments { set; get; } = new List<Attachment>();
		public object Data { set; get; }
	}

	private static async Task<string> EnviarCorreo(EmailParams emailParams)
	{
		try
		{
			var settings = _emailSettings;
			using var server = new SmtpClient(settings.Server, settings.Port);
			server.DeliveryMethod = SmtpDeliveryMethod.Network;
			server.DeliveryFormat = SmtpDeliveryFormat.International;
			server.UseDefaultCredentials = false;
			server.Credentials = new NetworkCredential(settings.From, settings.Pass);
			server.EnableSsl = true;
			using var mnsj = new MailMessage();
			mnsj.From = new MailAddress(settings.From);
			emailParams.EmailTo?.ForEach(mnsj.To.Add);
			mnsj.Subject = emailParams.Subject;
			mnsj.IsBodyHtml = true;
			mnsj.Body = emailParams.Template;
			emailParams.Attachments?.ForEach(mnsj.Attachments.Add);
			await server.SendMailAsync(mnsj);
			return "OK";
		}
		catch
		{
			throw;
		}
	}

	public static async Task<string> SendEmail(EmailParams emailParams)
	{
		try
		{
			emailParams.PathAttachment?.ForEach(p => emailParams.Attachments.Add(new Attachment(p)));
			return await EnviarCorreo(emailParams);
		}
		catch
		{
			throw;
		}
	}

	public static EmailParams ReplacePlantilla(this EmailParams emailParams)
	{
		switch (emailParams.TypeTemplate)
		{
			case TypeTemplate.PagoCurso:
				var pago = (PagoPdf)emailParams.Data;
				emailParams.TemplateAttach = emailParams.TemplateAttach
					.Replace("[NIT_EMPRESA]", pago.Nit)
					.Replace("[REGIMEN]", pago.Regimen)
					.Replace("[DIRECCION]", pago.Direccion)
					.Replace("[MUNICIPIO]", pago.Municipio)
					.Replace("[CIUDAD]", pago.Municipio)
					.Replace("[NUMERO_PAGO]", pago.IdConsecutivoPago.ToString())
					.Replace("[EMPRESA]", pago.NombreEmpresa)
					.Replace("[NOMBRE_CLIENTE]", pago.NombrePersona)
					.Replace("[DOCUMENTO]", pago.NumeroDocumento.ToString())
					.Replace("[CORREO]", pago.CorreoElectronico)
					.Replace("[NOMBRE_CURSO]", pago.NombreCurso)
					.Replace("[FECHA_CURSO]", pago.FechaCurso)
					.Replace("[HORA_CURSO]", pago.HoraInicioCurso)
					.Replace("[INSTRUCTOR]", pago.NombreDocente)
					.Replace("[COMPARENDO]", pago.NumeroComparendo)
					.Replace("[VALOR_CURSO]", "$" + string.Format("{0:N2}", decimal.Parse(pago.ValorCurso.ToString())))
					.Replace("[SECE]", pago.Sede)
					.Replace("[USUARIOCREACION]", pago.NombreUsuario);

				emailParams.Template = emailParams.Template
					.Replace("[Nombre_Usuario]", pago.NombreUsuario)
					.Replace("[SEDE]", pago.Sede)
					.Replace("[FECHA]", pago.FechaPago.ToString());

				break;

			case TypeTemplate.BienvenidaAgendador:
				var usuario = (Usuario)emailParams.Data;
				emailParams.Template = emailParams.Template
					.Replace("[Nombre_Usuario]", usuario.NombreUsuario)
					.Replace("[Usuario]", usuario.UserName.ToString())
					.Replace("[ClaveTemporal]", usuario.Clave);
				break;

			case TypeTemplate.CertificadoCurso:
				var certificado = (Certificado)emailParams.Data;
				emailParams.TemplateAttach = emailParams.TemplateAttach
					.Replace("[NIT_EMPRESA]", certificado.Nit)
					.Replace("[DIRECCION]", certificado.Direccion)
					.Replace("[MUNICIPIO]", certificado.Municipio)
					.Replace("[CERTIFICADO]", certificado.NumeroCertificado.ToString())
					.Replace("[EMPRESA]", certificado.Empresa)
					.Replace("[NOMBRE_CLIENTE]", certificado.NombrePersona)
					.Replace("[CC]", certificado.TipoDocumento)
					.Replace("[NUMERODOC]", certificado.NumeroDocumento.ToString())
					.Replace("[DESCUENTO]", certificado.PorcentajeDescuento.ToString())
					.Replace("[COMPARENDO]", certificado.NumeroComparendo)
					.Replace("[COMPARENDO]", certificado.NumeroComparendo)
					.Replace("[COMPARENDOMUNICIPIO]", certificado.MunicipioInfraccion)
					.Replace("[INFRACCION]", certificado.CodigoInfraccion)
					.Replace("[FECHA]", certificado.FechaCurso)
					.Replace("[SECE]", certificado.Sede)
					.Replace("[HORA]", certificado.HoraInicioCurso)
					.Replace("[HORAS_DURACION]", certificado.Duracion.ToString())
					.Replace("[INSTRUCTOR]", certificado.NombreDocente)
					.Replace("[USUARIOCREACION]", certificado.Usuario);

				emailParams.Template = emailParams.Template
					.Replace("[Nombre_Usuario]", certificado.NombrePersona)
					.Replace("[SEDE ]", certificado.Sede)
					.Replace("[FECHA]", certificado.FechaCurso);

				break;

			case TypeTemplate.PagoFinalCurso:
				var pagopdf = (PagoPdf)emailParams.Data;
				emailParams.TemplateAttach = emailParams.TemplateAttach
					.Replace("[NIT_EMPRESA]", pagopdf.Nit)
					.Replace("[REGIMEN]", pagopdf.Regimen)
					.Replace("[DIRECCION]", pagopdf.Direccion)
					.Replace("[MUNICIPIO]", pagopdf.Municipio)
					.Replace("[NUMERO_PAGO]", pagopdf.IdConsecutivoPago.ToString())
					.Replace("[EMPRESA]", pagopdf.NombreEmpresa)
					.Replace("[FECHA_LIQUIDACION]", pagopdf.FechaLiquidacion.ToString())
					.Replace("[COMPARENDO]", pagopdf.NumeroComparendo)
					.Replace("[HORA_LIQUIDACION]", pagopdf.HoraLiquidacion.ToString())
					.Replace("[CODIGOINFRACCION]", pagopdf.CodigoInfraccion)
					.Replace("[ORGANISMOCOMPARENDO]", pagopdf.MunicipioInfraccion)
					.Replace("[NOMBRE_CLIENTE]", pagopdf.NombrePersona)
					.Replace("[DIRECCION]", pagopdf.Direccion)
					.Replace("[MUNICIPIO]", pagopdf.Municipio)
					.Replace("[DOCUMENTO]", pagopdf.NumeroDocumento.ToString())
					.Replace("[TELEFONO]", pagopdf.Celular)
					.Replace("[CORREO]", pagopdf.CorreoElectronico)
					.Replace("[VALOR_MULTA]", string.Format("{0:N2}", decimal.Parse(pagopdf.ValorComparendo.ToString())))
					.Replace("[VALOR_CURSO]", string.Format("{0:N2}", decimal.Parse(pagopdf.ValorCurso.ToString())))
					.Replace("[PORCENTAJE]", string.Format("{0:C2}", decimal.Parse(pagopdf.PorcentajeDescuento.ToString())))
					.Replace("[VALOR_DESCUENTO]", string.Format("{0:N2}", decimal.Parse(pagopdf.ValorDescuento.ToString())))
					.Replace("[VALOR_FINAL]", string.Format("{0:N2}", decimal.Parse(pagopdf.ValorTotalPagar.ToString())));
				break;
		}
		return emailParams;
	}
}