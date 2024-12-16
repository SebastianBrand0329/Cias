using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Ma_ValorCurso")]
public partial class MaValorCurso
{
    [Key]
    public int IdValorCurso { get; set; }

    [Column(TypeName = "numeric(10, 2)")]
    public decimal? Valor { get; set; }

    public int? Anno { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string Descripcion { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string Estado { get; set; }
}
