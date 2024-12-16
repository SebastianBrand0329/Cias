using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Ma_Plantilla")]
public partial class MaPlantilla
{
    [Key]
    public int IdPlantilla { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string Descripcion { get; set; }

    [Column("plantilla")]
    [Unicode(false)]
    public string Plantilla { get; set; }

    public int? IdTipoPlantilla { get; set; }
}
