using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Ma_TipoFiltro")]
public partial class MaTipoFiltro
{
    [Key]
    public int IdTipoFiltro { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string TipoFiltro { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string Estado { get; set; }
}
