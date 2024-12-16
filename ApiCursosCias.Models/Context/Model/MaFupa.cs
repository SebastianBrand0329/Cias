using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Ma_Fupa")]
public partial class MaFupa
{
    [Key]
    [Column(TypeName = "numeric(18, 0)")]
    public decimal IdFupa { get; set; }

    public int? IdEmpresa { get; set; }

    public int? IdestadoFupa { get; set; }
}
