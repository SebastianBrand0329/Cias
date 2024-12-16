using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Ma_Departamento")]
public partial class MaDepartamento
{
    [Key]
    [StringLength(100)]
    [Unicode(false)]
    public string CodigoDepartamento { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Departamento { get; set; }

    public int? IdPais { get; set; }

    [InverseProperty("CodigoDepartamentoNavigation")]
    public virtual ICollection<MaMunicipio> MaMunicipios { get; set; } = new List<MaMunicipio>();
}
