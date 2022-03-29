using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Hire360WebAPI.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Hire360WebAPI.Helpers;

public class JwtHelperHR
{
    private readonly RequestDelegate? _next;
    private readonly AppSettings? _appSettings;

    public JwtHelperHR(RequestDelegate next, IOptions<AppSettings> appsettings)
    {
        _next = next;
        _appSettings = appsettings.Value;
    }

    public async Task Invoke(HttpContext context, IHumanResourceServices humanResourceServices)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
            attachUserToContext(humanResourceServices, context, token);
        await _next!(context);
    }

    private void attachUserToContext(IHumanResourceServices humanResourceServices, HttpContext context, string token)
    {
        try
        {
            var key = Encoding.ASCII.GetBytes(_appSettings!.Secret!);
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                },
                out SecurityToken validateToken
            );

            var jwtToken = (JwtSecurityToken)validateToken;
            var userID = Guid.Parse(jwtToken.Claims.FirstOrDefault(x => x.Type == "Id")!.Value);

            context.Items["HR"] = humanResourceServices.GetById(userID);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}