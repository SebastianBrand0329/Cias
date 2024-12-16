using ApiCursosCias.Models.Context.Model;
using ApiCursosCias.Models.Entities;

public interface IHelpers
{
    void SetTransaction(DataContext context);

    Task<InformacionSimit> GetInformacionSimit(InformacionSimit informacionSimit);
}