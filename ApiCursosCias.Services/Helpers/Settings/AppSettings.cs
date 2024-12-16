using ApiCursosCias.Models.Entities;

namespace ApiCursosCias.Services.Helpers.Settings;

public class AppSettings
{
	private static AppSettings settings;
	public static AppSettings Settings { get => settings; set => settings = value; }
	public bool Decrypt { set; get; }
	public string Secret { get; set; }
	public string Connection { get; set; }
	public string[] ApiVersion { set; get; }
	public int ResponseResult { get; set; }
	public int ErrorResult { get; set; }
	public string DateTimeFormatt { set; get; }
	public string UrlApiMensajeria { get; set; }
	public TokenManagement TokenManagement { set; get; }
	public Auth Auth { set; get; }
	public LoggingSettings Logging { set; get; }
	public NotificationSettings NotificationSettings { set; get; }
	public EmailSettings EmailSettings { get; set; }
	public IntegrationSettings Integration { set; get; }
	public RepositorySettings Repository { set; get; }
}

public class TokenManagement
{
	public string Secret { get; set; }
	public string Issuer { get; set; }
	public string Audience { get; set; }
	public string ExpirationType { get; set; }
	public int AccessExpiration { get; set; }
	public string RefreshExpirationType { get; set; }
	public int RefreshExpiration { get; set; }
}

public class Auth
{
	public string User { set; get; }
	public string Pass { set; get; }
	public string Token { set; get; }
}

public enum TypeRole
{ Administrativo = 1, Agendador = 2 }

public class RequestContext
{
	public static string UsuarioID { set; get; }
	public static Plataforma Plataforma { set; get; }
}

public class LoggingSettings
{
	public string DefaultConnection { get; set; }
	public string FolderFile { get; set; }
	public string FolderDatabase { get; set; }
	public string DbName { get; set; }
	public List<string> Segments { set; get; }
}

public class EmailSettings
{
	public string Server { get; set; }
	public string From { get; set; }
	public string Pass { get; set; }
	public int Port { get; set; }
}

public class IntegrationSettings
{
	public List<ServicesIntegration> Services { set; get; }
}

public class ServicesIntegration
{
	public string Name { set; get; }
	public string Url { set; get; }
	public Auth Authentication { set; get; }
	public List<Metodos> Methods { set; get; }
}

public class Metodos
{
	public string Method { set; get; }
	public string Value { set; get; }
}

public class RepositorySettings
{
	public string Directory { set; get; }
	public string Url { set; get; }
	public List<PathFolder> Folders { get; set; }
}

public class PathFolder
{
	public string Name { get; set; }
	public string File { get; set; }
	public string Path { get; set; }
}

public class NotificationSettings
{
	public string ScheduledTime { get; set; }
	public string ScheduledTimeReserva { get; set; }
	public string Log { get; set; }
}