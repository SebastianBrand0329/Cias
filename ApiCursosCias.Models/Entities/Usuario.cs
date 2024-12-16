using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiCursosCias.Models.Entities;

public class Usuario
{
	public int IdUsuario { get; set; }
	public decimal? UserName { get; set; }
	public string Clave { get; set; }

	[NotMapped]
	public string ClaveActual { get; set; }

	public string NombreUsuario { get; set; }
	public int? IdTipoDocumento { get; set; }
	public decimal? Identificacion { get; set; }
	public string Correo { get; set; }
	public string Celular { get; set; }
	public int? IdEmpresa { get; set; }
	public int? IdSede { get; set; }
	public bool? ClaveActualizada { get; set; }
	public int? IdEstado { get; set; }
	public int? IdRol { get; set; }
	public DateTime? FechaIngreso { get; set; }
	public int? IdUsuarioRegistra { get; set; }

	[NotMapped]
	[JsonIgnore]
	public Notificacion Notificacion { get; set; }
}

public class UsuarioLogin
{
	public int IdLogin { get; set; }
	public DateOnly? LoginDate { get; set; }
	public decimal? CodeLogin { get; set; }
	public string Token { get; set; }
	public string Expires { get; set; }
	public string Created { get; set; }
	public string Createdbyip { get; set; }
	public string Revoked { get; set; }
	public string Revokedbyip { get; set; }
	public string ReplacedbyToken { get; set; }
	public string Reasonrevoked { get; set; }
}

public class UsuarioLoginExtended : UsuarioLogin
{
	public bool IsExpired => DateTime.Now >= DateTime.Parse(Expires);
	public bool IsRevoked => Revoked != null;
	public bool IsActive => !IsRevoked && !IsExpired;
}

public class UsuarioExtend : Usuario
{
	public string Empresa { get; set; }
	public string Rol { get; set; }
	public string Token { get; set; }
	public List<Funcionalidad> Funcionalidades { get; set; }

	[JsonIgnore] // refresh token is returned in http only cookie
	public string RefreshToken { get; set; }

	[JsonIgnore]
	public List<UsuarioLoginExtended> RefreshTokens { get; set; } = new List<UsuarioLoginExtended>();
}

public class UsuarioAgendador : Usuario
{
	public string TipoDocumento { get; set; }
	public string Empresa { get; set; }
	public string Sede { get; set; }
	public string Rol { get; set; }
}