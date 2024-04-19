using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Security.Claims;
using System.Xml;
using Fx.Security;
using Fx.Security.Helper;
using Fx.Security.JWT;
using System.ComponentModel.DataAnnotations;

namespace Fx.Authentication.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    #region Initialization
    //private IUserService _userService;
    private readonly ILogger<UsersController> _logger;
    //private IUserDetailAccess _userDetailsDA;
    private readonly IConfiguration _configuration;
    #endregion

    #region Constructor
    public UsersController(ILogger<UsersController> logger, IConfiguration configuration)
    {
        //_userService = userService;
        _logger = logger;
        _configuration = configuration;
        //string connectionString = configuration.GetValue<string>("DBSettings:UserConnectionString");
        //Default initialization
        //_userDetailsDA = new UserAccess(connectionString);
    }
    #endregion

    #region Authentication Layer 
    [HttpPost("Authenticate")]
    public IActionResult Authenticate(AuthenticateRequest model)
    {
        /*
         * Works@San# -> Password.
         * JohnMiller -> UserName
         */
        var response = UserAuth(model); //_userService.Authenticate(model);

        if (response == null)
            return BadRequest(new { message = "Username or password is incorrect" });

        _logger.LogInformation("Hello, User Name {Name}!", model.Username);
        return Ok(response);
    }

    private AuthenticateResponse UserAuth(AuthenticateRequest model)
    {
        //var user = _userDetailsDA.VerifyLogin(model.Username, model.Password);
        var user = new User()
        {
            FirstName = "Ganesh",
            Company = "FSx",
            Id = 10,
            Role =model.Username,
            LastName = "Ram",
            Password = "System@14",
            Username = "sr.ganesh"
        };
        // return null if user not found
        if (user == null) return null;

        var token = GenerateJWT(user);

        return new AuthenticateResponse(user, token);
    }
    #endregion

    #region Authentication Test
    [HttpGet("TokenCheck")]
    [Authorize]
    public IActionResult TokenChecker()
    {
        return Ok("Hello, User!");
    }
    #endregion

    #region Generate Token
    private string GenerateJWT(User user)
    {
        var jwtToken = new JwtTokenHandlerRSA(GlobalConfig.JWTMeta.PublicKey, GlobalConfig.JWTMeta.PrivateKey);
        // Have what details you need to add in the user claim.
        var claims = new[]
        {
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Email, $"{user.Username}@suntoyo.com"),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
                new Claim("Role", user.Role),//In realtime this should come from the database.
                new Claim("Company", user.Company)
        };

        var token = jwtToken.GenerateToken(GlobalConfig.JWTMeta, claims);

        return token;
    }
    #endregion

    #region Create API's
    /// <summary>
    /// Create Users 
    /// </summary>
    /// <param name="userDetails">Users as JSON</param>
    /// <returns>Action Result</returns>
    [Route("Create")]
    //[Authorize]
    [HttpPost]
    public IActionResult Post([FromBody] User userDetails)
    {
        IActionResult actionResult = null;
        //Works@San# -> Password.
        //JohnMiller -> UserName
        try
        {
            _logger.LogInformation($"Post -> User {JsonConvert.SerializeObject(userDetails)}");
            //string uniqueId = _userDetailsDA.Create(userDetails);
            string uniqueId = "1";
            userDetails.Id = int.Parse(uniqueId);
            //Converts to Required JSON
            string result = JsonConvert.SerializeObject(userDetails, Newtonsoft.Json.Formatting.Indented);
            actionResult = Ok(result);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Post -> Users {JsonConvert.SerializeObject(userDetails)}");
            actionResult = StatusCode(500, $"Error: {ex.Message} for more details refer log.");
        }
        return actionResult;
    }

    #endregion

    #region Update API's
    /// <summary>
    /// Update Users
    /// </summary>
    /// <param name="userDetails">Users as JSON</param>
    /// <returns>Action Result</returns>
    [Route("Update")]
    //[Authorize]
    [HttpPut]
    public IActionResult Put([FromBody] User userDetails)
    {
        IActionResult actionResult = null;
        try
        {
            _logger.LogInformation($"Put -> User {JsonConvert.SerializeObject(userDetails)}");
            //var result = _userDetailsDA.Update(userDetails);
            //actionResult = Ok(result);
            actionResult = Ok("Udated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Put -> Users {JsonConvert.SerializeObject(userDetails)}");
            actionResult = StatusCode(500, $"Error: {ex.Message} for more details refer log.");
        }
        return actionResult;
    }
    #endregion

    #region Delete API's
    /// <summary>
    /// Delete Users
    /// </summary>
    /// <param name="userDetails">Users as JSON</param>
    /// <returns>Action Result</returns>
    [Route("Delete")]
    //[Authorize]
    [HttpDelete]
    public IActionResult Delete(int userId)
    {
        IActionResult actionResult = null;
        try
        {
            _logger.LogInformation($"Delete -> User {userId}");
            //var result = _userDetailsDA.Delete(userId);
            //actionResult = Ok(result);
            actionResult = Ok("Deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Delete -> Users {userId}");
            actionResult = StatusCode(500, $"Error: {ex.Message} for more details refer log.");
        }
        return actionResult;
    }
    #endregion

    #region Reset Password
    /// <summary>
    /// Update Users
    /// </summary>
    /// <param name="passwordReset">Password Reset as JSON</param>
    /// <returns>Action Result</returns>
    [Route("PasswordReset")]
    //[Authorize]
    [HttpPut]
    public IActionResult ResetPassword(PasswordReset passwordReset)
    {
        IActionResult actionResult = null;
        try
        {
            _logger.LogInformation($"ResetPassword -> User {JsonConvert.SerializeObject(passwordReset)}");
            //var result = _userDetailsDA.ResetPassword(passwordReset.Password, passwordReset.UserId);
            //actionResult = Ok(result);
            actionResult = Ok("Reset");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"ResetPassword -> Users {JsonConvert.SerializeObject(passwordReset)}");
            actionResult = StatusCode(500, $"Error: {ex.Message} for more details refer log.");
        }
        return actionResult;
    }
    #endregion
}
    

public class PasswordReset
{
    public string Password { get; set; }
    public string UserId { get; set; }
}

public class AuthenticateRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}


public class AuthenticateResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Token { get; set; }


    public AuthenticateResponse(User user, string token)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Username = user.Username;
        Token = token;
    }
}