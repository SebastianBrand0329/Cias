using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

[Table("Tr_Usuario_Login")]
public partial class TrUsuarioLogin
{
    [Key]
    public int IdLogin { get; set; }

    public DateOnly? LoginDate { get; set; }

    [Column(TypeName = "numeric(15, 0)")]
    public decimal? CodeLogin { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string Token { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Expires { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Created { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Createdbyip { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Revoked { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Revokedbyip { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string ReplacedbyToken { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string Reasonrevoked { get; set; }
}
