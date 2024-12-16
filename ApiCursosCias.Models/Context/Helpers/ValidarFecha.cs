using ApiCursosCias.Models.Context.Model;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Helpers;

public class ValidarFecha
{
    private readonly DataContext _context;

    public ValidarFecha(DataContext context)
    {
        _context = context;
    }

    public async Task<int> ValidarFechaCurso(DateTime date)
    {
        DateTime inicio = new DateTime(date.Year, date.Month, date.Day);
        DateTime fin = DateTime.Now.Date;

        List<DateTime> diasHabiles = await ObtenerDiasHabiles(inicio, fin);

        return diasHabiles.Count;
    }

    private async Task<List<DateTime>> ObtenerDiasHabiles(DateTime inicio, DateTime fin)
    {
        List<DateTime> diasHabiles = new List<DateTime>();

        for (DateTime fecha = inicio; fecha <= fin; fecha = fecha.AddDays(1))
        {
            if (await EsDiaHabil(fecha))
                diasHabiles.Add(fecha);
        }

        return diasHabiles;
    }

    private async Task<bool> EsDiaHabil(DateTime fecha)
    {
        var festivos = await _context.MaDiaFestivos.Where(x => x.Fecha == DateOnly.FromDateTime(fecha) && x.Estado.Equals("S")).FirstOrDefaultAsync();
        return fecha.DayOfWeek >= DayOfWeek.Monday && fecha.DayOfWeek <= DayOfWeek.Friday && festivos is null;
    }
}