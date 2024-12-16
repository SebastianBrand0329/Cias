using ApiCursosCias.Models.Context.Model;
using ApiCursosCias.Models.Entities;

namespace ApiCursosCias.Models.Context.Interfaces;

public interface ICursoDao
{
	void SetTransaction(DataContext dataContext);

	Task<Curso> ActualizarCurso(Request<Curso> request);

	Task<Curso> RegistrarCurso(Request<Curso> request);

	Task<List<GetCurso>> ObtenerCursos(Request<Curso> request);

	Task<List<GetCurso>> ObtenerCursosDia(Request<Curso> request);

	Task<List<GetListadoCurso>> ObtenerAgendamientoCursos(Request<Curso> request);

	Task<List<GetListadoCurso>> ObtenerCursosUsuario(Request<SetAgenda> request);

	Task<List<GetCurso>> ObtenerCursosFecha(Request<SetAgenda> request);

	Task<List<GetListadoCurso>> ObtenerCursosIdentificacion(Request<SetAgenda> request);

	Task<List<GetListadoCurso>> ObtenerCursosEstado(Request<SetAgenda> request);
}