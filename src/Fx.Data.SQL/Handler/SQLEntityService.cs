using RepoDb;
using Microsoft.Data.SqlClient;
using Fx.Data.SQL.Interface;
using Microsoft.Extensions.Logging;
using Fx.Data.SQL.Helpers;
using Parameters = System.Collections.Generic.Dictionary<string, string>;
using RepoDb.Enumerations;
using System.Data;
using System.Diagnostics;

namespace Fx.Data.SQL.Handler;

//Ref: SQL Server Data Type Reference,
//https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings

/// <summary>
/// SQL Server Entity Service to handle CRUD Operations
/// </summary>
public partial class SQLEntityService : IEntityService
{
    #region Global Property
    public ILogger Logger { get; set; }
    //public string ConnectionString { get; set; }
    IDbConnection DbConnection { get; set; }
    #endregion

    #region Contructor    
    /// <summary>
    /// SQL Entity Service which initializes the SQLServer, MySQL, SQLite, PostgreSQL
    /// </summary>
    /// <param name="dbConnection">DB Connectivity</param>
    /// <param name="logger">Logger</param>
    public SQLEntityService(IDbConnection dbConnection, ILogger logger)
    {
        Logger = logger;
        DbConnection = dbConnection;
        GlobalConfiguration.Setup().UseSqlServer();
        GlobalConfiguration.Setup().UseSqlite();
        GlobalConfiguration.Setup().UseMySql();
        GlobalConfiguration.Setup().UsePostgreSql();
    }
    #endregion

    #region Exceptional Case - Execute Non Query    
    /// <summary>
    /// Executes any query
    /// </summary>
    /// <param name="sqlQuery">execute any query</param>
    /// <returns>returns the resulted from the query</returns>
    public dynamic? ExecuteNonQuery(string sqlQuery)
    {
        int affectedRows;
        using (var connection = DbConnection)
        {
            affectedRows = connection.ExecuteNonQuery(sqlQuery);
        }
        return affectedRows;
    }    
    #endregion
}