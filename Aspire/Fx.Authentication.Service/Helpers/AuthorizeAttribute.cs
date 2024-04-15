using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Security.Claims;
using System.Linq;
using Newtonsoft.Json;
using Fx.Security.Helper;
using Fx.Security.JWT;
using Sun.Delivery.API;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context )
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