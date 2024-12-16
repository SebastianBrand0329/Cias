using ApiCursosCias.Models.Context.Interfaces;

namespace ApiCursosCias.Models.Context;

public abstract class DaoFactory
{
    public static DaoFactory GetDaoFactory(string connection)
    {
        AccessDaoFactory.ConnectionString = connection;
        return new AccessDaoFactory();
    }

    #region Transactional Interfaces

    public abstract ITransactionDao GetTransactionDao();

    public abstract IUsuarioDao GetUsuarioDao();

    public abstract IMaestroDao GetMaestroDao();

    public abstract ICursoDao GetCursoDao();

    public abstract IAgendaDao GetAgendaDao();

    public abstract IHelpers GetHelpersDao();

    public abstract IPagoDao GetPagoDao();

    public abstract IDescargaDao GetDescargaDao();

    #endregion Transactional Interfaces
}