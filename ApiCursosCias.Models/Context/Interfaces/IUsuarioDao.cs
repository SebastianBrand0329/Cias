using ApiCursosCias.Models.Context.Model;
using ApiCursosCias.Models.Entities;

namespace ApiCursosCias.Models.Context.Interfaces;

public interface IUsuarioDao
{
    void SetTransaction(DataContext dataContext);

    Task<UsuarioExtend> AutenticarUsuario(Request<Usuario> request);

    Task<List<UsuarioLoginExtended>> GetUsuarioLogin(UsuarioLogin request);

    Task<bool> SetUsuarioLogin(List<UsuarioLogin> request);

    Task<bool> ActulizarClave(Request<Usuario> request);

    Task<Usuario> RegistrarUsuario(Request<Usuario> request);

    Task<Usuario> ActualizarUsuario(Request<Usuario> request);

    Task<List<UsuarioAgendador>> ObtenerUsuariosAgendadores(Request<Usuario> request);
}