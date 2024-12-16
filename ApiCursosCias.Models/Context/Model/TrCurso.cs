using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Tr_curso")]
public partial class TrCurso
{
    [Key]
    public int IdCurso { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string NombreCurso { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string FechaCurso { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string HoraInicioCurso { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string HoraFinCurso { get; set; }

    public int? IdSede { get; set; }

    public int? IdDocente { get; set; }

    public int? CapacidadCurso { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaRegistro { get; set; }

    public int? IdUsuarioRegistro { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string Estado { get; set; }

    public int? Ocupado { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string Duracion { get; set; }
}
