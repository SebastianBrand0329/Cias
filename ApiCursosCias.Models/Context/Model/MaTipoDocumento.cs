using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Ma_TipoDocumento")]
public partial class MaTipoDocumento
{
    [Key]
    public int IdTipoDocumento { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string TipoDocumento { get; set; }
}
