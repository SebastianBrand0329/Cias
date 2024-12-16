using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiCursosCias.Models.Entities;

public class Consulta
{
	[JsonPropertyName("placa")]
	public string Placa { get; set; }

	[JsonPropertyName("idTipoDocumento")]
	public int IdTipoDocumento { get; set; }

	[JsonPropertyName("numeroDocumento")]
	public string NumeroDocumento { get; set; }

	[JsonPropertyName("reCaptchaDTO")]
	public DtoChaptCha ReCaptchaDTO { get; set; }
}

public class DtoChaptCha
{
	[JsonPropertyName("response")]
	public string Response { get; set; }

	[JsonPropertyName("consumidor")]
	public string Consumidor { get; set; }
}

public class ConsultaInfo
{
	[JsonPropertyName("filtro")]
	public string Filtro { get; set; }

	[JsonPropertyName("reCaptchaDTO")]
	public DtoChaptCha ReCaptchaDTO { get; set; }
}

public class InformacionSimit
{
	public List<MultasDto> multas { get; set; }
	public List<infractorDto> personasMismoDocumento { get; set; }
	public int? codigo { get; set; } = 0;
	public string descripcion { get; set; }
}

public class MultasDto
{
	public infractorDto infractor { get; set; }
	public int? valor { get; set; }
	public string placa { get; set; }
	public List<infraccionesDto> infracciones { get; set; }
	public List<proyeccionDto> proyeccion { get; set; }
	public bool comparendo { get; set; }
	public bool impreso { get; set; }
	public int? valorPagar { get; set; }
	public int? porcentajeDescuentoIntereses { get; set; }
	public int? porcentajeDescuentoCapita { get; set; }
	public int? idDepartamento { get; set; }
	public string departamento { get; set; }
	public string referenciaPago { get; set; }
	public string estadoPago { get; set; }
	public int? idResolucion { get; set; } = 0;
	public string numeroResolucion { get; set; }
	public string organismoTransito { get; set; }
	public string idOrganismoTransito { get; set; }
	public string estadoCartera { get; set; }
	public int? idEstadoCartera { get; set; }
	public string comparendosElectronicos { get; set; }
	public string polca { get; set; }
	public double? valorIntereses { get; set; }
	public int? idTipoResolucion { get; set; }
	public double? valorDescuentoIntereses { get; set; }
	public double? valorDescuentoCapital { get; set; }
	public double? valorGestion { get; set; }
	public bool liquidacionCoactivoParam18 { get; set; }
	public bool liquidarIncumpApParam135 { get; set; }
	public string fechaComparendo { get; set; }
	public string fechaResolucion { get; set; }
	public string cisa { get; set; }
	public string numeroComparendo { get; set; }
	public string nroCoactivo { get; set; }
	public string fechaCoactivo { get; set; }
	public string fechaHasta { get; set; }
	public int? consecutivoComparendo { get; set; }
	public string estadoComparendo { get; set; }
	public bool comparendoElectronico { get; set; }
	public string poseeCurso { get; set; }
	public int? idEstadoComparendo { get; set; }
	public decimal? valorDescuentoProntoPago { get; set; }
	public string fechaNotificacion { get; set; }
	public bool renuencia { get; set; }
	public bool contieneInfraccionesEmbriaguez { get; set; }
	public bool contieneInfraccionesHG { get; set; }
	public string tutor { get; set; }
	public int? IdtipoComparendo { get; set; }
}

public class infractorDto
{
	public string tipoDocumento { get; set; }
	public string numeroDocumento { get; set; }
	public string nombre { get; set; }
	public string apellido { get; set; }
	public int? idTipoDocumento { get; set; }
}

public class infraccionesDto
{
	public string codigoInfraccion { get; set; }
	public string descripcionInfraccion { get; set; }
	public int valorInfraccion { get; set; }
}

public class proyeccionDto
{
	public string descripcion { get; set; }
	public double valor { get; set; }
	public string fecha { get; set; }
	public int dias { get; set; }
	public string instrucciones { get; set; }
}

public class ConsultaComparendo
{
	public int IdConsultaComparendo { get; set; }

	public string Comparendo { get; set; }

	public string Descripcion { get; set; }
}

public class SetConsulta
{
	public string NumeroComparendo { get; set; }
	public string Consulta { get; set; }
}