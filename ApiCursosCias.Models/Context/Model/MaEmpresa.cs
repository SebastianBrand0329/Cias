using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Ma_Empresa")]
public partial class MaEmpresa
{
    [Key]
    public int IdEmpresa { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Empresa { get; set; }

    public int IdMunicipio { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string Estado { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Nit { get; set; }

    [ForeignKey("IdMunicipio")]
    [InverseProperty("MaEmpresas")]
    public virtual MaMunicipio IdMunicipioNavigation { get; set; }
}
