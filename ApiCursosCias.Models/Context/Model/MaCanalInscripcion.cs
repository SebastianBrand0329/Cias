using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Ma_CanalInscripcion")]
public partial class MaCanalInscripcion
{
    [Key]
    public int IdCanalInscripcion { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string CanalInscripcion { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string Estado { get; set; }
}
