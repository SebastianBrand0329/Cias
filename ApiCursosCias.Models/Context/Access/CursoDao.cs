using ApiCursosCias.Models.Context.Interfaces;
using ApiCursosCias.Models.Context.Model;
using ApiCursosCias.Models.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Access;

public class CursoDao : ICursoDao
{
	private DataContext _context;
	private readonly IMapper _mapper = MapperBootstrapper.Instance;

	public void SetTransaction(DataContext dataContext)
	{
		_context = dataContext;
	}

	public async Task<Curso> RegistrarCurso(Request<Curso> request)
	{
		try
		{
			if (await ValidarDocenteCurso(request.Data))
				throw new ResultException(0, "Error, el docente ya tiene un curso asignado en esa fecha y ese horario");
			if (!ValidarHorarioDocente(request.Data))
				throw new ResultException(0, "Error, el docente ya tiene un curso asignado en esa franja horaria");

			var curso = _mapper.Map<TrCurso>(request.Data);
			await _context.TrCursos.AddAsync(curso);
			await _context.SaveChangesAsync();

			return request.Data;
		}
		catch (Exception)
		{
			throw;
		}
	}

	public async Task<Curso> ActualizarCurso(Request<Curso> request)
	{
		try
		{
			switch (request.Data.Estado)
			{
				case "S":
					var curso = await ObtenerCurso(request.Data.IdCurso);
					if (curso == null)
						throw new ResultException(0, "Error, El curso enviado no existe");

					if (await ValidarUsuarioCurso(request.Data.IdCurso) == 1)
						throw new ResultException(0, "Error, El curso no se puede actualizar, ya hay usuarios agendados a este curso");
					if (!curso.FechaCurso.Equals(request.Data.FechaCurso) || !curso.HoraInicioCurso.Equals(request.Data.HoraInicioCurso) || curso.HoraFinCurso.Equals(request.Data.HoraInicioCurso))
					{
						if (await ValidarDocenteCurso(request.Data))
							throw new ResultException(0, "Error, el docente ya tiene un curso asignado en esa fecha y ese horario");
						if (!ValidarHorarioDocente(request.Data))
							throw new ResultException(0, "Error, el docente ya tiene un curso asignado en esa franja horaria");
					}

					curso.NombreCurso = request.Data.NombreCurso;
					curso.FechaCurso = request.Data.FechaCurso;
					curso.HoraInicioCurso = request.Data.HoraInicioCurso;
					curso.HoraFinCurso = request.Data.HoraFinCurso;
					curso.IdSede = request.Data.IdSede;
					curso.IdDocente = request.Data.IdDocente;
					curso.CapacidadCurso = request.Data.IdDocente;
					curso.IdUsuarioRegistro = request.Data.IdUsuario;

					_context.TrCursos.Update(curso);
					await _context.SaveChangesAsync();

					break;

				case "N":
					var EliminarCurso = await ObtenerCurso(request.Data.IdCurso);
					if (EliminarCurso == null)
						throw new ResultException(0, "Error, El curso enviado no existe");
					if (await ValidarUsuarioCurso(request.Data.IdCurso) == 1)
						throw new ResultException(0, "Error, El curso no se puede eliminar, ya hay usuarios agendados a este curso");
					EliminarCurso.Estado = "N";

					_context.TrCursos.Update(EliminarCurso);
					await _context.SaveChangesAsync();
					break;
			}

			return request.Data;
		}
		catch
		{
			throw;
		}
	}

	public async Task<List<GetCurso>> ObtenerCursos(Request<Curso> request)
	{
		try
		{
			var IdEmpresa = await ObtenerEmpresa(request.Data.IdUsuario);
			if (IdEmpresa == null || IdEmpresa == 0)
				throw new ResultException(0, "No existe información con ese id usuario");

			var result = (from curso in _context.TrCursos
						  join sede in _context.MaSedes on curso.IdSede equals sede.IdSede
						  join empresa in _context.MaEmpresas on sede.IdEmpresa equals empresa.IdEmpresa
						  join docente in _context.TrDocentes on curso.IdDocente equals docente.IdDocente
						  where sede.IdEmpresa == IdEmpresa & curso.Estado.Equals("S")
						  select new { curso, sede, docente }
								)
								.AsEnumerable()
								.Where(x => DateTime.Parse(x.curso.FechaCurso) >= DateTime.Now.Date)
								.Select(x => new GetCurso
								{
									IdCurso = x.curso.IdCurso,
									NombreCurso = x.curso.NombreCurso,
									FechaCurso = x.curso.FechaCurso,
									HoraInicioCurso = x.curso.HoraInicioCurso,
									HoraFinCurso = x.curso.HoraFinCurso,
									IdSede = x.curso.IdSede,
									Sede = x.sede.Sede,
									IdDocente = x.curso.IdDocente,
									Docente = x.docente.NombreDocente,
									CapacidadCurso = x.curso.CapacidadCurso,
									Estado = x.curso.Estado,
								}).ToList();

			return result;
		}
		catch
		{
			throw;
		}
	}

	public async Task<List<GetCurso>> ObtenerCursosDia(Request<Curso> request)
	{
		try
		{
			var IdSede = await ObtenerSede(request.Data.IdUsuario);
			if (IdSede == null)
				throw new ResultException(0, "El usuario no se encuentra asociado a una sede");

			var query = (from curso in _context.TrCursos
						 join agenda in _context.TrAgendaCursos
							on curso.IdCurso equals agenda.IdCurso into agendaGroup
						 from agenda in agendaGroup.DefaultIfEmpty()
						 join sede in _context.MaSedes on curso.IdSede equals sede.IdSede
						 join empresa in _context.MaEmpresas on sede.IdEmpresa equals empresa.IdEmpresa
						 join docente in _context.TrDocentes on curso.IdDocente equals docente.IdDocente
						 where sede.IdSede == IdSede
						 select new { curso, sede, docente, agenda = agenda ?? new TrAgendaCurso() })
			.AsEnumerable()
			.Where(x => DateTime.Parse(x.curso.FechaCurso) == DateTime.Now.Date &&
						TimeSpan.Parse(x.curso.HoraFinCurso) >= DateTime.Now.TimeOfDay);

			if (request.Data.NumeroDocumento != 0 && request.Data.NumeroDocumento != null)
			{
				query = query.Where(x => x.agenda != null &
										 x.agenda.NumeroDocumento == request.Data.NumeroDocumento &
										 x.agenda.IdTipoDocumento == request.Data.IdTipoDocumento);
			}

			var result = query.Select(x => new GetCurso
			{
				IdCurso = x.curso.IdCurso,
				NombreCurso = x.curso.NombreCurso,
				FechaCurso = x.curso.FechaCurso,
				HoraInicioCurso = x.curso.HoraInicioCurso,
				HoraFinCurso = x.curso.HoraFinCurso,
				IdSede = x.curso.IdSede,
				Sede = x.sede.Sede,
				Direccion = x.sede.Direccion,
				IdDocente = x.curso.IdDocente,
				Docente = x.docente.NombreDocente,
				CapacidadCurso = x.curso.CapacidadCurso
			}).ToList();

			return result;
		}
		catch
		{
			throw;
		}
	}

	public async Task<List<GetListadoCurso>> ObtenerAgendamientoCursos(Request<Curso> request)
	{
		try
		{
			var query = from curso in _context.TrCursos
						join agenda in _context.TrAgendaCursos on curso.IdCurso equals agenda.IdCurso
						join estado in _context.MaEstadoAgendamientos on agenda.IdEstadoAgenda equals estado.IdEstadoAgendamiento
						join sede in _context.MaSedes on curso.IdSede equals sede.IdSede
						join empresa in _context.MaEmpresas on sede.IdEmpresa equals empresa.IdEmpresa
						join docente in _context.TrDocentes on curso.IdDocente equals docente.IdDocente
						where agenda.IdCurso == request.Data.IdCurso && curso.Estado.Equals("S")
						select new { curso, agenda, sede, docente, estado };

			var result = await Task.Run(() => query.AsEnumerable()
												.Where(x => DateTime.Parse(x.curso.FechaCurso) == DateTime.Now.Date &&
						(x.agenda.IdEstadoAgenda == 1 && TimeSpan.Parse(x.curso.HoraFinCurso) >= DateTime.Now.TimeOfDay) ||
						x.agenda.IdEstadoAgenda == 2)
												.Select(x => new GetListadoCurso
												{
													IdCurso = x.curso.IdCurso,
													NombreCurso = x.curso.NombreCurso,
													FechaCurso = x.curso.FechaCurso,
													HoraInicioCurso = x.curso.HoraInicioCurso,
													HoraFinCurso = x.curso.HoraFinCurso,
													IdSede = x.curso.IdSede,
													Sede = x.sede.Sede,
													Direccion = x.sede.Direccion,
													IdDocente = x.curso.IdDocente,
													Docente = x.docente.NombreDocente,
													CapacidadCurso = x.curso.CapacidadCurso,
													IdFupa = x.agenda.IdFupa,
													IdTipoDocumento = x.agenda.IdTipoDocumento,
													NumeroDocumento = x.agenda.NumeroDocumento,
													NombrePersona = x.agenda.NombrePersona,
													NumeroComparendo = x.agenda.NumeroComparendo,
													Celular = x.agenda.Celular,
													CorreoElectronico = x.agenda.CorreoElectronico,
													IdAgendaCurso = x.agenda.IdAgendaCurso,
													IdEstadoAgenda = x.agenda.IdEstadoAgenda,
													Estado = x.estado.EstadoAgendamiento,
												}).ToList());

			return result;
		}
		catch
		{
			throw;
		}
	}

	public async Task<List<GetListadoCurso>> ObtenerCursosUsuario(Request<SetAgenda> request)
	{
		try
		{
			var query = from curso in _context.TrCursos
						join agenda in _context.TrAgendaCursos on curso.IdCurso equals agenda.IdCurso
						join tipoDoc in _context.MaTipoDocumentos on agenda.IdTipoDocumento equals tipoDoc.IdTipoDocumento
						join estado in _context.MaEstadoAgendamientos on agenda.IdEstadoAgenda equals estado.IdEstadoAgendamiento
						join sede in _context.MaSedes on curso.IdSede equals sede.IdSede
						join empresa in _context.MaEmpresas on sede.IdEmpresa equals empresa.IdEmpresa
						join docente in _context.TrDocentes on curso.IdDocente equals docente.IdDocente
						where /*agenda.IdCurso == request.Data.IdCurso &&*/ /*curso.Estado.Equals("S") &*/ agenda.IdEstadoAgenda == 1 & agenda.NumeroDocumento == request.Data.NumeroDocumento
						select new { curso, agenda, sede, docente, estado, tipoDoc };

			var result = await Task.Run(() => query.AsEnumerable()
					 .Where(x => DateTime.Now.Date <= DateTime.Parse(x.curso.FechaCurso))
					 .Select(x => new GetListadoCurso
					 {
						 IdCurso = x.curso.IdCurso,
						 NombreCurso = x.curso.NombreCurso,
						 FechaCurso = x.curso.FechaCurso,
						 HoraInicioCurso = x.curso.HoraInicioCurso,
						 HoraFinCurso = x.curso.HoraFinCurso,
						 IdSede = x.curso.IdSede,
						 Sede = x.sede.Sede,
						 Direccion = x.sede.Direccion,
						 IdDocente = x.curso.IdDocente,
						 Docente = x.docente.NombreDocente,
						 CapacidadCurso = x.curso.CapacidadCurso,
						 IdFupa = x.agenda.IdFupa,
						 IdTipoDocumento = x.agenda.IdTipoDocumento,
						 NumeroDocumento = x.agenda.NumeroDocumento,
						 NombrePersona = x.agenda.NombrePersona,
						 NumeroComparendo = x.agenda.NumeroComparendo,
						 Celular = x.agenda.Celular,
						 CorreoElectronico = x.agenda.CorreoElectronico,
						 IdAgendaCurso = x.agenda.IdAgendaCurso,
						 IdEstadoAgenda = x.agenda.IdEstadoAgenda,
						 Estado = x.estado.EstadoAgendamiento,
						 TipoDocumento = x.tipoDoc.TipoDocumento,
						 ValorCurso = x.agenda.ValorCurso,
						 ValorComparendo = x.agenda.ValorComparendo,
						 PorcentajeDescuento = x.agenda.PorcentajeDescuento,
					 }).ToList());

			return result;
		}
		catch
		{
			throw;
		}
	}

	public async Task<List<GetCurso>> ObtenerCursosFecha(Request<SetAgenda> request)
	{
		try
		{
			var IdSede = await ObtenerSede(request.Data.IdUsuario);
			if (IdSede == null)
				throw new ResultException(0, "El usuario no se encuentra asociado a una sede");

			var fecha = await ValidarFechaCurso(request.Data.FechaComparendo, request.Data.IdTipoComparendo);
			if (fecha == null)
				throw new ResultException(0, "No se pudo validar la fecha del comparendo");

			var valorCurso = await ObtenerValorCurso();
			if (valorCurso == 0)
				throw new ResultException(0, "No se encontró el valor del curso para el año actual");

			var result = (from curso in _context.TrCursos
						  join sede in _context.MaSedes on curso.IdSede equals sede.IdSede
						  join empresa in _context.MaEmpresas on sede.IdEmpresa equals empresa.IdEmpresa
						  join docente in _context.TrDocentes on curso.IdDocente equals docente.IdDocente
						  where sede.IdSede == IdSede & curso.Estado.Equals("S") & (curso.CapacidadCurso - curso.Ocupado) > 0
						  select new { curso, sede, docente }
						 )
						.AsEnumerable()
						.OrderBy(x => x.curso.FechaCurso)
						.Where(x => (DateTime.Parse(x.curso.FechaCurso) == DateTime.Now.Date && TimeSpan.Parse(x.curso.HoraInicioCurso) >= DateTime.Now.TimeOfDay) ||
								(DateTime.Parse(x.curso.FechaCurso) > DateTime.Now.Date && DateTime.Parse(x.curso.FechaCurso) <= fecha)
			)
						.Select(x => new GetCurso
						{
							IdCurso = x.curso.IdCurso,
							NombreCurso = x.curso.NombreCurso,
							FechaCurso = x.curso.FechaCurso,
							HoraInicioCurso = x.curso.HoraInicioCurso,
							HoraFinCurso = x.curso.HoraFinCurso,
							IdSede = x.curso.IdSede,
							Sede = x.sede.Sede,
							IdDocente = x.curso.IdDocente,
							Docente = x.docente.NombreDocente,
							CapacidadCurso = x.curso.CapacidadCurso,
							Ocupacion = x.curso.Ocupado,
							Direccion = x.sede.Direccion,
							ValorCurso = valorCurso,
							PorcentajeDescuento = CalcularPorcentaje(x.curso.FechaCurso, request.Data.FechaComparendo, request.Data.IdTipoComparendo)
						}).ToList();

			return result;
		}
		catch
		{
			throw;
		}
	}

	public async Task<List<GetListadoCurso>> ObtenerCursosIdentificacion(Request<SetAgenda> request)
	{
		try
		{
			var IdSede = await ObtenerSede(request.Data.IdUsuario);
			if (IdSede is null || IdSede == 0)
				throw new ResultException(0, "El usuario no se encuentra asociado a una sede");

			var result = await (from curso in _context.TrCursos
								join agenda in _context.TrAgendaCursos on curso.IdCurso equals agenda.IdCurso
								join tipoDoc in _context.MaTipoDocumentos on agenda.IdTipoDocumento equals tipoDoc.IdTipoDocumento
								join estado in _context.MaEstadoAgendamientos on agenda.IdEstadoAgenda equals estado.IdEstadoAgendamiento
								join sede in _context.MaSedes on curso.IdSede equals sede.IdSede
								join empresa in _context.MaEmpresas on sede.IdEmpresa equals empresa.IdEmpresa
								join docente in _context.TrDocentes on curso.IdDocente equals docente.IdDocente
								where agenda.IdEstadoAgenda == 4 & curso.IdSede == IdSede &
								curso.Estado.Equals("S") & agenda.NumeroDocumento == request.Data.NumeroDocumento
								select new { curso, agenda, sede, tipoDoc }
						  ).Select(x => new GetListadoCurso
						  {
							  IdFupa = x.agenda.IdFupa,
							  IdTipoDocumento = x.agenda.IdTipoDocumento,
							  NumeroDocumento = x.agenda.NumeroDocumento,
							  NombrePersona = x.agenda.NombrePersona,
							  NombreCurso = x.curso.NombreCurso,
							  FechaCurso = x.curso.FechaCurso,
							  HoraInicioCurso = x.curso.HoraInicioCurso,
							  HoraFinCurso = x.curso.HoraFinCurso,
							  IdSede = x.curso.IdSede,
							  Sede = x.sede.Sede,
							  Direccion = x.sede.Direccion,
							  NumeroComparendo = x.agenda.NumeroComparendo,
							  IdAgendaCurso = x.agenda.IdAgendaCurso,
							  TipoDocumento = x.tipoDoc.TipoDocumento
						  }).ToListAsync();

			return result;
		}
		catch
		{
			throw;
		}
	}

	public async Task<List<GetListadoCurso>> ObtenerCursosEstado(Request<SetAgenda> request)
	{
		try
		{
			var result = await (from curso in _context.TrCursos
								join agenda in _context.TrAgendaCursos on curso.IdCurso equals agenda.IdCurso
								join tipoDoc in _context.MaTipoDocumentos on agenda.IdTipoDocumento equals tipoDoc.IdTipoDocumento
								join estado in _context.MaEstadoAgendamientos on agenda.IdEstadoAgenda equals estado.IdEstadoAgendamiento
								join sede in _context.MaSedes on curso.IdSede equals sede.IdSede
								join empresa in _context.MaEmpresas on sede.IdEmpresa equals empresa.IdEmpresa
								join docente in _context.TrDocentes on curso.IdDocente equals docente.IdDocente
								where curso.IdCurso == request.Data.IdCurso && agenda.IdEstadoAgenda == request.Data.IdEstadoAgenda
								select new { curso, agenda, sede, docente, estado, tipoDoc }
								)
								.Select(x => new GetListadoCurso
								{
									IdCurso = x.curso.IdCurso,
									NombreCurso = x.curso.NombreCurso,
									FechaCurso = x.curso.FechaCurso,
									HoraInicioCurso = x.curso.HoraInicioCurso,
									HoraFinCurso = x.curso.HoraFinCurso,
									IdSede = x.curso.IdSede,
									Sede = x.sede.Sede,
									Direccion = x.sede.Direccion,
									IdDocente = x.curso.IdDocente,
									Docente = x.docente.NombreDocente,
									CapacidadCurso = x.curso.CapacidadCurso,
									IdFupa = x.agenda.IdFupa,
									IdTipoDocumento = x.agenda.IdTipoDocumento,
									NumeroDocumento = x.agenda.NumeroDocumento,
									NombrePersona = x.agenda.NombrePersona,
									NumeroComparendo = x.agenda.NumeroComparendo,
									Celular = x.agenda.Celular,
									CorreoElectronico = x.agenda.CorreoElectronico,
									IdAgendaCurso = x.agenda.IdAgendaCurso,
									IdEstadoAgenda = x.agenda.IdEstadoAgenda,
									Estado = x.estado.EstadoAgendamiento,
									TipoDocumento = x.tipoDoc.TipoDocumento,
									ValorCurso = x.agenda.ValorCurso
								}).ToListAsync();

			return result;
		}
		catch
		{
			throw;
		}
	}

	#region Métodos Privados

	private async Task<bool> ValidarDocenteCurso(Curso curso)
	{
		return await _context.TrCursos.AnyAsync(x =>
			x.IdDocente == curso.IdDocente &&
			x.FechaCurso == curso.FechaCurso &&
			x.HoraInicioCurso == curso.HoraInicioCurso &&
			x.HoraFinCurso == curso.HoraFinCurso &&
			x.Estado == "S");
	}

	private bool ValidarHorarioDocente(Curso curso)
	{
		var fechaHoy = DateTime.Now.Date;
		var horaInicioNuevoCurso = curso.HoraInicioCurso;
		var horaFinNuevoCurso = curso.HoraFinCurso;

		var cursos = _context.TrCursos
			.Where(x => x.IdDocente == curso.IdDocente && x.Estado == "S")
			.AsEnumerable() // Mueve la evaluación al cliente
			.Where(x => DateTime.Parse(x.FechaCurso) == DateTime.Parse(curso.FechaCurso) &&
						x.HoraInicioCurso.CompareTo(horaFinNuevoCurso) < 0 &&
						x.HoraFinCurso.CompareTo(horaInicioNuevoCurso) > 0)
			.ToList();

		return !cursos.Any();
	}

	private async Task<int> ValidarUsuarioCurso(int IdCurso)
	{
		var curso = await _context.TrAgendaCursos.Where(x => x.IdCurso == IdCurso).ToListAsync();

		return curso.Count > 0 ? 1 : 0;
	}

	private async Task<TrCurso> ObtenerCurso(int IdCurso)
	{
		return await _context.TrCursos.FirstOrDefaultAsync(x => x.IdCurso == IdCurso & x.Estado.Equals("S"));
	}

	private async Task<int?> ObtenerEmpresa(int? id)
	{
		var result = await _context.TrUsuarios.FirstOrDefaultAsync(x => x.IdUsuario == id & x.IdEstado == 1);

		if (result is null)
			return 0;

		return result.IdEmpresa;
	}

	private async Task<int?> ObtenerSede(int? IdUsuario)
	{
		var sede = await _context.TrUsuarios.FirstOrDefaultAsync(x => x.IdUsuario == IdUsuario);

		return sede != null ? sede.IdSede : 0;
	}

	private async Task<DateTime?> ValidarFechaCurso(DateTime date, int? idTipoComparendo)
	{
		DateTime inicio = new DateTime(date.Year, date.Month, date.Day);

		List<DateTime> diasHabiles = await ObtenerDiasHabiles(inicio, idTipoComparendo);

		return diasHabiles.LastOrDefault();
	}

	private async Task<List<DateTime>> ObtenerDiasHabiles(DateTime inicio, int? idTipoComparendo)
	{
		List<DateTime> diasHabiles = new List<DateTime>();
		int maxDias = idTipoComparendo == 1 ? 20 : (idTipoComparendo == 2 ? 26 : 0);
		int diasProcesados = 0;

		for (DateTime fecha = inicio; diasHabiles.Count < maxDias; fecha = fecha.AddDays(1))
		{
			if (await EsDiaHabil(fecha))
				diasHabiles.Add(fecha);

			diasProcesados++;

			if (diasProcesados >= maxDias * 2)
				break;
		}

		return diasHabiles;
	}

	private async Task<bool> EsDiaHabil(DateTime fecha)
	{
		var festivos = await _context.MaDiaFestivos.Where(x => x.Fecha == DateOnly.FromDateTime(fecha) && x.Estado.Equals("S")).FirstOrDefaultAsync();
		return fecha.DayOfWeek >= DayOfWeek.Monday && fecha.DayOfWeek <= DayOfWeek.Friday && festivos is null;
	}

	private decimal? CalcularPorcentaje(string Fecha, DateTime FechaComparendo, int? IdtipoComparendo)
	{
		var porcetanje = _context.MaTiemposDescuentos.Where(x => x.IdTipoComparendo == IdtipoComparendo).ToList();
		var dias = (DateTime.Parse(Fecha).Date - FechaComparendo.Date).Days;

		if (FechaComparendo.Date == DateTime.Now.Date)
			dias = 1;

		switch (IdtipoComparendo)
		{
			case 1:
				if (dias >= 1 && dias <= 5)
					return porcetanje.Where(x => x.IdTiemposDescuento == 1).Select(x => x.ValorDescuento).FirstOrDefault();
				if (dias >= 6 && dias <= 20)
					return porcetanje.Where(x => x.IdTiemposDescuento == 2).Select(x => x.ValorDescuento).FirstOrDefault();
				break;

			case 2:
				if (dias >= 1 && dias <= 11)
					return porcetanje.Where(x => x.IdTiemposDescuento == 3).Select(x => x.ValorDescuento).FirstOrDefault();
				if (dias >= 12 && dias <= 26)
					return porcetanje.Where(x => x.IdTiemposDescuento == 4).Select(x => x.ValorDescuento).FirstOrDefault();
				break;

			default:
				break;
		}

		return 0;
	}

	private async Task<decimal> ObtenerValorCurso()
	{
		var valor = await _context.MaValorCursos.FirstOrDefaultAsync(x => x.Anno == DateTime.Now.Year && x.Estado.Equals("S"));
		return (decimal)(valor != null ? valor.Valor : 0);
	}

	#endregion Métodos Privados
}