using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Hire360WebAPI.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Hire360WebAPI.Models;

namespace Hire360WebAPI.Helpers;

public class JwtHelper
{
    private readonly RequestDelegate? _next;
    private readonly AppSettings? _appSettings;

    public JwtHelper(RequestDelegate next, IOptions<AppSettings> appsettings)
    {
        _next = next;
        _appSettings = appsettings.Value;
    }

    public async Task Invoke(HttpContext context, IHumanResourceServices humanResourceServices, ICandidateServices candidateServices)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
            attachUserToContext(humanResourceServices, candidateServices, context, token);
        await _next!(context);
    }

    private void attachUserToContext(IHumanResourceServices humanResourceServices, ICandidateServices candidateServices, HttpContext context, string token)
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

            var hr = humanResourceServices.GetById(userID);

            if(hr != null)
                context.Items["User"] = new AuthResponse(hr.Hrid, hr.Hrname, hr.UserRole, "");
            else{
                var candidate = candidateServices.GetById(userID);
                context.Items["User"] = new AuthResponse(candidate.CandidateId, candidate.CandidateName, candidate.UserRole, "");
            }
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}