using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Tr_Usuario")]
public partial class TrUsuario
{
    [Key]
    public int IdUsuario { get; set; }

    [Column(TypeName = "numeric(15, 0)")]
    public decimal? Usuario { get; set; }

    [Required]
    [StringLength(200)]
    [Unicode(false)]
    public string Clave { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string NombreUsuario { get; set; }

    public int? IdTipoDocumento { get; set; }

    [Column(TypeName = "numeric(15, 0)")]
    public decimal? Identificacion { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string Correo { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Celular { get; set; }

    public int? IdEmpresa { get; set; }

    public int? IdSede { get; set; }

    public bool? ClaveActualizada { get; set; }

    public int? IdEstado { get; set; }

    public int? IdRol { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaIngreso { get; set; }

    public int? IdUsuarioRegistra { get; set; }
}
