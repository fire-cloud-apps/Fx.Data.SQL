using Amazon;
using Fx.Security.Helper;
using Fx.Security.JWT;
using Microsoft.Extensions.Configuration;
using Sun.Delivery.API.Helpers;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public static class GlobalConfig
{
    //public static AWSConfig AWS { get; set; }
    public static JWTDetails JWTMeta { get; set; }
}