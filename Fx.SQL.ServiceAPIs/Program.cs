using Fx.Data.SQL.Handler;
using Fx.Data.SQL.Helpers;
using Fx.Data.SQL.Interface;
using Microsoft.AspNetCore.Mvc;
using Parameters = System.Collections.Generic.Dictionary<string, string>;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.Logger.LogInformation("Minimal API Started");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region Exception Handling
//app.UseExceptionHandler(exceptionHandlerApp =>
//{
//    exceptionHandlerApp.Run(async httpContext =>
//    {
//        //var exceptionHandlerPathFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
//        //var exception = exceptionHandlerPathFeature.Error; for custom exceptions and filters        

//        var pds = httpContext.RequestServices.GetService<IProblemDetailsService>();
//        if (pds == null
//            || !await pds.TryWriteAsync(new() { HttpContext = httpContext }))
//        {
//            // Fallback behavior
//            await httpContext.Response.WriteAsync("Fallback: An error occurred.");
//        }
//    });
//});

//app.MapGet("/exception", () =>
//{
//    throw new InvalidOperationException("Sample Exception");
//});
#endregion

#region Demo API
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
#endregion

string connectionString = "Server=localhost;Database={0};User Id=sa;Password=System@1984;TrustServerCertificate=True;";//DMS
IEntityService entityService = new SQLServerEntityService(connectionString, app.Logger);

#region Generic Insertion
app.MapPost("mssql-api/{db}/{entity}/create", (string db, string entity, [FromBody] Parameters data) =>
{
    var resultId = entityService.Create(db, entity, data);
    return Results.Ok($"Created Record Id:{resultId}. On DB: {db}, Table: {entity}, Data: {Conversions.DictionaryToJson(data)}");
});
#endregion

#region Generic Select Single By Condition
app.MapPost("mssql-api/{db}/{entity}/get-one", (string db, string entity, [FromBody] Conditions conditions) =>
{
    var results = entityService.GetSingle(db, entity, conditions);
    app.Logger.LogInformation($"Selected Record:{results}");
    return Results.Ok(results);
});
#endregion

#region Generic Select By Pagination
app.MapPost("mssql-api/{db}/{entity}/get-by-page", (string db, string entity, [FromBody] Conditions conditions) =>
{
    var results = entityService.GetByPage(db, entity, conditions);
    app.Logger.LogInformation($"Selected Record: {results}");
    return Results.Ok(results);
});
#endregion

#region Generic Select By Pagination Complete Filter
app.MapPost("mssql-api/{db}/{entity}/get-by-filter", (string db, string entity, [FromBody] FilterParams filter) =>
{
    var results = entityService.GetByPage(db, entity, filter);
    app.Logger.LogInformation($"Get by Filter Record: {results}");
    return Results.Ok(results);
});
#endregion

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
