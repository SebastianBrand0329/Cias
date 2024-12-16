using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Ma_Municipio")]
public partial class MaMunicipio
{
    [Key]
    public int IdMunicipio { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string CodigoMunicipio { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Municipio { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string CodigoDepartamento { get; set; }

    [ForeignKey("CodigoDepartamento")]
    [InverseProperty("MaMunicipios")]
    public virtual MaDepartamento CodigoDepartamentoNavigation { get; set; }

    [InverseProperty("IdMunicipioNavigation")]
    public virtual ICollection<MaEmpresa> MaEmpresas { get; set; } = new List<MaEmpresa>();
}
