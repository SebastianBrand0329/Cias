using ApiCursosCias.Models.Context.Model;
using ApiCursosCias.Models.Entities;

namespace ApiCursosCias.Models.Context.Interfaces;

public interface IMaestroDao
{
    void SetTransaction(DataContext dataContext);

    Task<List<Maestro>> GetMaestroRoles();

    Task<List<Maestro>> GetMaestroSedes(Request<Usuario> request);

    Task<List<Maestro>> GetMaestroTipoDocumento();

    Task<List<Maestro>> GetMaestroDescuentos();

    Task<List<Maestro>> GetMaestroDocentes(Request<Usuario> request);
}