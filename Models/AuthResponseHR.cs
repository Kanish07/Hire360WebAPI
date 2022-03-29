using Hire360WebAPI.Entities;
namespace Hire360WebAPI.Models;

public class AuthResponseHR
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public Role Role { get; set; }
    public string? Token { get; set; }

    public AuthResponseHR(HumanResource humanResource, string token)
    {
        Id = humanResource.Hrid;
        Username = humanResource.Hrname;
        Role = humanResource.UserRole;
        Token = token;
    }
};