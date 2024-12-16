using System.Text.Json.Serialization;

namespace ApiCursosCias.Models.Entities;

public class Pago
{
	public int IdPagoAgenda { get; set; }

	public int? IdAgendaCurso { get; set; }

	public decimal? IdFupa { get; set; }

	public int? IdConsecutivoPago { get; set; }

	public DateTime? FechaPago { get; set; }

	public int? IdUsuario { get; set; }

	public decimal? ValorComparendo { get; set; }

	public decimal? PorcentajeDescuento { get; set; }

	public decimal? ValorDescuento { get; set; }

	public decimal? ValorApagar { get; set; }

	public decimal? ValorTotalPagar { get; set; }

	public int? IdSede { get; set; }

	public string CodigoInfraccion { get; set; }

	public decimal? ValorCurso { get; set; }

	public string MunicipioInfraccion { get; set; }

	public string Horaregistro { get; set; }
}

public class PagoPdf
{
	public int? IdConsecutivoPago { get; set; }

	public DateTime FechaLiquidacion { get; set; }

	public TimeSpan HoraLiquidacion { get; set; }

	public decimal? NumeroDocumento { get; set; }

	public string NombrePersona { get; set; }

	public string Celular { get; set; }

	public string CorreoElectronico { get; set; }

	public string FechaCurso { get; set; }

	public string HoraInicioCurso { get; set; }

	public string HoraFinCurso { get; set; }

	public string Sede { get; set; }

	public string Direccion { get; set; }

	public string Municipio { get; set; }

	public string NumeroComparendo { get; set; }

	public string CodigoInfraccion { get; set; }

	public decimal? ValorComparendo { get; set; }

	public decimal? ValorCurso { get; set; }

	public decimal? PorcentajeDescuento { get; set; }

	public decimal? ValorDescuento { get; set; }

	public decimal? ValorAPagar { get; set; }

	public decimal? ValorTotalPagar { get; set; }
	public string Nit { get; set; }
	public string Regimen { get; set; }
	public string NombreEmpresa { get; set; }
	public string NombreCurso { get; set; }
	public string NombreDocente { get; set; }
	public string NombreUsuario { get; set; }
	public string MunicipioInfraccion { get; set; }
	public DateTime? FechaPago { get; set; }

	public Notificacion Notificacion { get; set; }
}