using Fx.Data.SQL.Handler;
using Fx.Data.SQL.Helpers;
using Fx.Data.SQL.Interface;
using Fx.Security.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SQLServer = Fx.Data.SQLAPIs.EndPoints.SQLServer;
using SQLite = Fx.Data.SQLAPIs.EndPoints.SQLite;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting.Server;
using System.Net;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Configuration
//var jwtMeta = new JWTDetails()
//{
//    Issuer = builder.Configuration.GetValue<string>("JWT:Issuer"),
//    Audiance = builder.Configuration.GetValue<string>("JWT:Issuer"),
//    Subject = builder.Configuration.GetValue<string>("JWT:Subject"),
//    PrivateKey = builder.Configuration.GetValue<string>("JWT:PrivateKey"),
//    PublicKey = builder.Configuration.GetValue<string>("JWT:PublicKey"),
//    Expiry = DateTime.UtcNow.AddHours(int.Parse(builder.Configuration.GetValue<string>("JWT:Expiry")))
//};
//GlobalConfig.JWTMeta = jwtMeta;
#endregion

#region Authentication Middleware

//builder.Services.AddCustomJwtAuthentication(jwtMeta);//Comes from Fx.Security.

/*
RSA _rsa;
_rsa = RSA.Create();
_rsa.ImportRSAPublicKey(Convert.FromBase64String(jwtMeta.PublicKey), out _);
_rsa.ImportRSAPrivateKey(Convert.FromBase64String(jwtMeta.PrivateKey), out _);
var jwtToken = new JwtTokenHandlerRSA(GlobalConfig.JWTMeta.PublicKey, GlobalConfig.JWTMeta.PrivateKey);
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
*/

//builder.Services.AddAuthorization();
#endregion

var app = builder.Build();

#region Find the runing Address
// Get server features
var server = app.Services.GetService<IServer>();
var addressFeature = server?.Features.Get<IServerAddressesFeature>();

// Iterate and log listening addresses
if (addressFeature?.Addresses.Any() == true)
{
    foreach (var address in addressFeature.Addresses)
    {
        Console.WriteLine($"Listening on: {address}");
    }
}
else
{
    Console.WriteLine("Unable to determine listening addresses using IServerAddressesFeature.");
}


#endregion

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}


//Enabled for Production as well.
app.UseSwagger();
app.UseSwaggerUI();

#region Enables Authentication & Authorization
//app.UseAuthentication();
//app.UseAuthorization();
#endregion

app.UseHttpsRedirection();

#region Handling - Test API's
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


//app.MapGet("/exception", () =>
//{
//throw new InvalidOperationException("Sample Exception");
//});

//https://betterstack.com/community/questions/how-to-view-contents-of-docker-images/

//app.MapGet("/hello-world", () => "Hello World!").RequireAuthorization();
//app.MapGet("/hello-no-auth", () => "Invoked without Token!").AllowAnonymous();
//app.MapGet("/hello-no-authx", () => "Invoked without Token!").AllowAnonymous().WithOpenApi();
#endregion

app.MapGet("/hello", () =>
{
    return Results.Ok("I am listening.");
});

#region DB Settings
string sql_connectionString = "Server=localhost;Database={0};User Id=sa;Password=System@1984;TrustServerCertificate=True;";//DMS
string sqlite_ConnectionString = "Data Source=C:\\Works\\Github\\Fx.Data.SQL\\Fx.SQL.ServiceAPIs\\Db\\{0};";
IEntityService entityService;
#endregion

//SQL Server End Points
SQLServer.DMLEndPoints.Map(app, sql_connectionString);
SQLServer.QueryEndPoints.Map(app, sql_connectionString);
SQLServer.OperationEndPoints.Map(app, sql_connectionString);

//SQLite End Points
SQLite.DMLEndPoints.Map(app, sqlite_ConnectionString);


app.Run();


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

    public static IResult ReturnAsSuccess(dynamic? result, string message = "Executed Successfully")
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
