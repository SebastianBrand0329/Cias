using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCursosCias.Models.Entities;

public class Certificado
{
	public decimal? IdFupa { get; set; }

	public string Empresa { get; set; }

	public string Nit { get; set; }
	public string Sede { get; set; }
	public string Direccion { get; set; }
	public string Municipio { get; set; }

	public string NombrePersona { get; set; }

	public decimal? NumeroDocumento { get; set; }

	public string TipoDocumento { get; set; }

	public decimal? PorcentajeDescuento { get; set; }

	public string NumeroComparendo { get; set; }
	public string CodigoInfraccion { get; set; }

	public string FechaCurso { get; set; }

	public string HoraInicioCurso { get; set; }
	public string Duracion { get; set; }

	public string Usuario { get; set; }

	public string MunicipioInfraccion { get; set; }

	public int? NumeroCertificado { get; set; }

	public string NombreDocente { get; set; }

	[NotMapped]
	public Notificacion Notificacion { get; set; }
}