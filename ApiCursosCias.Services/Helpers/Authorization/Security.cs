using ApiCursosCias.Models.Entities;
using ApiCursosCias.Services.Helpers.Extension;
using ApiCursosCias.Services.Helpers.Settings;
using IM.Encrypt.Access.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ApiCursosCias.Services.Helpers.Authorization;

public class Security
{
    private static readonly TokenManagement _tokenSettings = AppSettings.Settings.TokenManagement;

    public static string GetToken(string baseString, int? typeRole, string refreshToken)
    {
        var accessExpiration = _tokenSettings.AccessExpiration;
        var expirationType = _tokenSettings.ExpirationType;
        // authentication successful so generate jwt token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_tokenSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _tokenSettings.Issuer,
            Audience = _tokenSettings.Audience,
            Subject = new ClaimsIdentity(new Claim[]
            {
                    new Claim("id", baseString),
                    new Claim("role", typeRole.ToString()),
                    new Claim("refreshToken", refreshToken)
            }),
            Expires = expirationType.Equals("m") ? DateTime.UtcNow.AddMinutes(accessExpiration) : DateTime.UtcNow.AddDays(accessExpiration),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static string[] ValidateJwtToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenSettings.Secret);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                // Specify the key used to sign the token:
                IssuerSigningKey = new SymmetricSecurityKey(key),
                RequireSignedTokens = true,
                // Ensure the token was issued by a trusted authorization server (default true):
                ValidateIssuer = true,
                ValidIssuer = _tokenSettings.Issuer,
                // Ensure the token audience matches our audience value (default true):
                ValidateAudience = true,
                ValidAudience = _tokenSettings.Audience,
                // Clock skew compensates for server time drift.
                // We recommend 5 minutes or less:
                //ClockSkew = TimeSpan.FromMinutes(5),
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero,
                // Ensure the token hasn't expired:
                RequireExpirationTime = true,
                ValidateLifetime = true
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var usuarioId = jwtToken.Claims.First(x => x.Type == "id").Value;
            var role = jwtToken.Claims.First(x => x.Type == "role").Value;
            var refreshToken = jwtToken.Claims.First(x => x.Type == "refreshToken").Value;

            // return user id from JWT token if validation successful
            return new string[] { usuarioId, role, refreshToken };
        }
        catch
        {
            // return null if validation fails
            return null;
        }
    }

    public static Auth Authenticate(Auth auth)
    {
        var authSet = AppSettings.Settings.Decryt();

        auth.User = Encryt.Decrypt(auth.User, authSet.Secret);
        auth.Pass = Encryt.Decrypt(auth.Pass, authSet.Secret);

        var user = authSet.Auth.User.Equals(auth.User) & authSet.Auth.Pass.Equals(auth.Pass) ? auth : null;
        return user;
    }

    public static UsuarioLoginExtended GenerateRefreshToken(string ipAddress)
    {
        var tokenSettings = AppSettings.Settings;
        // generate token that is valid for 1 days
        var randomNumberGenerator = RandomNumberGenerator.Create();
        var randomBytes = new byte[64];
        randomNumberGenerator.GetBytes(randomBytes);
        var refreshExpiration = tokenSettings.TokenManagement.RefreshExpiration;
        var expirationType = tokenSettings.TokenManagement.RefreshExpirationType;
        var refreshToken = new UsuarioLoginExtended
        {
            Token = Convert.ToBase64String(randomBytes),
            Expires = expirationType.Equals("m") ? DateTime.Now.AddMinutes(refreshExpiration).ToString(tokenSettings.DateTimeFormatt) :
                        DateTime.Now.AddDays(refreshExpiration).ToString(tokenSettings.DateTimeFormatt),
            Created = DateTime.Now.ToString(tokenSettings.DateTimeFormatt),
            Createdbyip = ipAddress
        };
        return refreshToken;
    }

    public static UsuarioLoginExtended RotateRefreshToken(UsuarioLoginExtended refreshToken, string ipAddress)
    {
        var newRefreshToken = GenerateRefreshToken(ipAddress);
        RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
        return newRefreshToken;
    }

    public static void RevokeDescendantRefreshTokens(UsuarioLoginExtended refreshToken, List<UsuarioLoginExtended> user, string ipAddress, string reason)
    {
        // recursively traverse the refresh token chain and ensure all descendants are revoked
        if (!string.IsNullOrEmpty(refreshToken.ReplacedbyToken))
        {
            var childToken = user.SingleOrDefault(x => x.Token == refreshToken.ReplacedbyToken);
            if (childToken.IsActive)
                RevokeRefreshToken(childToken, ipAddress, reason);
            else
                RevokeDescendantRefreshTokens(childToken, user, ipAddress, reason);
        }
    }

    public static void RevokeRefreshToken(UsuarioLoginExtended token, string ipAddress, string reason = null, string replacedByToken = null)
    {
        token.Revoked = DateTime.Now.ToString(AppSettings.Settings.DateTimeFormatt);
        token.Revokedbyip = ipAddress;
        token.Reasonrevoked = reason;
        token.ReplacedbyToken = replacedByToken;
    }
}