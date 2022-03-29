using Hire360WebAPI.Entities;
namespace Hire360WebAPI.Models;

public class AuthResponseCandidate
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public Role Role { get; set; }
    public string? Token { get; set; }

    public AuthResponseCandidate(Candidate candidate, string token)
    {
        Id = candidate.CandidateId;
        Username = candidate.CandidateName;
        Role = candidate.UserRole;
        Token = token;
    }
};