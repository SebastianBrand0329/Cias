using ApiCursosCias.Models.Context.Model;
using ApiCursosCias.Models.Entities;

namespace ApiCursosCias.Models.Context.Interfaces;

public interface IDescargaDao
{
    void SetTransaction(DataContext dataContext);

    Task<PagoPdf> DescargarPagoCurso(Request<SetAgenda> request);

    Task<PagoPdf> DescargarPagoInformativoCurso(Request<SetAgenda> request);

    Task<Certificado> DescargarCertificadoCurso(Request<SetAgenda> request);
}