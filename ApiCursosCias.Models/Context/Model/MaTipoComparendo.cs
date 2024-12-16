using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Ma_TipoComparendo")]
public partial class MaTipoComparendo
{
    [Key]
    public int IdTipoComparendo { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string TipoComparendo { get; set; }
}
