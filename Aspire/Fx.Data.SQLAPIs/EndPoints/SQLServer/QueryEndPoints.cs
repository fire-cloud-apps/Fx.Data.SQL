using Fx.Data.SQL.Helpers;
using Fx.Data.SQL.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Fx.Data.SQLAPIs.EndPoints.SQLServer;
public static class QueryEndPoints
{
    public static void Map(WebApplication app, string sql_connectionString)
    {
        IEntityService entityService = null;

        #region Generic Select By Id
        app.MapGet("mssql-api/{db}/{entity}/get/{id}", (string db, string entity, string id) =>
        {
            entityService = Common.GetSQLServerEntityService(sql_connectionString, db, app.Logger);
            var results = entityService.GetById(entity, id);
            app.Logger.LogInformation($"Selected Record:{results}");
            return Common.ReturnAsSuccess(results);
        });
        #endregion

        #region Generic Select Single By Condition
        app.MapPost("mssql-api/{db}/{entity}/get-one", (string db, string entity, [FromBody] Conditions conditions) =>
        {
            entityService = Common.GetSQLServerEntityService(sql_connectionString, db, app.Logger);
            var results = entityService.GetSingle(entity, conditions);
            app.Logger.LogInformation($"Selected Record:{results}");
            return Common.ReturnAsSuccess(results);
        });
        #endregion

        #region Generic Select By Pagination
        app.MapPost("mssql-api/{db}/{entity}/get-by-page", (string db, string entity, [FromBody] Conditions conditions) =>
        {
            entityService = Common.GetSQLServerEntityService(sql_connectionString, db, app.Logger);
            var results = entityService.GetByPage(entity, conditions);
            app.Logger.LogInformation($"Selected Record: {results}");
            return Common.ReturnAsSuccess(results);
        });
        #endregion

        #region Generic Select By Pagination Complete Filter
        app.MapPost("mssql-api/{db}/{entity}/get-by-filter", (string db, string entity, [FromBody] FilterParams filter) =>
        {
            entityService = Common.GetSQLServerEntityService(sql_connectionString, db, app.Logger);
            var results = entityService.GetByPage(entity, filter);
            app.Logger.LogInformation($"Get by Filter Record: {results}");
            return Common.ReturnAsSuccess(results);

        });
        #endregion
    }
}

