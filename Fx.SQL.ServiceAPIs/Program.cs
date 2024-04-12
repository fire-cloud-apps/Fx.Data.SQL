using Fx.Data.SQL.Handler;
using Fx.Data.SQL.Helpers;
using Fx.Data.SQL.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
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
app.UseExceptionHandler(exceptionHandlerApp =>
{
    //Ref: https://learn.microsoft.com/en-us/answers/questions/697739/error-handling-in-net6-minimal-api
    exceptionHandlerApp.Run(async httpContext =>
    {
        var exceptionHandlerPathFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
        if(exceptionHandlerPathFeature is not null)
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

#region SQL Server DB Settings
string connectionString = "Server=localhost;Database={0};User Id=sa;Password=System@1984;TrustServerCertificate=True;";//DMS
IEntityService entityService = new SQLServerEntityService(connectionString, app.Logger);
#endregion

#region Generic Insertion
app.MapPost("mssql-api/{db}/{entity}/create", (string db, string entity, [FromBody] Parameters data) =>
{
    var resultId = entityService.Create(db, entity, data);
    return ReturnAsSuccess(resultId);
    
});
#endregion


#region Generic Update
app.MapPut("mssql-api/{db}/{entity}/update", (string db, string entity, [FromBody] Parameters data) =>
{
    var resultId = entityService.Update(db, entity, data);
    return ReturnAsSuccess(resultId);
});
#endregion

#region Generic Delete
//Ref: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding?view=aspnetcore-8.0
app.MapDelete("mssql-api/{db}/{entity}/delete/{id}", (string db, string entity, long id) =>
{
    var resultId = entityService.Delete(db, entity, id);
    return ReturnAsSuccess(resultId);
});
#endregion

#region Generic Select By Id
app.MapGet("mssql-api/{db}/{entity}/get/{id}", (string db, string entity, string id) =>
{
    var results = entityService.GetById(db, entity, id);
    app.Logger.LogInformation($"Selected Record:{results}");
    return ReturnAsSuccess(results);
});
#endregion

#region Generic Select Single By Condition
app.MapPost("mssql-api/{db}/{entity}/get-one", (string db, string entity, [FromBody] Conditions conditions) =>
{
    var results = entityService.GetSingle(db, entity, conditions);
    app.Logger.LogInformation($"Selected Record:{results}");
    return ReturnAsSuccess(results);
});
#endregion

#region Generic Select By Pagination
app.MapPost("mssql-api/{db}/{entity}/get-by-page", (string db, string entity, [FromBody] Conditions conditions) =>
{
    var results = entityService.GetByPage(db, entity, conditions);
    app.Logger.LogInformation($"Selected Record: {results}");
    return ReturnAsSuccess(results);
});
#endregion

#region Generic Select By Pagination Complete Filter
app.MapPost("mssql-api/{db}/{entity}/get-by-filter", (string db, string entity, [FromBody] FilterParams filter) =>
{
    var results = entityService.GetByPage(db, entity, filter);
    app.Logger.LogInformation($"Get by Filter Record: {results}");
    return ReturnAsSuccess(results);
    
});
#endregion

app.Run();

IResult ReturnAsSuccess(dynamic? result)
{   
    ResponseModel responseModel = new ResponseModel()
    {
        Data = result,
        Message = "Executed Successfully",
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
