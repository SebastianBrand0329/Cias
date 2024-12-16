using ApiCursosCias.Models.Context.Access;
using ApiCursosCias.Models.Context.Interfaces;
using ApiCursosCias.Models.Context.Model;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context;

public class AccessDaoFactory : DaoFactory
{
    #region Static Access Members

    public static string ConnectionString { get; set; }

    public static DbContextOptions<DataContext> OptionsBuilder
    {
        get
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(ConnectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .Options;
            return options;
        }
    }

    #endregion Static Access Members

    #region Transactional Interfaces

    public override ITransactionDao GetTransactionDao()
    {
        return new TransactionDao();
    }

    public override IUsuarioDao GetUsuarioDao()
    {
        return new UsuarioDao();
    }

    public override IMaestroDao GetMaestroDao()
    {
        return new MaestroDao();
    }

    public override ICursoDao GetCursoDao()
    {
        return new CursoDao();
    }

    public override IAgendaDao GetAgendaDao()
    {
        return new AgendaDao();
    }

    public override IHelpers GetHelpersDao()
    {
        return new HelpersDao();
    }

    public override IPagoDao GetPagoDao()
    {
        return new PagoDao();
    }

    public override IDescargaDao GetDescargaDao()
    {
        return new DescargaDao();
    }

    #endregion Transactional Interfaces
}