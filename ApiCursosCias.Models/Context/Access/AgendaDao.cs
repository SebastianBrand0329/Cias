using ApiCursosCias.Models.Context.Interfaces;
using ApiCursosCias.Models.Context.Model;
using ApiCursosCias.Models.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ApiCursosCias.Models.Context.Access;

public class AgendaDao : IAgendaDao
{
	private DataContext _context;
	private readonly IMapper _mapper = MapperBootstrapper.Instance;

	public void SetTransaction(DataContext dataContext)
	{
		_context = dataContext;
	}

	public async Task<List<GetAgenda>> ObtenerAgendaSede(Request<Usuario> request)
	{
		try
		{
			var idSede = await ObtenerSede(request.Data.IdUsuario);
			if (idSede is null || idSede == 0)
				throw new ResultException(0, "Error, no existe información con la sede asociada");

			var result = await (from agenda in _context.TrAgendaCursos
								join curso in _context.TrCursos on agenda.IdCurso equals curso.IdCurso
								join tipoDocumento in _context.MaTipoDocumentos on agenda.IdTipoDocumento equals tipoDocumento.IdTipoDocumento
								join sede in _context.MaSedes on curso.IdSede equals sede.IdSede
								join empresa in _context.MaEmpresas on sede.IdEmpresa equals empresa.IdEmpresa
								join docente in _context.TrDocentes on curso.IdDocente equals docente.IdDocente
								where curso.IdSede == idSede && agenda.FechaRegistro.Value.Date == DateTime.Now.Date
								select new GetAgenda
								{
									IdTipoDocumento = tipoDocumento.IdTipoDocumento,
									NumeroDocumento = agenda.NumeroDocumento,
									NombrePersona = agenda.NombrePersona,
									Celular = agenda.Celular,
									CorreoElectronico = agenda.CorreoElectronico,
									IdCurso = curso.IdCurso,
									NombreCurso = curso.NombreCurso,
									FechaCurso = curso.FechaCurso,
									HoraInicioCurso = curso.HoraInicioCurso,
									HoraFinCurso = curso.HoraFinCurso,
									IdSede = curso.IdSede,
									FechaRegistro = agenda.FechaRegistro,
									NombreDocente = docente.NombreDocente,
									IdDocente = docente.IdDocente,
									IdAgendaCurso = agenda.IdAgendaCurso,
									TipoDocumento = tipoDocumento.TipoDocumento
								}).ToListAsync();

			return result;
		}
		catch
		{
			throw;
		}
	}

	public async Task<string> RegistrarAgenda(Request<SetAgenda> request)
	{
		if (await ValidarUsuarioCurso(request.Data))
			throw new ResultException(0, "El usuario ya se encuentra inscrito en este curso");

		if (await ValidarComparendoCurso(request.Data))
			throw new ResultException(0, "El comparendo ya se encuentra registrado en este curso");

		if (await ValidarComparendoRegistrado(request.Data))
			throw new ResultException(0, "El comparendo ya realizó un curso");

		var dias = await ValidarFechaCurso(request.Data.FechaComparendo);
		if (!await ValidarDiasComparendo(request.Data, dias))
			throw new ResultException(0, "La fecha del comparendo supera los días para el descuento");

		var capacidad = await ObtenerCapacidadCurso(request.Data.IdCurso);
		if (capacidad == 0)
			throw new ResultException(0, "El curso seleccionado no cuenta con disponibilidad de agendamiento");

		var idEmpresa = await ObtenerEmpresaUsuario(request.Data.IdUsuario);
		if (idEmpresa == 0)
			throw new ResultException(0, "El usuario no se encuentra asociado a una empresa");

		if (await ObtenerCantidadFupasEmpresas(idEmpresa) == 0)
			throw new ResultException(0, "La empresa no cuenta con Fupas disponibles en el momento");

		var fupa = await ObtenerFupa(idEmpresa);
		request.Data.IdFupa = fupa;
		request.Data.IdEstadoAgenda = 1;

		var valorCurso = await ObtenerValorCurso();
		if (valorCurso == 0)
			throw new ResultException(0, "No se encontró el valor del curso para el año actual");

		request.Data.ValorCurso = valorCurso;

		var agenda = _mapper.Map<TrAgendaCurso>(request.Data);
		await _context.TrAgendaCursos.AddAsync(agenda);
		await _context.SaveChangesAsync();

		await ActualizarFupa(agenda.IdFupa);
		await ActualizarOcupacionCurso(request.Data.IdCurso);

		return "Ok";
	}

	public async Task<bool> RegistrarConsulta(SetConsulta request)
	{
		try
		{
			var consulta = _mapper.Map<TrConsultaComparendo>(request);
			await _context.TrConsultaComparendos.AddAsync(consulta);
			await _context.SaveChangesAsync();
			return true;
		}
		catch
		{
			throw;
		}
	}

	public async Task<string> RegistrarIngresoAgenda(Request<SetAgenda> request)
	{
		try
		{
			var agenda = await _context.TrAgendaCursos.FirstOrDefaultAsync(x => x.IdAgendaCurso == request.Data.IdAgendaCurso &
																				x.IdEstadoAgenda == 2);

			if (agenda == null)
				throw new ResultException(0, "No puede realizar el ingreso al curso, debe realizar el pago.");

			agenda.IdEstadoAgenda = 3;
			_context.TrAgendaCursos.Update(agenda);
			await _context.SaveChangesAsync();
			return "Ok";
		}
		catch
		{
			throw;
		}
	}

	public async Task<Certificado> RegistrarSalidaAgenda(Request<SetAgenda> request)
	{
		try
		{
			var agenda = await _context.TrAgendaCursos.FirstOrDefaultAsync(x => x.IdAgendaCurso == request.Data.IdAgendaCurso &
																				x.IdEstadoAgenda == 3);
			if (agenda == null)
				throw new ResultException(0, "No puede realizar la salida al curso, usted no realizó el ingreso al mismo.");

			agenda.IdEstadoAgenda = 4;
			_context.TrAgendaCursos.Update(agenda);
			await _context.SaveChangesAsync();

			var certificado = await ObtenerCertificadoPdf(agenda.IdAgendaCurso);
			certificado.Notificacion = await ObtenerPlantillaPdf();

			return certificado;
		}
		catch
		{
			throw;
		}
	}

	public async Task<bool> LiberarFupas()
	{
		try
		{
			var fupas = ObtenerListadoFupasLiberar();
			if (fupas == null)
				return false;

			await _context.TrAgendaCursoHsts.AddRangeAsync(fupas);
			var items = fupas.Select(fupa => new TrAgendaCurso
			{
				IdAgendaCurso = fupa.IdAgendaCurso,
				IdFupa = fupa.IdFupa,
				IdCurso = fupa.IdCurso,
				IdTipoDocumento = fupa.IdTipoDocumento,
				NumeroDocumento = fupa.NumeroDocumento,
				NombrePersona = fupa.NombrePersona,
				Celular = fupa.Celular,
				CorreoElectronico = fupa.CorreoElectronico,
				NumeroComparendo = fupa.NumeroComparendo,
				FechaRegistro = fupa.FechaRegistro,
				IdUsuarioRegistro = fupa.IdUsuarioRegistro,
				IdCanal = fupa.IdCanal,
				IdEstadoAgenda = fupa.IdEstadoAgenda,
				ValorCurso = fupa.ValorCurso,
				ValorComparendo = fupa.ValorComparendo,
				PorcentajeDescuento = fupa.PorcentajeDescuento
			}).ToList();
			//var items = _mapper.Map<List<TrAgendaCurso>>(fupas);
			_context.TrAgendaCursos.RemoveRange(items);

			var idFupas = items.Select(item => item.IdFupa).ToList();

			var ActualizarFupas = await _context.MaFupas.Where(x => idFupas.Contains(x.IdFupa))
													   .ToListAsync();

			ActualizarFupas.ForEach(x => x.IdestadoFupa = 0);
			_context.MaFupas.UpdateRange(ActualizarFupas);
			await _context.SaveChangesAsync();

			return true;
		}
		catch
		{
			throw;
		}
	}

	#region Métodos Privados

	private async Task<int?> ObtenerSede(int? IdUsuario)
	{
		var result = await _context.TrUsuarios.FirstOrDefaultAsync(x => x.IdUsuario == IdUsuario);
		return result != null ? result.IdSede : 0;
	}

	private async Task<bool> ValidarUsuarioCurso(SetAgenda agenda)
	{
		return await _context.TrAgendaCursos.FirstOrDefaultAsync(x => x.IdCurso == agenda.IdCurso &
																	  x.NumeroDocumento == agenda.NumeroDocumento) != null ? true : false;
	}

	private async Task<bool> ValidarComparendoCurso(SetAgenda agenda)
	{
		return await _context.TrAgendaCursos.AnyAsync(x => x.NumeroComparendo == agenda.NumeroComparendo & x.IdCurso == agenda.IdCurso);
	}

	public async Task<bool> ValidarComparendoRegistrado(Agenda agenda)
	{
		return !await _context.TrAgendaCursos.AnyAsync(x => x.NumeroComparendo == agenda.NumeroComparendo & x.IdEstadoAgenda != 4);
	}

	private async Task<int> ValidarFechaCurso(DateTime date)
	{
		DateTime inicio = new DateTime(date.Year, date.Month, date.Day);
		DateTime fin = DateTime.Now.Date;
		var diasHabiles = new List<DateTime>();
		if ((fin - inicio).Days <= 50)
			diasHabiles = await ObtenerDiasHabiles(inicio, fin);

		return diasHabiles.Count;
	}

	private async Task<List<DateTime>> ObtenerDiasHabiles(DateTime inicio, DateTime fin)
	{
		List<DateTime> diasHabiles = new List<DateTime>();

		for (DateTime fecha = inicio; fecha <= fin; fecha = fecha.AddDays(1))
		{
			if (await EsDiaHabil(fecha))
				diasHabiles.Add(fecha);
		}

		return diasHabiles;
	}

	private async Task<bool> EsDiaHabil(DateTime fecha)
	{
		var festivos = await _context.MaDiaFestivos.Where(x => x.Fecha == DateOnly.FromDateTime(fecha) && x.Estado.Equals("S")).FirstOrDefaultAsync();
		return fecha.DayOfWeek >= DayOfWeek.Monday && fecha.DayOfWeek <= DayOfWeek.Friday && festivos is null;
	}

	private async Task<bool> ValidarDiasComparendo(SetAgenda agenda, int dias)
	{
		return await _context.MaTiemposDescuentos.AnyAsync(x => x.IdTipoComparendo == agenda.IdTipoComparendo &&
																x.DiasInicial <= dias &&
																x.DiasFinal >= dias);
	}

	private async Task<int?> ObtenerCapacidadCurso(int? idCurso)
	{
		var curso = await _context.TrCursos.Where(x => x.IdCurso == idCurso)
										   .Select(x => new { x.CapacidadCurso, x.Ocupado })
										   .FirstOrDefaultAsync();

		return curso != null ? curso.CapacidadCurso - curso.Ocupado : 0;
	}

	private async Task<int?> ObtenerEmpresaUsuario(int? IdUsuario)
	{
		var usuario = await _context.TrUsuarios.FirstOrDefaultAsync(x => x.IdUsuario == IdUsuario);

		return usuario != null ? usuario.IdEmpresa : 0;
	}

	private async Task<int?> ObtenerCantidadFupasEmpresas(int? IdEmpresa)
	{
		var fupas = await _context.MaFupas.Where(x => x.IdEmpresa == IdEmpresa & x.IdestadoFupa == 0).ToListAsync();

		return fupas != null ? fupas.Count : 0;
	}

	private async Task<decimal> ObtenerFupa(int? IdEmpresa)
	{
		var fupa = await _context.MaFupas.FirstOrDefaultAsync(x => x.IdEmpresa == IdEmpresa & x.IdestadoFupa == 0);

		return fupa != null ? fupa.IdFupa : 0;
	}

	private async Task<bool> ActualizarFupa(decimal? idFupa)
	{
		var fupa = await _context.MaFupas.FirstOrDefaultAsync(x => x.IdFupa == idFupa);

		if (fupa == null)
			return false;

		fupa.IdestadoFupa = 1;

		_context.MaFupas.Update(fupa);
		await _context.SaveChangesAsync();
		return true;
	}

	private async Task<bool> ActualizarOcupacionCurso(int? IdCurso)
	{
		var curso = await _context.TrCursos.FirstOrDefaultAsync(x => x.IdCurso == IdCurso & x.Estado.Equals("S"));

		if (curso == null) return false;

		curso.Ocupado += 1;
		_context.TrCursos.Update(curso);
		await _context.SaveChangesAsync();
		return true;
	}

	private async Task<decimal> ObtenerValorCurso()
	{
		var valor = await _context.MaValorCursos.FirstOrDefaultAsync(x => x.Anno == DateTime.Now.Year && x.Estado.Equals("S"));
		return (decimal)(valor != null ? valor.Valor : 0);
	}

	private async Task<Certificado> ObtenerCertificadoPdf(int? IdAgendaCurso)
	{
		return await (from curso in _context.TrCursos
					  join agenda in _context.TrAgendaCursos on curso.IdCurso equals agenda.IdCurso
					  join tipoDoc in _context.MaTipoDocumentos on agenda.IdTipoDocumento equals tipoDoc.IdTipoDocumento
					  join pago in _context.TrPagoAgenda on agenda.IdAgendaCurso equals pago.IdAgendaCurso
					  join usuario in _context.TrUsuarios on pago.IdUsuario equals usuario.IdUsuario
					  join estado in _context.MaEstadoAgendamientos on agenda.IdEstadoAgenda equals estado.IdEstadoAgendamiento
					  join sede in _context.MaSedes on curso.IdSede equals sede.IdSede
					  join municipio in _context.MaMunicipios on sede.IdMunicipio equals municipio.IdMunicipio
					  join empresa in _context.MaEmpresas on sede.IdEmpresa equals empresa.IdEmpresa
					  join docente in _context.TrDocentes on curso.IdDocente equals docente.IdDocente
					  where agenda.IdAgendaCurso == IdAgendaCurso
					  select new { curso, agenda, empresa, sede, municipio, pago, usuario, tipoDoc, docente }
					  ).Select(x => new Certificado
					  {
						  IdFupa = x.agenda.IdFupa,
						  Empresa = x.empresa.Empresa,
						  Nit = x.empresa.Nit,
						  Sede = x.sede.Sede,
						  Direccion = x.sede.Direccion,
						  Municipio = x.municipio.Municipio,
						  NombrePersona = x.agenda.NombrePersona,
						  NumeroDocumento = x.agenda.NumeroDocumento,
						  PorcentajeDescuento = x.pago.PorcentajeDescuento,
						  NumeroComparendo = x.agenda.NumeroComparendo,
						  CodigoInfraccion = x.pago.CodigoInfraccion,
						  FechaCurso = x.curso.FechaCurso,
						  HoraInicioCurso = x.curso.HoraInicioCurso,
						  Duracion = x.curso.Duracion,
						  Usuario = x.usuario.NombreUsuario,
						  MunicipioInfraccion = x.pago.MunicipioInfraccion,
						  NumeroCertificado = x.agenda.IdAgendaCurso,
						  TipoDocumento = x.tipoDoc.TipoDocumento,
						  NombreDocente = x.docente.NombreDocente,
					  }).FirstOrDefaultAsync();
	}

	private async Task<Notificacion> ObtenerPlantillaPdf()
	{
		var plantillas = await _context.MaPlantillas.Where(x => x.IdPlantilla == 4 || x.IdPlantilla == 2)
													.ToListAsync();

		var plantilla = plantillas.FirstOrDefault(x => x.IdPlantilla == 2);
		var plantillaPdf = plantillas.FirstOrDefault(x => x.IdPlantilla == 4);

		var notificacion = _mapper.Map<Notificacion>(plantilla);
		notificacion.PlantillaAdjunto = plantillaPdf.Plantilla;

		return notificacion;
	}

	private List<TrAgendaCursoHst> ObtenerListadoFupasLiberar()
	{
		var fupas = (from agenda in _context.TrAgendaCursos
					 join curso in _context.TrCursos on agenda.IdCurso equals curso.IdCurso
					 where agenda.IdEstadoAgenda == 1
					 select new { curso, agenda })
					 .AsEnumerable()
					 .Where(x => (DateTime.Parse(x.curso.FechaCurso) == DateTime.Now & DateTime.Now.TimeOfDay > TimeSpan.Parse(x.curso.HoraInicioCurso)) ||
								 (DateTime.Parse(x.curso.FechaCurso) < DateTime.Now))
					 .Select(x => new TrAgendaCursoHst
					 {
						 IdAgendaCurso = x.agenda.IdAgendaCurso,
						 IdCurso = x.agenda.IdCurso,
						 IdTipoDocumento = x.agenda.IdTipoDocumento,
						 NumeroDocumento = x.agenda.NumeroDocumento,
						 NombrePersona = x.agenda.NombrePersona,
						 Celular = x.agenda.Celular,
						 CorreoElectronico = x.agenda.CorreoElectronico,
						 NumeroComparendo = x.agenda.NumeroComparendo,
						 FechaRegistro = x.agenda.FechaRegistro,
						 IdUsuarioRegistro = x.agenda.IdUsuarioRegistro,
						 IdCanal = x.agenda.IdCanal,
						 IdEstadoAgenda = x.agenda.IdEstadoAgenda,
						 IdFupa = x.agenda.IdFupa,
						 ValorCurso = x.agenda.ValorCurso,
						 ValorComparendo = x.agenda.ValorComparendo,
						 PorcentajeDescuento = x.agenda.PorcentajeDescuento
					 }).ToList();

		return fupas;
	}

	#endregion Métodos Privados
}