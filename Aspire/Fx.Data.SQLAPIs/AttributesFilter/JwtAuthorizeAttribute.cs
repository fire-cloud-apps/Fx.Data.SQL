using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Serilog;
using Fx.Security.JWT;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Fx.Data.SQLAPIs.AttributesFilter;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class JwtAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public JwtAuthorizeAttribute(string role)
    {

    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        #region Initializor to Get Value
        Log.Information($"Issuer: {GlobalConfig.JWTMeta.Issuer}");
        var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault().Split(" ")[1];
        Log.Information($"Token: {token}");

        #endregion

        #region Token Verifier

        var jwtToken = new JwtTokenHandlerRSA(GlobalConfig.JWTMeta.PublicKey, GlobalConfig.JWTMeta.PrivateKey);

        ClaimsPrincipal claims;
        var isValid = jwtToken.VerifyToken(token: token,
            issuer: GlobalConfig.JWTMeta.Issuer,
            audience: GlobalConfig.JWTMeta.Audiance, out claims);
        if (isValid)
        {
            User user = new User();
            user.FirstName = claims.FindFirst(ClaimTypes.Name).Value;
            user.Id = int.Parse(claims.FindFirst(ClaimTypes.NameIdentifier).Value);
            user.Username = claims.FindFirst(ClaimTypes.PrimarySid).Value;
            Log.Information($"Verify Claim User: {JsonConvert.SerializeObject(user)}");
            //context.Result = new JsonResult(new { message = "Authorized" }) { StatusCode = StatusCodes.Status200OK };
        }
        else
        {
            Log.Warning($"Invalid Token or Token Expired.");
            //context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            context.Result = new UnauthorizedObjectResult(new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized });
        }
        #endregion
    }
}

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Company { get; set; }
}

public class AdminRoleRequirement : IAuthorizationRequirement
{
    public AdminRoleRequirement(string role) => Role = role;
    public string Role { get; set; }
}

public class AdminRoleRequirementHandler : AuthorizationHandler<AdminRoleRequirement>
{
    public AdminRoleRequirementHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRoleRequirement requirement)
    {
        if (context.User.HasClaim(c => c.Value == requirement.Role))
        {
            context.Succeed(requirement);
        }
        else
        {

            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            _httpContextAccessor.HttpContext.Response.ContentType = "application/json";
            await _httpContextAccessor.HttpContext.Response.WriteAsJsonAsync(new { StatusCode = StatusCodes.Status401Unauthorized, Message = "Unauthorized. Required admin role." });
            await _httpContextAccessor.HttpContext.Response.CompleteAsync();

            context.Fail();

        }

    }
    private readonly IHttpContextAccessor _httpContextAccessor;
}
