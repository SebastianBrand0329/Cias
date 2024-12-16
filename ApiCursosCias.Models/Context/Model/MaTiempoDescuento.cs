using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Keyless]
[Table("Ma_TiempoDescuento")]
public partial class MaTiempoDescuento
{
    public int IdTiempoDescuento { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string CanalInscripcion { get; set; }

    public int? Valor { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string Estado { get; set; }
}
