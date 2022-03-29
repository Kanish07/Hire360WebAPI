using System.IdentityModel.Tokens.Jwt;
using Hire360WebAPI.Helpers;
using Hire360WebAPI.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Hire360WebAPI.Services;

public interface ICandidateServices
{
    AuthResponse Authenticate(Candidate model);
    IEnumerable<Candidate> GetAll();
    Candidate GetById(Guid Id);
}

public class CandidateServices : ICandidateServices
{
    private readonly Hire360Context _context;

    private readonly AppSettings _appSettings;

    public CandidateServices(Hire360Context context, IOptions<AppSettings> appSettings)
    {
        _context = context;
        _appSettings = appSettings.Value;
    }

    public AuthResponse Authenticate(Candidate candidate)
    {       
        var token = GenerateJwtToken(candidate);
        return new AuthResponse(candidate.CandidateId, candidate.CandidateName, candidate.UserRole, token);
    }

    public IEnumerable<Candidate> GetAll()
    {
        return _context.Candidates.ToList();
    }

    public Candidate GetById(Guid Id)
    {
        return _context.Candidates.FirstOrDefault(x => x.CandidateId == Id)!;
    }

    private string GenerateJwtToken(Candidate candidate)
    {
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret!);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                new[]
                {
                    new Claim("Id", candidate.CandidateId.ToString())
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