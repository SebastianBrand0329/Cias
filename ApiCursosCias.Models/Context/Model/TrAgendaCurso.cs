using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Tr_AgendaCurso")]
public partial class TrAgendaCurso
{
    [Key]
    public int IdAgendaCurso { get; set; }

    public int? IdCurso { get; set; }

    public int? IdTipoDocumento { get; set; }

    [Column(TypeName = "numeric(15, 0)")]
    public decimal? NumeroDocumento { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string NombrePersona { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string Celular { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string CorreoElectronico { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string NumeroComparendo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaRegistro { get; set; }

    public int? IdUsuarioRegistro { get; set; }

    public int? IdCanal { get; set; }

    public int? IdEstadoAgenda { get; set; }

    [Column(TypeName = "numeric(18, 0)")]
    public decimal IdFupa { get; set; }

    [Column(TypeName = "numeric(10, 2)")]
    public decimal? ValorCurso { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? ValorComparendo { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PorcentajeDescuento { get; set; }
}
