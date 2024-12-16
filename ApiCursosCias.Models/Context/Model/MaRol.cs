using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Ma_Rol")]
public partial class MaRol
{
    [Key]
    public int IdRol { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Rol { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string Estado { get; set; }
}
