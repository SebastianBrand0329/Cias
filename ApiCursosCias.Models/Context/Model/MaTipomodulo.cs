using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Keyless]
[Table("Ma_Tipomodulo")]
public partial class MaTipomodulo
{
    public int IdTipomodulo { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Tipomodulo { get; set; }
}
