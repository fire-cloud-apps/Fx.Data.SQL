
using Fx.Data.SQL.Helpers;
using Fx.Data.SQL.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Fx.Data.SQLAPIs.EndPoints.SQLServer;
public static class OperationEndPoints
{
    public static void Map(WebApplication app, string sql_connectionString)
    {
        IEntityService entityService = null;

        #region Generic Average
        //Ref: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding?view=aspnetcore-8.0
        app.MapPost("mssql-api/{db}/{table}/average/{field}", (string db, string table, string field, [FromBody] List<Filter> filters) =>
        {
            entityService = Common.GetSQLServerEntityService(sql_connectionString, db, app.Logger);
            var resultId = entityService.Average(table, field, filters);
            return Common.ReturnAsSuccess(resultId);
        });
        #endregion
    }
}

