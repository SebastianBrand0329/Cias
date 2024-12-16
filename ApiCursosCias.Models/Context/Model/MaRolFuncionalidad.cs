using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Keyless]
[Table("Ma_RolFuncionalidad")]
public partial class MaRolFuncionalidad
{
    public int IdRolFuncionalidad { get; set; }

    public int? IdRol { get; set; }

    public int? IdFuncionalidad { get; set; }

    [ForeignKey("IdFuncionalidad")]
    public virtual MaFuncionalidad IdFuncionalidadNavigation { get; set; }

    [ForeignKey("IdRol")]
    public virtual MaRol IdRolNavigation { get; set; }
}
