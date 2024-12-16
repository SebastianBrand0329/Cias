using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Ma_DiaFestivo")]
public partial class MaDiaFestivo
{
    [Key]
    public int IdDiaFestivo { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string Nombre { get; set; }

    public DateOnly? Fecha { get; set; }

    public int? Anno { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string Descripcion { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string Estado { get; set; }
}
