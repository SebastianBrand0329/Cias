using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Ma_EstadoAgendamiento")]
public partial class MaEstadoAgendamiento
{
    [Key]
    public int IdEstadoAgendamiento { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string EstadoAgendamiento { get; set; }
}
