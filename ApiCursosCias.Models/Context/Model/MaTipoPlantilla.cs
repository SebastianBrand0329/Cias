using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Ma_tipoPlantilla")]
public partial class MaTipoPlantilla
{
    [Key]
    public int IdTipoPlantilla { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string TipoPlantilla { get; set; }
}
