using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Ma_Funcionalidad")]
public partial class MaFuncionalidad
{
    [Key]
    public int IdFuncionalidad { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Funcionalidad { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string Estado { get; set; }

    public int? IdModulo { get; set; }
}
