using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Ma_EstadoFupa")]
public partial class MaEstadoFupa
{
    [Key]
    public int IdEstadoFupa { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string EstadoFupa { get; set; }
}
