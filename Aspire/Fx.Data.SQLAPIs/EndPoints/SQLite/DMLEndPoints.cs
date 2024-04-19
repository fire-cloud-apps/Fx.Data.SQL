namespace Fx.Data.SQLAPIs.EndPoints.SQLite;

using global::Fx.Data.SQL.Interface;
using Microsoft.AspNetCore.Mvc;
using Parameters = System.Collections.Generic.Dictionary<string, string>;

public static class DMLEndPoints
{
    public static void Map(WebApplication app, string sql_connectionString)
    {
        IEntityService entityService = null;

        #region Generic Insertion
        //SQLite
        app.MapPost("sqlite-api/{db}/{entity}/create", (string db, string entity, [FromBody] Parameters data) =>
        {
            entityService = Common.GetSQLiteServerEntityService(sql_connectionString, db, app.Logger);
            var resultId = entityService.Create(entity, data);
            return Common.ReturnAsSuccess(resultId);
        });
        #endregion


    }
}



