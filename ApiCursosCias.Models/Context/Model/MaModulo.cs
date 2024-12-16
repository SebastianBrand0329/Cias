using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Ma_Modulo")]
public partial class MaModulo
{
    [Key]
    public int IdModulo { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Modulo { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string Estado { get; set; }
}
