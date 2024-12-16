using ApiCursosCias.Models.Context.Model;
using ApiCursosCias.Models.Entities;

namespace ApiCursosCias.Models.Context.Interfaces;

public interface IAgendaDao
{
	void SetTransaction(DataContext dataContext);

	Task<List<GetAgenda>> ObtenerAgendaSede(Request<Usuario> request);

	Task<string> RegistrarAgenda(Request<SetAgenda> request);

	Task<bool> RegistrarConsulta(SetConsulta request);

	Task<string> RegistrarIngresoAgenda(Request<SetAgenda> request);

	Task<Certificado> RegistrarSalidaAgenda(Request<SetAgenda> request);

	Task<bool> LiberarFupas();
}