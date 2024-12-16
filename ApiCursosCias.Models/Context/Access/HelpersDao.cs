using ApiCursosCias.Models.Context.Model;
using ApiCursosCias.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Access;

public class HelpersDao : IHelpers
{
	private DataContext _context;

	public void SetTransaction(DataContext context)
	{
		_context = context;
	}

	public async Task<InformacionSimit> GetInformacionSimit(InformacionSimit informacionSimit)
	{
		if (informacionSimit?.multas == null)
			throw new ArgumentNullException(nameof(informacionSimit), "El objeto proporcionado no puede ser nulo.");

		var multasValidas = new InformacionSimit
		{
			multas = new List<MultasDto>()
		};

		foreach (var multa in informacionSimit.multas)
		{
			var fecha = multa.idEstadoComparendo == 2 ? multa.fechaNotificacion : multa.fechaComparendo;

			if (DateTime.TryParse(fecha, out var fechaValida))
			{
				var dias = await ValidarDiasCurso(fechaValida);

				if ((multa.idEstadoComparendo == 2 && dias >= 1 && dias <= 26) ||
					(multa.idEstadoComparendo == 1 && dias >= 1 && dias <= 20))
					multasValidas.multas.Add(multa);
			}
		}

		if (multasValidas.multas.Any())
		{
			return new InformacionSimit
			{
				multas = multasValidas.multas,
				descripcion = informacionSimit.descripcion,
				personasMismoDocumento = informacionSimit.personasMismoDocumento,
				codigo = informacionSimit.codigo
			};
		}

		throw new ResultException(0, "Actualmente no cuenta con multas que estén dentro del rango de tiempo permitido para realizar cursos");
	}

	#region Métodos Privados

	private async Task<int?> ValidarDiasCurso(DateTime date)
	{
		var inicio = date.Date;
		var fin = DateTime.Now.Date;
		var diasHabiles = new List<DateTime>();
		if ((fin - inicio).Days <= 50)
			diasHabiles = await ObtenerDiasHabiles(inicio, fin);

		return diasHabiles.Count;
	}

	private async Task<List<DateTime>> ObtenerDiasHabiles(DateTime inicio, DateTime fin)
	{
		var diasHabiles = new List<DateTime>();
		var fecha = inicio;

		var diasFestivos = await _context.MaDiaFestivos
			.Where(x => x.Fecha >= DateOnly.FromDateTime(inicio) && x.Fecha <= DateOnly.FromDateTime(fin) && x.Estado.Equals("S"))
			.ToListAsync();

		while (fecha <= fin && diasHabiles.Count <= 26)
		{
			if (EsDiaHabil(fecha, diasFestivos))
				diasHabiles.Add(fecha);

			fecha = fecha.AddDays(1);
		}

		return diasHabiles;
	}

	private bool EsDiaHabil(DateTime fecha, List<MaDiaFestivo> diasFestivos)
	{
		return fecha.DayOfWeek >= DayOfWeek.Monday && fecha.DayOfWeek <= DayOfWeek.Friday &&
			   !diasFestivos.Any(x => x.Fecha == DateOnly.FromDateTime(fecha));
	}

	#endregion Métodos Privados
}