using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Tr_ConsultaComparendo")]
public partial class TrConsultaComparendo
{
    [Key]
    public int IdConsultaComparendo { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string Comparendo { get; set; }

    [Unicode(false)]
    public string Descripcion { get; set; }
}
