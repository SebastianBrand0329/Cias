using ApiCursosCias.Models.Entities;

namespace ApiCursosCias.Models.Context.Interfaces;

public interface ITransactionDao
{
	Task<UsuarioExtend> AutenticarUsuario(Request<Usuario> request);

	Task<List<UsuarioLoginExtended>> GetUsuarioLogin(UsuarioLogin request);

	Task<bool> SetUsuarioLogin(List<UsuarioLogin> request);

	Task<bool> ActulizarClave(Request<Usuario> request);

	Task<List<Maestro>> GetMaestroRoles();

	Task<List<Maestro>> GetMaestroSedes(Request<Usuario> request);

	Task<List<Maestro>> GetMaestroTipoDocumento();

	Task<List<Maestro>> GetMaestroDescuentos();

	Task<List<Maestro>> GetMaestroDocentes(Request<Usuario> request);

	Task<Usuario> RegistrarUsuario(Request<Usuario> request);

	Task<Usuario> ActualizarUsuario(Request<Usuario> request);

	Task<List<UsuarioAgendador>> ObtenerUsuariosAgendadores(Request<Usuario> request);

	Task<Curso> RegistrarCurso(Request<Curso> request);

	Task<Curso> ActualizarCurso(Request<Curso> request);

	Task<List<GetCurso>> ObtenerCursos(Request<Curso> request);

	Task<List<GetAgenda>> ObtenerAgendaSede(Request<Usuario> request);

	Task<string> RegistrarAgenda(Request<SetAgenda> request);

	Task<List<GetCurso>> ObtenerCursosDia(Request<Curso> request);

	Task<List<GetListadoCurso>> ObtenerAgendamientoCursos(Request<Curso> request);

	Task<List<GetListadoCurso>> ObtenerCursosUsuario(Request<SetAgenda> request);

	Task<List<GetCurso>> ObtenerCursosFecha(Request<SetAgenda> request);

	Task<bool> RegistrarConsulta(SetConsulta request);

	Task<InformacionSimit> GetInformacionSimit(InformacionSimit informacionSimit);

	Task<PagoPdf> RegistrarPago(Request<Pago> request);

	Task<List<GetListadoCurso>> ObtenerCursosIdentificacion(Request<SetAgenda> request);

	Task<List<GetListadoCurso>> ObtenerCursosEstado(Request<SetAgenda> request);

	Task<string> RegistrarIngresoAgenda(Request<SetAgenda> request);

	Task<Certificado> RegistrarSalidaAgenda(Request<SetAgenda> request);

	Task<PagoPdf> ObtenerPago(Request<SetAgenda> request);

	Task<PagoPdf> DescargarPagoCurso(Request<SetAgenda> request);

	Task<PagoPdf> DescargarPagoInformativoCurso(Request<SetAgenda> request);

	Task<Certificado> DescargarCertificadoCurso(Request<SetAgenda> request);

	Task<bool> LiberarFupas();
}