using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Keyless]
[Table("Ma_TiemposDescuento")]
public partial class MaTiemposDescuento
{
    public int IdTiemposDescuento { get; set; }

    public int? IdTipoComparendo { get; set; }

    public int? DiasInicial { get; set; }

    public int? DiasFinal { get; set; }

    [Column(TypeName = "decimal(4, 2)")]
    public decimal? ValorDescuento { get; set; }
}
