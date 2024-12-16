using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Net;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiCursosCias.Models.Entities;

public class Request<T>
{
    [JsonPropertyName("data"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public T? Data { set; get; }

    [JsonPropertyName("versionCode"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? VersionCode { set; get; }

    [JsonPropertyName("versionName"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? VersionName { set; get; }

    [JsonPropertyName("language"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Language { set; get; }

    [JsonPropertyName("platform"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Platform { set; get; }

    [JsonPropertyName("idApp"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? IdApp { set; get; }

    [JsonPropertyName("app"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? App { set; get; }
}

public class Response<T>
{
    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; }

    [JsonPropertyName("httpStatusCode")]
    public HttpStatusCode HttpStatusCode { get; set; }

    [JsonPropertyName("statusCode")]
    public int StatusCode { set; get; }

    [JsonPropertyName("statusMessage")]
    public string StatusMessage { set; get; }

    [JsonPropertyName("result")]
    public T Result { set; get; }
}

public class ResponseProblem
{
    [JsonPropertyName("type"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Type { get; set; }

    [JsonPropertyName("title"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Title { get; set; }

    [JsonPropertyName("status"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Status { get; set; }

    [JsonPropertyName("detail"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Detail { get; set; }

    [JsonPropertyName("statusCode"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? StatusCode { set; get; }

    [JsonPropertyName("statusMessage"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string StatusMessage { set; get; }

    public override string ToString() => JsonSerializer.Serialize(this);
}

public class AppException : Exception
{
    public AppException() : base()
    {
    }

    public AppException(string message) : base(message)
    {
    }

    public AppException(string message, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}

[Serializable]
public class ResultException : Exception
{
    private static readonly string DefaultMessage = ".";

    public int Code { get; set; }
    public string Detail { get; set; }

    public ResultException() : base(DefaultMessage)
    {
    }

    public ResultException(string message) : base(message)
    {
    }

    public ResultException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public ResultException(int code, string detail) : base(DefaultMessage)
    {
        Code = code;
        Detail = detail;
    }

    public ResultException(int code, string detail, Exception innerException) : base(DefaultMessage, innerException)
    {
        Code = code;
        Detail = detail;
    }

    protected ResultException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

public class Plataforma
{
    [JsonIgnore]
    public int? IdVersion { set; get; }

    [JsonPropertyName("versionCode"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? VersionCode { set; get; }

    [JsonPropertyName("versionName"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string VersionName { set; get; }

    [JsonPropertyName("language"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Language { set; get; }

    [JsonPropertyName("platform"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Platform { set; get; }

    [JsonPropertyName("idApp"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? IdApp { set; get; }

    [JsonPropertyName("APP"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string App { set; get; }
}

public class RequestFile<T>
{
    [FromJson]
    public T DataContent { set; get; }

    public List<IFormFile> Images { set; get; }
}