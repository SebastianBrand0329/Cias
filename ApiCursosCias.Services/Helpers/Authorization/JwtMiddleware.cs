using ApiCursosCias.Services.Helpers.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;
using System.Text;

namespace ApiCursosCias.Services.Helpers.Authorization;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            if (ValidateBasicAuth(context))
            {
                if (context.Request.Headers.TryGetValue("token", out StringValues headerToken))
                {
                    var token = headerToken.FirstOrDefault();
                    if (token != null)
                    {
                        var usuario = Security.ValidateJwtToken(token);
                        if (usuario != null)
                            context.Items["usuario"] = usuario;
                    }
                }
            }
        }
        await _next(context);
    }

    private static bool ValidateBasicAuth(HttpContext context)
    {
        try
        {
            Auth user;
            var authHeader = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
            var auth = new Auth { User = credentials[0], Pass = credentials[1] };
            user = Security.Authenticate(auth);
            if (user == null)
                return false;
            context.Items["UserBasicAuth"] = auth.User;
            return true;
        }
        catch
        {
            return false;
        }
    }
}