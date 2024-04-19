using Fx.Data.SQL.Interface;
using Fx.Data.SQLAPIs.AttributesFilter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parameters = System.Collections.Generic.Dictionary<string, string>;

namespace Fx.Data.SQLAPIs.EndPoints.SQLServer;
public static class DMLEndPoints
{
    public static void Map(WebApplication app,  string sql_connectionString)
    {
        IEntityService entityService = null;
        #region Generic Insertion

        app.MapPost("mssql-api/{db}/{entity}/create",(string db, string entity, [FromBody] Parameters data) =>
        {
            entityService = Common.GetSQLServerEntityService(sql_connectionString, db, app.Logger);
            var resultId = entityService.Create(entity, data);
            return Common.ReturnAsSuccess(resultId);
        });//.RequireAuthorization();
        #endregion

        #region Generic Update
        app.MapPut("mssql-api/{db}/{entity}/update-all", (string db, string entity, [FromBody] Parameters data) =>
        {
            entityService = Common.GetSQLServerEntityService(sql_connectionString, db, app.Logger);
            var resultId = entityService.Update(entity, data);
            return Common.ReturnAsSuccess(resultId);
        });
        #endregion

        #region Generic Update by Field
        app.MapPut("mssql-api/{db}/{entity}/update-field", (string db, string entity, [FromHeader(Name = "Update-Fields")] string fields, [FromBody] Parameters data) =>
        {
            string[] fieldArray = fields.Split(',');
            entityService = Common.GetSQLServerEntityService(sql_connectionString, db, app.Logger);
            var resultId = entityService.Update(entity, fieldArray, data);
            return Common.ReturnAsSuccess(resultId, $"No of records Updated: {resultId}.");
        });
        #endregion

        #region Generic Delete
        //Ref: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding?view=aspnetcore-8.0
        app.MapDelete("mssql-api/{db}/{entity}/delete/{id}", (string db, string entity, long id) =>
        {
            entityService = Common.GetSQLServerEntityService(sql_connectionString, db, app.Logger);
            var resultId = entityService.Delete(entity, id);
            return Common.ReturnAsSuccess(resultId);
        });
        #endregion
    }
}

