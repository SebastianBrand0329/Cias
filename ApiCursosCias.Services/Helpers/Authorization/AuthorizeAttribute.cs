using ApiCursosCias.Models.Entities;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System;
using ApiCursosCias.Services.Helpers.Settings;
using RequestContext = ApiCursosCias.Services.Helpers.Settings.RequestContext;

namespace ApiCursosCias.Services.Helpers.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly IList<TypeRole> _roles;

    public AuthorizeAttribute(params TypeRole[] roles)
    {
        _roles = roles ?? Array.Empty<TypeRole>();
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // skip authorization if action is decorated with [AllowAnonymous] attribute
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
            return;

        var userBasicAuth = (string)context.HttpContext.Items["UserBasicAuth"];
        if (string.IsNullOrEmpty(userBasicAuth))
            context.Result = new JsonResult(new { message = "Unauthorized Basic" }) { StatusCode = StatusCodes.Status401Unauthorized };
        else
        {
            // authorization
            var action = context.ActionDescriptor.RouteValues.Where(a => a.Key.Equals("action")).FirstOrDefault().Value.ToLower();
            if (!action.Equals("gettoken") &
            !action.Equals("refreshtoken") &
            !action.Equals("autenticarusuario") &
                !action.Equals("passwordforgot"))
            {
                var usuario = (string[])context.HttpContext.Items["usuario"];
                if (usuario == null || (_roles.Any() && !_roles.Contains((TypeRole)int.Parse(usuario[1]))))
                    context.Result = new JsonResult(new { message = "Unauthorized Method Operation" }) { StatusCode = StatusCodes.Status401Unauthorized };
                else
                {
                    var usuarioId = RequestContext.UsuarioID = usuario[0];
                    if (usuarioId == null)
                        context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
            }
        }
    }
}

public class ValidateParamFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var jobj = context.ActionArguments["request"];
        if (jobj != null)
        {
            var json = JsonSerializer.Serialize(jobj);

            var plataforma = JsonSerializer.Deserialize<Plataforma>(json);
            if (plataforma != null)
                RequestContext.Plataforma = plataforma;
            else
                context.Result = new JsonResult(new ResponseProblem { StatusMessage = "plataforma no valida" });
        }
        else
            context.Result = new JsonResult(new ResponseProblem { StatusMessage = "request no valido" });

        base.OnActionExecuting(context);
    }
}