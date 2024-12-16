using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiCursosCias.Models.Entities;

public class Maestro
{
	public int Id { get; set; }
	public string Nombre { get; set; }
	public decimal? ValorDescuento { get; set; }
	public int IdDocente { get; set; }

	public int? IdTipoDocumento { get; set; }

	public string Identificacion { get; set; }

	public string Correo { get; set; }

	public string Celular { get; set; }

	public int? IdSede { get; set; }

	public int? IdEstado { get; set; }

	public DateTime? FechaIngreso { get; set; }

	public int? IdUsuarioRegistra { get; set; }
}