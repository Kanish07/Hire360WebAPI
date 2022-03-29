using Hire360WebAPI.Entities;
namespace Hire360WebAPI.Models;

public class AuthResponse
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public Role Role { get; set; }
    public string? Token { get; set; }

    public AuthResponse(Guid id, string username, Role role, string token)
    {
        Id = id;
        Username = username;
        Role = role;
        Token = token;
    }
};