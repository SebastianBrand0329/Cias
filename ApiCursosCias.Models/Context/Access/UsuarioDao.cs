using ApiCursosCias.Models.Context.Interfaces;
using ApiCursosCias.Models.Context.Model;
using ApiCursosCias.Models.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Access;

public class UsuarioDao : IUsuarioDao
{
	private DataContext _context;
	private readonly IMapper _mapper = MapperBootstrapper.Instance;

	public void SetTransaction(DataContext dataContext)
	{
		_context = dataContext;
	}

	public async Task<UsuarioExtend> AutenticarUsuario(Request<Usuario> request)
	{
		try
		{
			var userValid = await _context.TrUsuarios.FirstOrDefaultAsync(x => x.Usuario == request.Data.UserName &&
																			   x.Clave.Equals(request.Data.Clave)) ?? throw new ResultException(0, "usuario o contraseña no coinciden");

			if (userValid.IdEstado.Equals(0))
				throw new ResultException(0, "Usuario Inactivo");

			var result = (from usuario in _context.TrUsuarios
						  join rol in _context.MaRols on usuario.IdRol equals rol.IdRol
						  join rolFuncionalidad in _context.MaRolFuncionalidads on rol.IdRol equals rolFuncionalidad.IdRol
						  join funcionalidad in _context.MaFuncionalidads on rolFuncionalidad.IdFuncionalidad equals funcionalidad.IdFuncionalidad
						  join empresa in _context.MaEmpresas on usuario.IdEmpresa equals empresa.IdEmpresa
						  join modulo in _context.MaModulos on funcionalidad.IdModulo equals modulo.IdModulo
						  where usuario.Usuario.Equals(request.Data.UserName) & modulo.Estado.Equals("S")
						  select new { usuario, rol, funcionalidad, empresa, modulo }
						  ).ToList()
						   .GroupBy(s => new
						   {
							   s.usuario.IdUsuario,
							   s.usuario.Usuario,
							   s.usuario.Clave,
							   s.usuario.NombreUsuario,
							   s.rol.IdRol,
							   s.usuario.IdEmpresa,
							   s.empresa.Empresa,
							   s.usuario.ClaveActualizada,
							   s.usuario.IdEstado,
						   })
						   .Select(g => new UsuarioExtend
						   {
							   IdUsuario = g.Key.IdUsuario,
							   UserName = g.Key.Usuario,
							   Clave = g.Key.Clave,
							   NombreUsuario = g.Key.NombreUsuario,
							   IdRol = g.Key.IdRol,
							   IdEmpresa = g.Key.IdEmpresa,
							   Empresa = g.Key.Empresa,
							   ClaveActualizada = g.Key.ClaveActualizada,
							   IdEstado = g.Key.IdEstado,
							   Funcionalidades = g.Select(x => new Funcionalidad
							   {
								   IdFuncionalidad = x.funcionalidad.IdFuncionalidad,
								   NombreFuncionalidad = x.funcionalidad.Funcionalidad,
								   IdModulo = x.modulo.IdModulo,
								   Modulo = x.modulo.Modulo,
							   }).ToList()
						   }).FirstOrDefault();

			return result;
		}
		catch (Exception)
		{
			throw;
		}
	}

	public async Task<List<UsuarioLoginExtended>> GetUsuarioLogin(UsuarioLogin request)
	{
		try
		{
			List<TrUsuarioLogin> result = default;
			if (request.CodeLogin.HasValue)
				result = await _context.TrUsuarioLogins.Where(x => x.CodeLogin.Equals(request.CodeLogin)).ToListAsync();
			else if (!string.IsNullOrEmpty(request.Token))
				result = await _context.TrUsuarioLogins.Where(x => x.Token.Equals(request.Token)).ToListAsync();

			return result != null && result.Count > 0 ? _mapper.Map<List<UsuarioLoginExtended>>(result) : null;
		}
		catch
		{
			throw;
		}
	}

	public async Task<bool> SetUsuarioLogin(List<UsuarioLogin> request)
	{
		try
		{
			var logins = _mapper.Map<List<TrUsuarioLogin>>(request);

			foreach (var login in logins)
			{
				var exist = _context.TrUsuarioLogins.Any(x => x.CodeLogin.Equals(login.CodeLogin) && x.Token.Equals(login.Token));
				if (exist)
					_context.TrUsuarioLogins.Update(login);
				else
					await _context.TrUsuarioLogins.AddAsync(login);
				await _context.SaveChangesAsync();
			}

			return true;
		}
		catch
		{
			throw;
		}
	}

	public async Task<bool> ActulizarClave(Request<Usuario> request)
	{
		try
		{
			var user = await _context.TrUsuarios.FirstOrDefaultAsync(x => x.Usuario.Equals(request.Data.UserName) && x.IdEstado != 0);

			if (!user.Clave.Equals(request.Data.ClaveActual))
				throw new ResultException(0, "La clave actual no coincide");

			if (user == null)
				throw new ResultException(0, "El usuario no existe");

			if (request.Data.Clave.Equals(request.Data.ClaveActual))
				throw new ResultException(0, "La nueva contraseña debe ser diferente a la contraseña actual.");

			user.Clave = request.Data.Clave;
			user.ClaveActualizada = true;
			request.Data.ClaveActualizada = user.ClaveActualizada;
			_context.TrUsuarios.Update(user);
			await _context.SaveChangesAsync();
			return true;
		}
		catch
		{
			throw;
		}
	}

	public async Task<Usuario> RegistrarUsuario(Request<Usuario> request)
	{
		try
		{
			if (!await ValidarDocumento(request.Data.Identificacion))
				throw new ResultException(0, "Error, ya existe un usuario con ese número de documento");
			else if (await ValidarRol(request.Data.IdRol))
				throw new ResultException(0, "Error, El rol enviado no existe");

			request.Data.IdEmpresa = await ObtenerEmpresa(request.Data.IdUsuarioRegistra);
			var user = _mapper.Map<TrUsuario>(request.Data);
			await _context.TrUsuarios.AddAsync(user);
			await _context.SaveChangesAsync();

			request.Data.Notificacion = await ObtenerPlantilla();

			return request.Data;
		}
		catch
		{
			throw;
		}
	}

	public async Task<Usuario> ActualizarUsuario(Request<Usuario> request)
	{
		try
		{
			switch (request.Data.IdEstado)
			{
				case 0:
					var user = await ObtenerUsuario(request.Data.UserName);
					if (user is null)
						throw new ResultException(0, "Error, el usuario no existe");
					user.IdEstado = request.Data.IdEstado;

					_context.TrUsuarios.Update(user);
					await _context.SaveChangesAsync();
					break;

				case 1:
					var userUpdate = await ObtenerUsuario(request.Data.UserName);

					if (userUpdate is null)
						throw new ResultException(0, "Error, el usuario no existe");
					else if (await ValidarRol(request.Data.IdRol))
						throw new ResultException(0, "Error, El rol enviado no existe");

					userUpdate.NombreUsuario = request.Data.NombreUsuario;
					userUpdate.IdTipoDocumento = request.Data.IdTipoDocumento;
					userUpdate.Correo = request.Data.Correo;
					userUpdate.Celular = request.Data.Celular;
					userUpdate.IdEmpresa = request.Data.IdEmpresa;
					userUpdate.IdSede = request.Data.IdSede;
					userUpdate.IdRol = request.Data.IdRol;
					userUpdate.FechaIngreso = request.Data.FechaIngreso;

					_context.TrUsuarios.Update(userUpdate);
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

	public async Task<List<UsuarioAgendador>> ObtenerUsuariosAgendadores(Request<Usuario> request)
	{
		try
		{
			var IdEmpresa = await ObtenerEmpresa(request.Data.IdUsuario);

			var result = (from usuario in _context.TrUsuarios
						  join tipoDocumento in _context.MaTipoDocumentos on usuario.IdTipoDocumento equals tipoDocumento.IdTipoDocumento
						  join empresa in _context.MaEmpresas on usuario.IdEmpresa equals empresa.IdEmpresa
						  join sede in _context.MaSedes on usuario.IdSede equals sede.IdSede
						  join rol in _context.MaRols on usuario.IdRol equals rol.IdRol
						  where usuario.IdRol == 2 & usuario.IdEstado == 1 & usuario.IdEstado == IdEmpresa
						  select new { usuario, tipoDocumento, empresa, sede, rol }
						  ).ToList()
						  .Select(x => new UsuarioAgendador
						  {
							  UserName = x.usuario.Usuario,
							  NombreUsuario = x.usuario.NombreUsuario,
							  IdTipoDocumento = x.usuario.IdTipoDocumento,
							  TipoDocumento = x.tipoDocumento.TipoDocumento,
							  Identificacion = x.usuario.Identificacion,
							  Correo = x.usuario.Correo,
							  Celular = x.usuario.Celular,
							  IdEmpresa = x.usuario.IdEmpresa,
							  Empresa = x.empresa.Empresa,
							  IdSede = x.usuario.IdSede,
							  Sede = x.sede.Sede,
							  IdRol = x.usuario.IdRol,
							  Rol = x.rol.Rol,
							  IdEstado = x.usuario.IdEstado,
						  }).ToList();

			return result;
		}
		catch
		{
			throw;
		}
	}

	#region Métodos Privados

	private async Task<bool> ValidarDocumento(decimal? identificacion)
	{
		var result = await _context.TrUsuarios.SingleOrDefaultAsync(x => x.Identificacion == identificacion & x.IdEstado == 1);

		if (result is null)
			return true;

		return false;
	}

	private async Task<bool> ValidarRol(int? number)
	{
		var result = await _context.MaRols.SingleOrDefaultAsync(x => x.IdRol == number);

		if (result is null)
			return true;

		return false;
	}

	private async Task<int?> ObtenerEmpresa(int? id)
	{
		var result = await _context.TrUsuarios.FirstOrDefaultAsync(x => x.IdUsuario == id & x.IdEstado == 1);

		if (result is null)
			return 0;

		return result.IdEmpresa;
	}

	private async Task<TrUsuario> ObtenerUsuario(decimal? user)
	{
		return await _context.TrUsuarios.FirstOrDefaultAsync(x => x.Usuario == user & x.IdEstado == 1);
	}

	private async Task<Notificacion> ObtenerPlantilla()
	{
		var plantilla = await _context.MaPlantillas.FirstOrDefaultAsync(X => X.IdPlantilla == 1);
		return _mapper.Map<Notificacion>(plantilla);
	}

	#endregion Métodos Privados
}