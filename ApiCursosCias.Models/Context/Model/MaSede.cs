using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Ma_Sede")]
public partial class MaSede
{
    [Key]
    public int IdSede { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Sede { get; set; }

    public int IdEmpresa { get; set; }

    public int IdMunicipio { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string Estado { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string Direccion { get; set; }
}
