using ApiCursosCias.Models.Context.Interfaces;
using ApiCursosCias.Models.Context.Model;
using ApiCursosCias.Models.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Access;

public class PagoDao : IPagoDao
{
	private DataContext _context;
	private readonly IMapper _mapper = MapperBootstrapper.Instance;

	public void SetTransaction(DataContext dataContext)
	{
		_context = dataContext;
	}

	public async Task<PagoPdf> RegistrarPago(Request<Pago> request)
	{
		try
		{
			var valorCurso = await ObtenerValorCurso();
			if (valorCurso == 0)
				throw new ResultException(0, "No se encontró el valor del curso para el año actual");

			var agendaCurso = await ObtenerFupaCurso(request.Data.IdAgendaCurso);
			if (agendaCurso.IdFupa is null)
				throw new ResultException(0, "No se encontró información de la Fupa.");
			else if (agendaCurso.IdCurso is null)
				throw new ResultException(0, "No se encontró información asociada a un curso");

			var idSede = await ObtenerSedeCurso(agendaCurso.IdCurso);
			if (idSede == null)
				throw new ResultException(0, "No se encontró la sede asociada al curso");

			var agenda = await ObtenerAgenda(request.Data.IdAgendaCurso);
			if (agenda == null)
				throw new ResultException(0, "No se encontró agenda");

			agenda.IdEstadoAgenda = 2;
			_context.TrAgendaCursos.Update(agenda);
			await _context.SaveChangesAsync();

			var horaActual = DateTime.Now.ToString("HH:mm:ss");
			request.Data.IdFupa = agendaCurso.IdFupa;

			if (request.Data.PorcentajeDescuento.HasValue && request.Data.ValorComparendo.HasValue)
			{
				request.Data.ValorDescuento = CalcularDescuento(request.Data.PorcentajeDescuento.Value, request.Data.ValorComparendo.Value);
				request.Data.ValorTotalPagar = CalcularValorPagar(request.Data.ValorComparendo.Value, request.Data.ValorDescuento.Value, valorCurso);
			}
			else
				throw new ResultException(0, "El porcentaje de descuento y el valor a pagar deben ser válidos.");

			request.Data.IdSede = idSede;
			request.Data.Horaregistro = horaActual;
			request.Data.ValorCurso = valorCurso;
			var pago = _mapper.Map<TrPagoAgendum>(request.Data);
			pago.IdConsecutivoPago = pago.IdAgendaCurso;
			await _context.TrPagoAgenda.AddAsync(pago);
			await _context.SaveChangesAsync();

			await ActualizarEstadoFupa(agenda.IdFupa);

			var infoPdf = await ObtenerPagoInformativoPdf(pago.IdAgendaCurso);
			infoPdf.Notificacion = await ObtenerPlantillaPdf();

			return infoPdf;
		}
		catch
		{
			throw;
		}
	}

	public async Task<PagoPdf> ObtenerPago(Request<SetAgenda> request)
	{
		try
		{
			var pago = await ObtenerInformacionPdf(request.Data.IdAgendaCurso);
			pago.Notificacion = await ObtenerPdfPago();

			return pago;
		}
		catch
		{
			throw;
		}
	}

	#region Métodos Privados

	private async Task<decimal> ObtenerValorCurso()
	{
		var valor = await _context.MaValorCursos.FirstOrDefaultAsync(x => x.Anno == DateTime.Now.Year & x.Estado.Equals("S"));
		return (decimal)(valor != null ? valor.Valor : 0);
	}

	private async Task<(decimal? IdFupa, int? IdCurso)> ObtenerFupaCurso(int? IdAgenda)
	{
		var result = await _context.TrAgendaCursos.Where(x => x.IdAgendaCurso == IdAgenda & x.IdEstadoAgenda == 1)
												  .Select(x => new { x.IdFupa, x.IdCurso })
												  .FirstOrDefaultAsync();

		return result != null ? (result.IdFupa, result.IdCurso) : (null, null);
	}

	private async Task<int?> ObtenerSedeCurso(int? IdCurso)
	{
		return await _context.TrCursos.Where(x => x.IdCurso == IdCurso)
									  .Select(x => x.IdSede)
									  .FirstOrDefaultAsync();
	}

	private async Task<TrAgendaCurso> ObtenerAgenda(int? IdAgendaCurso)
	{
		return await _context.TrAgendaCursos.FirstOrDefaultAsync(x => x.IdAgendaCurso == IdAgendaCurso);
	}

	private decimal CalcularDescuento(decimal Descuento, decimal ValorPagar)
	{
		if (Descuento < 0.25m || Descuento > 0.5m)
			throw new ArgumentOutOfRangeException(nameof(Descuento), "El descuento debe estar entre 25% y 50%.");

		if (ValorPagar < 0)
			throw new ArgumentOutOfRangeException(nameof(ValorPagar), "El valor a pagar no puede ser negativo.");

		var descuentoCalculado = ValorPagar * Descuento;
		return Math.Round(descuentoCalculado, 2);
	}

	private decimal CalcularValorPagar(decimal ValorPagar, decimal ValorDescuento, decimal ValorCurso)
	{
		if (ValorPagar < 0)
			throw new ArgumentOutOfRangeException(nameof(ValorPagar), "El valor a pagar no puede ser negativo.");

		if (ValorDescuento < 0)
			throw new ArgumentOutOfRangeException(nameof(ValorDescuento), "El descuento no puede ser negativo.");

		var valorFinal = (ValorPagar - ValorDescuento) - ValorCurso;
		return Math.Round(valorFinal, 2);
	}

	private async Task<PagoPdf> ObtenerInformacionPdf(int? IdAgendaCurso)
	{
		var result = await (from curso in _context.TrCursos
							join agenda in _context.TrAgendaCursos on curso.IdCurso equals agenda.IdCurso
							join pago in _context.TrPagoAgenda on agenda.IdAgendaCurso equals pago.IdAgendaCurso
							join usuario in _context.TrUsuarios on pago.IdUsuario equals usuario.IdUsuario
							join estadoAgenda in _context.MaEstadoAgendamientos on agenda.IdEstadoAgenda equals estadoAgenda.IdEstadoAgendamiento
							join sede in _context.MaSedes on curso.IdSede equals sede.IdSede
							join municipio in _context.MaMunicipios on sede.IdMunicipio equals municipio.IdMunicipio
							join empresa in _context.MaEmpresas on sede.IdEmpresa equals empresa.IdEmpresa
							join docente in _context.TrDocentes on curso.IdDocente equals docente.IdDocente
							where agenda.IdAgendaCurso == IdAgendaCurso
							select new PagoPdf
							{
								IdConsecutivoPago = pago.IdConsecutivoPago,
								FechaLiquidacion = DateTime.Now.Date,
								HoraLiquidacion = DateTime.Now.TimeOfDay,
								NumeroDocumento = agenda.NumeroDocumento,
								NombrePersona = agenda.NombrePersona,
								Celular = agenda.Celular,
								CorreoElectronico = agenda.CorreoElectronico,
								FechaCurso = curso.FechaCurso,
								HoraInicioCurso = curso.HoraInicioCurso,
								HoraFinCurso = curso.HoraFinCurso,
								Sede = sede.Sede,
								Direccion = sede.Direccion,
								Municipio = municipio.Municipio,
								NumeroComparendo = agenda.NumeroComparendo,
								CodigoInfraccion = pago.CodigoInfraccion,
								ValorComparendo = pago.ValorComparendo,
								ValorCurso = pago.ValorCurso,
								PorcentajeDescuento = pago.PorcentajeDescuento,
								ValorDescuento = pago.ValorDescuento,
								ValorAPagar = pago.ValorApagar,
								ValorTotalPagar = pago.ValorTotalPagar,
								NombreEmpresa = empresa.Empresa,
								Nit = empresa.Nit,
								NombreUsuario = usuario.NombreUsuario,
								NombreCurso = curso.NombreCurso,
								NombreDocente = docente.NombreDocente,
								Regimen = "Regimen comun"
							}).FirstOrDefaultAsync();

		return result;
	}

	private async Task<PagoPdf> ObtenerPagoInformativoPdf(int? IdAgendaCurso)
	{
		return await (from curso in _context.TrCursos
					  join agenda in _context.TrAgendaCursos on curso.IdCurso equals agenda.IdCurso
					  join pago in _context.TrPagoAgenda on agenda.IdAgendaCurso equals pago.IdAgendaCurso
					  join usuario in _context.TrUsuarios on pago.IdUsuario equals usuario.IdUsuario
					  join sede in _context.MaSedes on curso.IdSede equals sede.IdSede
					  join municipio in _context.MaMunicipios on sede.IdMunicipio equals municipio.IdMunicipio
					  join empresa in _context.MaEmpresas on sede.IdEmpresa equals empresa.IdEmpresa
					  join docente in _context.TrDocentes on curso.IdDocente equals docente.IdDocente
					  where agenda.IdAgendaCurso == IdAgendaCurso
					  select new PagoPdf
					  {
						  Nit = empresa.Nit,
						  Regimen = "Regimen Comun",
						  Direccion = sede.Direccion,
						  Municipio = municipio.Municipio,
						  IdConsecutivoPago = pago.IdConsecutivoPago,
						  NombreEmpresa = empresa.Empresa,
						  NombrePersona = agenda.NombrePersona,
						  NumeroDocumento = agenda.NumeroDocumento,
						  CorreoElectronico = agenda.CorreoElectronico,
						  NombreCurso = curso.NombreCurso,
						  FechaCurso = curso.FechaCurso,
						  HoraInicioCurso = curso.HoraInicioCurso,
						  HoraFinCurso = curso.HoraFinCurso,
						  NombreDocente = docente.NombreDocente,
						  NumeroComparendo = agenda.NumeroComparendo,
						  ValorCurso = pago.ValorCurso,
						  Sede = sede.Sede,
						  NombreUsuario = usuario.NombreUsuario,
						  MunicipioInfraccion = pago.MunicipioInfraccion,
						  FechaPago = pago.FechaPago,
					  }).FirstOrDefaultAsync();
	}

	private async Task<Notificacion> ObtenerPlantillaPdf()
	{
		var plantillas = await _context.MaPlantillas.Where(x => x.IdPlantilla == 5 || x.IdPlantilla == 3)
													.ToListAsync();

		var plantilla = plantillas.FirstOrDefault(x => x.IdPlantilla == 5);
		var plantillaPdf = plantillas.FirstOrDefault(x => x.IdPlantilla == 3);

		var notificacion = _mapper.Map<Notificacion>(plantilla);
		notificacion.PlantillaAdjunto = plantillaPdf.Plantilla;

		return notificacion;
	}

	private async Task<Notificacion> ObtenerPdfPago()
	{
		var plantilla = await _context.MaPlantillas.FirstOrDefaultAsync(x => x.IdPlantilla == 6);
		return _mapper.Map<Notificacion>(plantilla);
	}

	public async Task<bool> ActualizarEstadoFupa(decimal? IdFupa)
	{
		var fupa = await _context.MaFupas.FirstOrDefaultAsync(x => x.IdFupa == IdFupa);
		if (fupa == null)
			return false;

		fupa.IdestadoFupa = 2;
		_context.MaFupas.Update(fupa);
		await _context.SaveChangesAsync();

		return true;
	}

	#endregion Métodos Privados
}