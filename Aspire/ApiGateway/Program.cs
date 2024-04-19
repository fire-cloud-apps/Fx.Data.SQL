using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Fx.Security.JWT;
using ApiGateway.Aggregations;

var builder = WebApplication.CreateBuilder(args);

//Configuration
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<CommonAggregator>();
builder.Services.AddOcelot(builder.Configuration)
    .AddSingletonDefinedAggregator<CommonAggregator>();
    ; // Ocelot Service Initialized.




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
#endregion

builder.Services.AddCustomJwtAuthentication(jwtMeta);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
await app.UseOcelot();//Initializes the Ocelot

app.UseAuthentication();
app.UseAuthentication();

//Ref: https://arbems.com/en/building-api-gateway-on-net-with-ocelot/
// https://www.youtube.com/watch?v=k4l3Ptd4yjw&t=15s

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
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

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
