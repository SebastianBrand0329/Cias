using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Keyless]
[Table("Tr_Docente")]
public partial class TrDocente
{
    public int IdDocente { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string NombreDocente { get; set; }

    public int? IdTipoDocumento { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Identificacion { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string Correo { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Celular { get; set; }

    public int? IdSede { get; set; }

    public int? IdEstado { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaIngreso { get; set; }

    public int? IdUsuarioRegistra { get; set; }
}
