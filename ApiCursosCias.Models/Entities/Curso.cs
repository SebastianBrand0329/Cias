namespace ApiCursosCias.Models.Entities;

public class Curso
{
	public int IdCurso { get; set; }
	public string NombreCurso { get; set; }
	public string FechaCurso { get; set; }
	public string HoraInicioCurso { get; set; }
	public string HoraFinCurso { get; set; }
	public int? IdSede { get; set; }
	public int? IdDocente { get; set; }
	public int? CapacidadCurso { get; set; }
	public DateTime? FechaRegistro { get; set; }
	public int? IdUsuario { get; set; }
	public string Estado { get; set; }
	public string Duracion { get; set; }
	public decimal? NumeroDocumento { get; set; }
	public int? IdTipoDocumento { get; set; }
}

public class GetCurso : Curso
{
	public string Sede { get; set; }
	public string Docente { get; set; }
	public string Direccion { get; set; }
	public decimal? PorcentajeDescuento { get; set; }
	public int? Ocupacion { get; set; }
	public decimal? ValorCurso { get; set; }
}

public class GetListadoCurso : GetCurso
{
	public decimal? IdFupa { get; set; }
	public string NombrePersona { get; set; }
	public string NumeroComparendo { get; set; }
	public string Celular { get; set; }
	public string CorreoElectronico { get; set; }
	public int? IdAgendaCurso { get; set; }
	public int? IdEstadoAgenda { get; set; }
	public string EstadoAgendamiento { get; set; }
	public string TipoDocumento { get; set; }
	public decimal? ValorComparendo { get; set; }
}