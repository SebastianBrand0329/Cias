using ApiCursosCias.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace ApiCursosCias.Services.Helpers.Settings;

public class ApiVersioningErrorResponseProvider : DefaultErrorResponseProvider
{
    public override IActionResult CreateResponse(ErrorResponseContext context)
    {
        return new BadRequestObjectResult(new ResponseProblem
        {
            Title = "One or more validation errors occurred",
            StatusCode = AppSettings.Settings.ErrorResult,
            Status = StatusCodes.Status400BadRequest,
            StatusMessage = "Something went wrong while selecting the api version"
        });
    }
}