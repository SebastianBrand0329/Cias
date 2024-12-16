using ApiCursosCias.Models.Context.Interfaces;
using ApiCursosCias.Models.Context.Model;
using ApiCursosCias.Models.Entities;
using Azure.Core;
using Microsoft.EntityFrameworkCore.Storage;

namespace ApiCursosCias.Models.Context.Access;

public class TransactionDao : ITransactionDao
{
	private DataContext _context;
	private IDbContextTransaction _dataTransaction;

	private readonly IUsuarioDao _usuario;
	private readonly IMaestroDao _maestro;
	private readonly ICursoDao _curso;
	private readonly IAgendaDao _agenda;
	private readonly IHelpers _helpers;
	private readonly IPagoDao _pago;
	private IDescargaDao _descarga;

	private readonly DaoFactory _factory;

	public TransactionDao()
	{
		_factory = DaoFactory.GetDaoFactory(AccessDaoFactory.ConnectionString);
		_usuario = _factory.GetUsuarioDao();
		_maestro = _factory.GetMaestroDao();
		_curso = _factory.GetCursoDao();
		_agenda = _factory.GetAgendaDao();
		_helpers = _factory.GetHelpersDao();
		_pago = _factory.GetPagoDao();
		_descarga = _factory.GetDescargaDao();
	}

	public async Task<UsuarioExtend> AutenticarUsuario(Request<Usuario> request)
	{
		try
		{
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_usuario.SetTransaction(_context);

						var result = await _usuario.AutenticarUsuario(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
		}
		catch
		{
			throw;
		}
	}

	public async Task<List<UsuarioLoginExtended>> GetUsuarioLogin(UsuarioLogin request)
	{
		try
		{
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_usuario.SetTransaction(_context);

						var result = await _usuario.GetUsuarioLogin(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_usuario.SetTransaction(_context);

						var result = await _usuario.SetUsuarioLogin(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_usuario.SetTransaction(_context);

						var result = await _usuario.ActulizarClave(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
		}
		catch
		{
			throw;
		}
	}

	public async Task<List<Maestro>> GetMaestroRoles()
	{
		try
		{
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_maestro.SetTransaction(_context);

						var result = await _maestro.GetMaestroRoles();
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_maestro.SetTransaction(_context);

						var result = await _maestro.GetMaestroSedes(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_usuario.SetTransaction(_context);

						var result = await _usuario.RegistrarUsuario(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_usuario.SetTransaction(_context);

						var result = await _usuario.ActualizarUsuario(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_usuario.SetTransaction(_context);

						var result = await _usuario.ObtenerUsuariosAgendadores(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
		}
		catch
		{
			throw;
		}
	}

	public async Task<Curso> RegistrarCurso(Request<Curso> request)
	{
		try
		{
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_curso.SetTransaction(_context);

						var result = await _curso.RegistrarCurso(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
		}
		catch
		{
			throw;
		}
	}

	public async Task<Curso> ActualizarCurso(Request<Curso> request)
	{
		try
		{
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_curso.SetTransaction(_context);

						var result = await _curso.ActualizarCurso(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_curso.SetTransaction(_context);

						var result = await _curso.ObtenerCursos(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_maestro.SetTransaction(_context);

						var result = await _maestro.GetMaestroTipoDocumento();
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
		}
		catch
		{
			throw;
		}
	}

	public async Task<List<GetAgenda>> ObtenerAgendaSede(Request<Usuario> request)
	{
		try
		{
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_agenda.SetTransaction(_context);

						var result = await _agenda.ObtenerAgendaSede(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
		}
		catch
		{
			throw;
		}
	}

	public async Task<string> RegistrarAgenda(Request<SetAgenda> request)
	{
		try
		{
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_agenda.SetTransaction(_context);

						var result = await _agenda.RegistrarAgenda(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_curso.SetTransaction(_context);

						var result = await _curso.ObtenerCursosDia(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_curso.SetTransaction(_context);

						var result = await _curso.ObtenerAgendamientoCursos(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_curso.SetTransaction(_context);

						var result = await _curso.ObtenerCursosUsuario(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_curso.SetTransaction(_context);

						var result = await _curso.ObtenerCursosFecha(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_maestro.SetTransaction(_context);

						var result = await _maestro.GetMaestroDescuentos();
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
		}
		catch
		{
			throw;
		}
	}

	public async Task<List<Maestro>> GetMaestroDocentes(Request<Usuario> request)
	{
		try
		{
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_maestro.SetTransaction(_context);

						var result = await _maestro.GetMaestroDocentes(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
		}
		catch
		{
			throw;
		}
	}

	public async Task<bool> RegistrarConsulta(SetConsulta request)
	{
		try
		{
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_agenda.SetTransaction(_context);

						var result = await _agenda.RegistrarConsulta(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
		}
		catch
		{
			throw;
		}
	}

	public async Task<InformacionSimit> GetInformacionSimit(InformacionSimit request)
	{
		try
		{
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_helpers.SetTransaction(_context);

						var result = await _helpers.GetInformacionSimit(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
		}
		catch
		{
			throw;
		}
	}

	public async Task<PagoPdf> RegistrarPago(Request<Pago> request)
	{
		try
		{
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_pago.SetTransaction(_context);

						var result = await _pago.RegistrarPago(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_curso.SetTransaction(_context);

						var result = await _curso.ObtenerCursosIdentificacion(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_agenda.SetTransaction(_context);

						var result = await _agenda.RegistrarIngresoAgenda(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_agenda.SetTransaction(_context);

						var result = await _agenda.RegistrarSalidaAgenda(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_pago.SetTransaction(_context);

						var result = await _pago.ObtenerPago(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
		}
		catch
		{
			throw;
		}
	}

	public async Task<PagoPdf> DescargarPagoCurso(Request<SetAgenda> request)
	{
		try
		{
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_descarga.SetTransaction(_context);

						var result = await _descarga.DescargarPagoCurso(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_descarga.SetTransaction(_context);

						var result = await _descarga.DescargarPagoInformativoCurso(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_descarga.SetTransaction(_context);

						var result = await _descarga.DescargarCertificadoCurso(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_curso.SetTransaction(_context);

						var result = await _curso.ObtenerCursosEstado(request);
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
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
			using (_context = new DataContext(AccessDaoFactory.OptionsBuilder))
			{
				using (_dataTransaction = _context.Database.BeginTransaction())
				{
					try
					{
						//CARGAR TRANSACCION EN OBJETOS DE LA CLASE
						_agenda.SetTransaction(_context);

						var result = await _agenda.LiberarFupas();
						await _dataTransaction.CommitAsync();
						return result;
					}
					catch
					{
						await _dataTransaction.RollbackAsync();
						throw;
					}
				}
			}
		}
		catch
		{
			throw;
		}
	}
}