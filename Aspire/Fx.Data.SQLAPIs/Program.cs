using Fx.Data.SQL.Handler;
using Fx.Data.SQL.Helpers;
using Fx.Data.SQL.Interface;
using Fx.Data.SQLAPIs.AttributesFilter;
using Fx.Security.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using Parameters = System.Collections.Generic.Dictionary<string, string>;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Configuration
var jwtMeta = new JWTDetails()
{
    Issuer = builder.Configuration.GetValue<string>("JWT:Issuer"),
    Audiance = builder.Configuration.GetValue<string>("JWT:Issuer"),
    Subject = builder.Configuration.GetValue<string>("JWT:Subject"),
    PrivateKey = builder.Configuration.GetValue<string>("JWT:PrivateKey"),
    PublicKey = builder.Configuration.GetValue<string>("JWT:PublicKey"),
    Expiry = DateTime.UtcNow.AddHours(int.Parse(builder.Configuration.GetValue<string>("JWT:Expiry")))
};
GlobalConfig.JWTMeta = jwtMeta;
#endregion

#region Authentication Middleware
RSA _rsa;
_rsa = RSA.Create();
_rsa.ImportRSAPublicKey(Convert.FromBase64String(jwtMeta.PublicKey), out _);
_rsa.ImportRSAPrivateKey(Convert.FromBase64String(jwtMeta.PrivateKey), out _);
var jwtToken = new JwtTokenHandlerRSA(GlobalConfig.JWTMeta.PublicKey, GlobalConfig.JWTMeta.PrivateKey);
//var isValid = jwtToken.VerifyToken(token: token,
//            issuer: GlobalConfig.JWTMeta.Issuer,
//            audience: GlobalConfig.JWTMeta.Audience, out claims);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwtMeta.Issuer,
        ValidAudience = jwtMeta.Audiance,
        //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtMeta.PublicKey)),
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new RsaSecurityKey(_rsa),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});
builder.Services.AddAuthorization();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region Enables Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();
#endregion

app.UseHttpsRedirection();

#region Exception Handling
app.UseExceptionHandler(exceptionHandlerApp =>
{
//Ref: https://learn.microsoft.com/en-us/answers/questions/697739/error-handling-in-net6-minimal-api
exceptionHandlerApp.Run(async httpContext =>
{
var exceptionHandlerPathFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
if (exceptionHandlerPathFeature is not null)
{
var exception = exceptionHandlerPathFeature.Error;// for custom exceptions and filters 
app.Logger.LogError(exception, exception.Message);
ResponseModel responseModel = new ResponseModel()
{
Data = Array.Empty<string>(),
Message = exception.Message,
Success = false
};
await httpContext.Response.WriteAsJsonAsync(responseModel);
}
});
});


app.MapGet("/exception", () =>
{
throw new InvalidOperationException("Sample Exception");
});
#endregion

app.MapGet("/hello-world", () => "Hello World!").RequireAuthorization();
app.MapGet("/hello-no-auth", () => "Invoked without Token!").AllowAnonymous();

#region Generate Token
app.MapPost("/security/createToken",
[AllowAnonymous] ([FromBody] UserTemp user) =>
{
    if (user.UserName == "srg" && user.Password == "srg")
    {
        var issuer = jwtMeta.Issuer;
        var audience = jwtMeta.Audiance;
        var key = Encoding.ASCII.GetBytes(jwtMeta.PublicKey);
        var claims = new[]
            {
                new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
            };

        #region  Token Old Code
        //var tokenDescriptor = new SecurityTokenDescriptor
        //{
        //    Subject = new ClaimsIdentity(claims),
        //    Expires = DateTime.UtcNow.AddMinutes(50),
        //    Issuer = issuer,
        //    Audience = audience,
        //    SigningCredentials = new SigningCredentials
        //    (new SymmetricSecurityKey(key),
        //    SecurityAlgorithms.HmacSha512Signature)
        //};
        //var tokenHandler = new JwtSecurityTokenHandler();
        //var token = tokenHandler.CreateToken(tokenDescriptor);
        //var jwtTokenx = tokenHandler.WriteToken(token);
        //var stringToken = tokenHandler.WriteToken(token);
        //return Results.Ok(stringToken);
        #endregion

        var token  = jwtToken.GenerateToken(jwtMeta, claims);
        return Results.Ok(token);
        
    }
    return Results.Unauthorized();
});
#endregion

#region Demo API
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};
//[Fx.Data.SQLAPIs.AttributesFilter.Authorize]
app.MapGet("/weather-forecast", [JwtAuthorize] () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();
//.RequireAuthorization();
#endregion

#region DB Settings
string sql_connectionString = "Server=localhost;Database={0};User Id=sa;Password=System@1984;TrustServerCertificate=True;";//DMS
string sqlite_ConnectionString = "Data Source=C:\\Works\\Github\\Fx.Data.SQL\\Fx.SQL.ServiceAPIs\\Db\\{0};";
IEntityService entityService;
#endregion

#region Generic Insertion
//SQL Server
app.MapPost("mssql-api/{db}/{entity}/create", (string db, string entity, [FromBody] Parameters data) =>
{
    entityService = Common.GetSQLServerEntityService(sql_connectionString, db, app.Logger);
    var resultId = entityService.Create(db, entity, data);
    return ReturnAsSuccess(resultId);
}).RequireAuthorization()
    ;
//SQLite
app.MapPost("sqlite-api/{db}/{entity}/create", (string db, string entity, [FromBody] Parameters data) =>
{
    entityService = Common.GetSQLiteServerEntityService(sqlite_ConnectionString, db, app.Logger);
    var resultId = entityService.Create(db, entity, data);
    return ReturnAsSuccess(resultId);
});

#endregion

#region Generic Update
app.MapPut("mssql-api/{db}/{entity}/update-all", (string db, string entity, [FromBody] Parameters data) =>
{
entityService = Common.GetSQLServerEntityService(sql_connectionString, db, app.Logger);
var resultId = entityService.Update(db, entity, data);
return ReturnAsSuccess(resultId);
});
#endregion

#region Generic Update by Field
app.MapPut("mssql-api/{db}/{entity}/update-field", (string db, string entity, [FromHeader(Name = "Update-Fields")] string fields, [FromBody] Parameters data) =>
{
string[] fieldArray = fields.Split(',');
entityService = Common.GetSQLServerEntityService(sql_connectionString, db, app.Logger);
var resultId = entityService.Update(db, entity, fieldArray, data);
return ReturnAsSuccess(resultId, $"No of records Updated: {resultId}.");
});
#endregion

#region Generic Delete
//Ref: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding?view=aspnetcore-8.0
app.MapDelete("mssql-api/{db}/{entity}/delete/{id}", (string db, string entity, long id) =>
{
entityService = Common.GetSQLServerEntityService(sql_connectionString, db, app.Logger);
var resultId = entityService.Delete(db, entity, id);
return ReturnAsSuccess(resultId);
});
#endregion

#region Generic Select By Id
app.MapGet("mssql-api/{db}/{entity}/get/{id}", (string db, string entity, string id) =>
{
entityService = Common.GetSQLServerEntityService(sql_connectionString, db, app.Logger);
var results = entityService.GetById(db, entity, id);
app.Logger.LogInformation($"Selected Record:{results}");
return ReturnAsSuccess(results);
});
#endregion

#region Generic Select Single By Condition
app.MapPost("mssql-api/{db}/{entity}/get-one", (string db, string entity, [FromBody] Conditions conditions) =>
{
entityService = Common.GetSQLServerEntityService(sql_connectionString, db, app.Logger);
var results = entityService.GetSingle(db, entity, conditions);
app.Logger.LogInformation($"Selected Record:{results}");
return ReturnAsSuccess(results);
});
#endregion

#region Generic Select By Pagination
app.MapPost("mssql-api/{db}/{entity}/get-by-page", (string db, string entity, [FromBody] Conditions conditions) =>
{
    entityService = Common.GetSQLServerEntityService(sql_connectionString, db, app.Logger);
    var results = entityService.GetByPage(db, entity, conditions);
    app.Logger.LogInformation($"Selected Record: {results}");
    return ReturnAsSuccess(results);
});
#endregion

#region Generic Select By Pagination Complete Filter
app.MapPost("mssql-api/{db}/{entity}/get-by-filter", (string db, string entity, [FromBody] FilterParams filter) =>
{
    entityService = Common.GetSQLServerEntityService(sql_connectionString, db, app.Logger);
    var results = entityService.GetByPage(db, entity, filter);
    app.Logger.LogInformation($"Get by Filter Record: {results}");
    return ReturnAsSuccess(results);

});
#endregion

#region Generic Average
//Ref: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding?view=aspnetcore-8.0
app.MapPost("mssql-api/{db}/{table}/average/{field}", (string db, string table, string field, [FromBody] List<Filter> filters) =>
{
    entityService = Common.GetSQLServerEntityService(sql_connectionString, db, app.Logger);
    var resultId = entityService.Average(table, field, filters);
    return ReturnAsSuccess(resultId);
});
#endregion

app.Run();

IResult ReturnAsSuccess(dynamic? result, string message = "Executed Successfully")
{
    ResponseModel responseModel = new ResponseModel()
    {
        Data = result,
        Message = message,
        Success = true
    };
    //if(result is null)
    //{
    //    return Results.NoContent();
    //}
    return Results.Ok(responseModel);
}

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class Common
{
    public static IEntityService GetSQLServerEntityService(string conStr, string db, ILogger log)
    {
        string connString = string.Format(conStr, db);
        IEntityService entityService = new SQLEntityService(new SqlConnection(connString), log);
        return entityService;
    }
    public static IEntityService GetSQLiteServerEntityService(string conStr, string db, ILogger log)
    {
        string connString = string.Format(conStr, db);
        IEntityService entityService = new SQLEntityService(new SqliteConnection(connString), log);
        return entityService;
    }
}

public static class GlobalConfig
{
    //public static AWSConfig AWS { get; set; }
    public static JWTDetails JWTMeta { get; set; }
}



public class UserTemp
{
    public string UserName { get; set; }
    public string Password { get; set; }
}
