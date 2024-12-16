using ApiCursosCias.Models.Entities;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace ApiCursosCias.Services.Helpers.Middleware;

public class ErrorHandler
{
    private readonly RequestDelegate _next;

    public ErrorHandler(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = error switch
            {
                AppException => (int)HttpStatusCode.BadRequest,// custom application error
                KeyNotFoundException => (int)HttpStatusCode.NotFound,// not found error
                _ => (int)HttpStatusCode.InternalServerError,// unhandled error
            };
            var result = JsonSerializer.Serialize(new ResponseProblem { StatusMessage = error?.Message });
            await response.WriteAsync(result);
        }
    }
}