using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Hire360WebAPI.Models;
using Hire360WebAPI.Entities;
namespace Hire360WebAPI.Helpers.CandidateJWT;

[AttributeUsage(AttributeTargets.Method)]
public class AllowAnonymousAttributeCandidate : Attribute
{ }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly IList<Role> _roles;

    public AuthorizeAttribute(params Role[] roles)
    {
        _roles = roles ?? new Role[] { };
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata
            .OfType<AllowAnonymousAttributeCandidate>()
            .Any();
        if (allowAnonymous)
            return;

        var candidate = (Candidate)context.HttpContext.Items["Candidate"]!;
        // var humanResource = (HumanResource)context.HttpContext.Items["HR"]!;
        if (candidate == null)
        {
            context.Result = new JsonResult(new { message = "unauthorized" })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }
        else if (!_roles.Contains(candidate.UserRole) && _roles.Any())
        {
            context.Result = new JsonResult(new { message = "forbidden" })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}