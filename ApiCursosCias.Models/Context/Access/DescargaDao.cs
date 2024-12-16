using ApiCursosCias.Models.Context.Interfaces;
using ApiCursosCias.Models.Context.Model;
using ApiCursosCias.Models.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Access;

public class DescargaDao : IDescargaDao
{
	private DataContext _context;
	private readonly IMapper _mapper = MapperBootstrapper.Instance;

	public void SetTransaction(DataContext dataContext)
	{
		_context = dataContext;
	}

	public async Task<PagoPdf> DescargarPagoCurso(Request<SetAgenda> request)
	{
		try
		{
			var result = await (from c in _context.TrCursos
								join a in _context.TrAgendaCursos on c.IdCurso equals a.IdCurso
								join pa in _context.TrPagoAgenda on a.IdAgendaCurso equals pa.IdAgendaCurso
								join u in _context.TrUsuarios on pa.IdUsuario equals u.IdUsuario
								join s in _context.MaSedes on c.IdSede equals s.IdSede
								join mu in _context.MaMunicipios on s.IdMunicipio equals mu.IdMunicipio
								join e in _context.MaEmpresas on s.IdEmpresa equals e.IdEmpresa
								join d in _context.TrDocentes on c.IdDocente equals d.IdDocente
								where a.IdAgendaCurso == request.Data.IdAgendaCurso
								select new PagoPdf
								{
									Nit = e.Nit,
									Regimen = "Regimen comun",
									Direccion = s.Direccion,
									Municipio = mu.Municipio,
									IdConsecutivoPago = pa.IdConsecutivoPago,
									NombreEmpresa = e.Empresa,
									NombrePersona = a.NombrePersona,
									NumeroDocumento = a.NumeroDocumento,
									CorreoElectronico = a.CorreoElectronico,
									NombreCurso = c.NombreCurso,
									FechaCurso = c.FechaCurso,
									HoraInicioCurso = c.HoraInicioCurso,
									HoraFinCurso = c.HoraFinCurso,
									NombreDocente = d.NombreDocente,
									NumeroComparendo = a.NumeroComparendo,
									ValorCurso = pa.ValorCurso,
									Sede = s.Sede,
									NombreUsuario = u.NombreUsuario
								}).FirstOrDefaultAsync();

			result.Notificacion = await ObtenerPlantillaPago();

			return result;
		}
		catch
		{
			throw;
		}
	}

	public async Task<PagoPdf> DescargarPagoInformativoCurso(Request<SetAgenda> request)
	{
		try
		{
			var result = await (from c in _context.TrCursos
								join a in _context.TrAgendaCursos on c.IdCurso equals a.IdCurso
								join pa in _context.TrPagoAgenda on a.IdAgendaCurso equals pa.IdAgendaCurso
								join ea in _context.MaEstadoAgendamientos on a.IdEstadoAgenda equals ea.IdEstadoAgendamiento
								join s in _context.MaSedes on c.IdSede equals s.IdSede
								join mu in _context.MaMunicipios on s.IdMunicipio equals mu.IdMunicipio
								join e in _context.MaEmpresas on s.IdEmpresa equals e.IdEmpresa
								join d in _context.TrDocentes on c.IdDocente equals d.IdDocente
								where a.IdAgendaCurso == request.Data.IdAgendaCurso
								select new PagoPdf
								{
									IdConsecutivoPago = pa.IdConsecutivoPago,
									FechaLiquidacion = DateTime.Now.Date,
									HoraLiquidacion = DateTime.Now.TimeOfDay,
									NumeroDocumento = a.NumeroDocumento,
									NombrePersona = a.NombrePersona,
									Celular = a.Celular,
									CorreoElectronico = a.CorreoElectronico,
									FechaCurso = c.FechaCurso,
									HoraInicioCurso = c.HoraInicioCurso,
									HoraFinCurso = c.HoraFinCurso,
									Sede = s.Sede,
									Direccion = s.Direccion,
									Municipio = mu.Municipio,
									NumeroComparendo = a.NumeroComparendo,
									CodigoInfraccion = pa.CodigoInfraccion,
									ValorComparendo = pa.ValorComparendo,
									ValorCurso = pa.ValorCurso,
									PorcentajeDescuento = pa.PorcentajeDescuento,
									ValorDescuento = pa.ValorDescuento,
									ValorAPagar = pa.ValorApagar,
									ValorTotalPagar = pa.ValorTotalPagar
								}).FirstOrDefaultAsync();

			result.Notificacion = await ObtenerPlantillaPagoInformativo();
			return result;
		}
		catch
		{
			throw;
		}
	}

	public async Task<Certificado> DescargarCertificadoCurso(Request<SetAgenda> request)
	{
		try
		{
			var result = await (from c in _context.TrCursos
								join a in _context.TrAgendaCursos on c.IdCurso equals a.IdCurso
								join td in _context.MaTipoDocumentos on a.IdTipoDocumento equals td.IdTipoDocumento
								join pa in _context.TrPagoAgenda on a.IdAgendaCurso equals pa.IdAgendaCurso
								join u in _context.TrUsuarios on pa.IdUsuario equals u.IdUsuario
								join ea in _context.MaEstadoAgendamientos on a.IdEstadoAgenda equals ea.IdEstadoAgendamiento
								join s in _context.MaSedes on c.IdSede equals s.IdSede
								join mu in _context.MaMunicipios on s.IdMunicipio equals mu.IdMunicipio
								join e in _context.MaEmpresas on s.IdEmpresa equals e.IdEmpresa
								join d in _context.TrDocentes on c.IdDocente equals d.IdDocente
								where a.IdAgendaCurso == request.Data.IdAgendaCurso
								select new Certificado
								{
									IdFupa = a.IdFupa,
									Empresa = e.Empresa,
									Nit = e.Nit,
									Sede = s.Sede,
									Direccion = s.Direccion,
									Municipio = mu.Municipio,
									NombrePersona = a.NombrePersona,
									NumeroDocumento = a.NumeroDocumento,
									PorcentajeDescuento = pa.PorcentajeDescuento,
									NumeroComparendo = a.NumeroComparendo,
									CodigoInfraccion = pa.CodigoInfraccion,
									FechaCurso = c.FechaCurso,
									HoraInicioCurso = c.HoraInicioCurso,
									Duracion = c.Duracion,
									Usuario = u.NombreUsuario,
									MunicipioInfraccion = pa.MunicipioInfraccion,
									TipoDocumento = td.TipoDocumento
								}).FirstOrDefaultAsync();

			result.Notificacion = await ObtenerPlantillaCertficiado();

			return result;
		}
		catch
		{
			throw;
		}
	}

	#region Métodos Privados

	private async Task<Notificacion> ObtenerPlantillaPago()
	{
		var plantilla = await _context.MaPlantillas.FirstOrDefaultAsync(x => x.IdPlantilla == 3);
		return _mapper.Map<Notificacion>(plantilla);
	}

	private async Task<Notificacion> ObtenerPlantillaPagoInformativo()
	{
		var plantilla = await _context.MaPlantillas.FirstOrDefaultAsync(x => x.IdPlantilla == 6);
		return _mapper.Map<Notificacion>(plantilla);
	}

	private async Task<Notificacion> ObtenerPlantillaCertficiado()
	{
		var plantilla = await _context.MaPlantillas.FirstOrDefaultAsync(x => x.IdPlantilla == 4);
		return _mapper.Map<Notificacion>(plantilla);
	}

	#endregion Métodos Privados
}