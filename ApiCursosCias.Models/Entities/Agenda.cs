using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCursosCias.Models.Entities;

public class Agenda
{
	public int IdAgendaCurso { get; set; }

	public int? IdCurso { get; set; }

	public int? IdTipoDocumento { get; set; }

	public decimal? NumeroDocumento { get; set; }

	public string NombrePersona { get; set; }

	public string Celular { get; set; }

	public string CorreoElectronico { get; set; }

	public string NumeroComparendo { get; set; }

	public DateTime? FechaRegistro { get; set; }

	public int? IdUsuario { get; set; }

	public decimal? IdFupa { get; set; }

	public int? IdEstadoAgenda { get; set; }
	public decimal? ValorCurso { get; set; }

	public decimal? ValorComparendo { get; set; }

	public decimal? PorcentajeDescuento { get; set; }
}

public class GetAgenda : Agenda
{
	public string NombreCurso { get; set; }
	public string FechaCurso { get; set; }
	public string HoraInicioCurso { get; set; }
	public string HoraFinCurso { get; set; }
	public int? IdSede { get; set; }
	public string Sede { get; set; }
	public string NombreDocente { get; set; }
	public int? IdDocente { get; set; }
	public string TipoDocumento { get; set; }
}

public class SetAgenda : Agenda
{
	public DateTime FechaComparendo { get; set; }
	public int? IdTipoComparendo { get; set; }
	public int IdCanal { get; set; }

	[NotMapped]
	public string Consulta { get; set; }
}