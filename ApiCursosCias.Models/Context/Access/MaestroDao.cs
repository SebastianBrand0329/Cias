using ApiCursosCias.Models.Context.Interfaces;
using ApiCursosCias.Models.Context.Model;
using ApiCursosCias.Models.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Access;

public class MaestroDao : IMaestroDao
{
	private DataContext _context;
	private readonly IMapper _mapper = MapperBootstrapper.Instance;

	public void SetTransaction(DataContext dataContext)
	{
		_context = dataContext;
	}

	public async Task<List<Maestro>> GetMaestroRoles()
	{
		try
		{
			var result = await _context.MaRols.OrderBy(x => x.IdRol)
											 .ToListAsync();
			return _mapper.Map<List<Maestro>>(result);
		}
		catch
		{
			throw;
		}
	}

	public async Task<List<Maestro>> GetMaestroSedes(Request<Usuario> request)
	{
		try
		{
			var result = await _context.MaSedes.Where(x => x.IdEmpresa == request.Data.IdEmpresa)
											   .OrderBy(x => x.IdSede)
											   .ToListAsync();
			return _mapper.Map<List<Maestro>>(result);
		}
		catch
		{
			throw;
		}
	}

	public async Task<List<Maestro>> GetMaestroTipoDocumento()
	{
		try
		{
			var result = await _context.MaTipoDocumentos.OrderBy(x => x.IdTipoDocumento)
														.ToListAsync();

			return _mapper.Map<List<Maestro>>(result);
		}
		catch
		{
			throw;
		}
	}

	public async Task<List<Maestro>> GetMaestroDescuentos()
	{
		try
		{
			var result = await _context.MaTiemposDescuentos.OrderBy(x => x.IdTipoComparendo)
														   .ToListAsync();

			return _mapper.Map<List<Maestro>>(result);
		}
		catch (Exception)
		{
			throw;
		}
	}

	public async Task<List<Maestro>> GetMaestroDocentes(Request<Usuario> request)
	{
		try
		{
			var result = await _context.TrDocentes.Where(x => x.IdSede == request.Data.IdSede & x.IdEstado == 1)
												  .ToListAsync();

			return _mapper.Map<List<Maestro>>(result);
		}
		catch
		{
			throw;
		}
	}
}