using System.IdentityModel.Tokens.Jwt;
using Hire360WebAPI.Helpers;
using Hire360WebAPI.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Hire360WebAPI.Services;

public interface IHumanResourceServices
{
    AuthResponse Authenticate(HumanResource model);
    IEnumerable<HumanResource> GetAll();
    HumanResource GetById(Guid Id);
}

public class HumanResourceServices : IHumanResourceServices
{
    private readonly Hire360Context _context;

    private readonly AppSettings _appSettings;

    public HumanResourceServices(Hire360Context context, IOptions<AppSettings> appSettings)
    {
        _context = context;
        _appSettings = appSettings.Value;
    }

    public AuthResponse Authenticate(HumanResource humanResource)
    {       
        var token = GenerateJwtToken(humanResource);
        return new AuthResponse(humanResource.Hrid, humanResource.Hrname, humanResource.UserRole, token);
    }

    public IEnumerable<HumanResource> GetAll()
    {
        return _context.HumanResources.ToList();
    }

    public HumanResource GetById(Guid Id)
    {
        return _context.HumanResources.FirstOrDefault(x => x.Hrid == Id)!;
    }

    private string GenerateJwtToken(HumanResource humanResource)
    {
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret!);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                new[]
                {
                    new Claim("Id", humanResource.Hrid.ToString())
                }
            ),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}