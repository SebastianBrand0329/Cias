using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Tr_PagoAgenda")]
public partial class TrPagoAgendum
{
    [Key]
    public int IdPagoAgenda { get; set; }

    public int? IdAgendaCurso { get; set; }

    [Column(TypeName = "numeric(18, 0)")]
    public decimal? IdFupa { get; set; }

    public int? IdConsecutivoPago { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaPago { get; set; }

    public int? IdUsuario { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? ValorComparendo { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PorcentajeDescuento { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? ValorDescuento { get; set; }

    [Column("ValorAPagar", TypeName = "numeric(18, 2)")]
    public decimal? ValorApagar { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? ValorTotalPagar { get; set; }

    public int? IdSede { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string CodigoInfraccion { get; set; }

    [Column(TypeName = "numeric(10, 2)")]
    public decimal? ValorCurso { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MunicipioInfraccion { get; set; }

    [Column("horaregistro")]
    [StringLength(20)]
    [Unicode(false)]
    public string Horaregistro { get; set; }
}
