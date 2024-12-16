using ApiCursosCias.Models.Context.Model;
using ApiCursosCias.Models.Entities;

namespace ApiCursosCias.Models.Context.Interfaces;

public interface IPagoDao
{
    void SetTransaction(DataContext context);

    Task<PagoPdf> RegistrarPago(Request<Pago> request);

    Task<PagoPdf> ObtenerPago(Request<SetAgenda> request);
}