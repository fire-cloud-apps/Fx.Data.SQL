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
    public SQLEntityService(IDbConnection dbConnection, ILogger logger)
    {
        Logger = logger;
        DbConnection = dbConnection;
        GlobalConfiguration.Setup().UseSqlServer();
        GlobalConfiguration.Setup().UseSqlite();
    }
    #endregion

    #region Create
    /// <summary>
    /// Generic Insertion for SQL Server
    /// </summary>
    /// <param name="database">Database Name eg.'DMS', 'Master'</param>
    /// <param name="entity">Table Name eg.'test', 'user'</param>
    /// <param name="jsonData">
    /// eg.{
    ///  "C1": "Field 1",
    ///  "C2": "Field 2",
    ///  "Age": "31"
    ///}
    ///</param>
    /// <returns>Record 'id' of the inserted Object.</returns>
    public long Create(string database, string entity, Parameters jsonData)
    {
        var parameter = Conversions.ParameterConversion(jsonData);
        //Other Errors: If the data mismatch happens during conversion, it may throw an error.
        #region
        //Ref: https://repodb.net/operation/insert via via Anonymous Type.
        #endregion
        int id;
        using (var connection = DbConnection)
        {
            id = connection.Insert<int>(entity, parameter);
        }
        return id;
    }
    
    #endregion

    #region Update
    ///<inheritdoc cref="IEntityService.Update(string, string, Parameters)" />
    public dynamic? Update(string database, string entity, Parameters jsonData)
    {
        var parameter = Conversions.ParameterConversion(jsonData);
        //Other Errors: If the data mismatch happens during conversion, it may throw an error.
        #region
        //https://repodb.net/operation/update via Anonymous Type.
        #endregion
        int affectedRecords;
        using (var connection = DbConnection)
        {
            affectedRecords = connection.Update(
                tableName: entity, 
                entity: parameter,
                trace: TraceFactory.CreateTracer());
        }
        return affectedRecords;
    }
    #endregion

    #region Update Sepecific Field    
    ///<inheritdoc cref="IEntityService.Update(string, string, IList{string}, Parameters)" />
    public dynamic? Update(string database, string entity, IList<string> updateFields, Parameters jsonData)
    {
        var parameter = Conversions.ParameterConversion(jsonData);        
        int affectedRecords;
        using (var connection = DbConnection)
        {
            affectedRecords = connection.Update(
                tableName: entity,
                entity: parameter,
                fields: Field.From(updateFields.ToArray()),
                trace: TraceFactory.CreateTracer());
        }
        return affectedRecords;
    }
    #endregion

    #region Delete
    public dynamic? Delete(string database, string entity, long deleteId)
    {
        #region
        //https://repodb.net/operation/delete via Anonymous Type.
        #endregion
        int deletedRows;
        using (var connection = DbConnection)
        {
            deletedRows = connection.Delete(
                entity, deleteId,
                trace: TraceFactory.CreateTracer());
        }
        return deletedRows;
    }
    #endregion

    #region Get By Id
    public dynamic? GetById(string database, string entity, string id)
    {
        IEnumerable<dynamic> recordSet;
        using (var connection = DbConnection)
        {
            //Ref: https://repodb.net/class/parameter#queryfield
            recordSet = connection.Query(entity, new QueryField("Id", id), trace: TraceFactory.CreateTracer());
        }
        return recordSet.FirstOrDefault();
    }
    #endregion

    #region Get Single
    public dynamic? GetSingle(string database, string entity, Conditions conditions)
    {
        IEnumerable<dynamic> recordSet;
        using (var connection = DbConnection)
        {
            //Ref: https://repodb.net/reference/queryexpressions
            recordSet = connection.Query(entity, new QueryField(conditions.Field, Conversions.GetOperation(conditions.Condition), conditions.Value), trace: TraceFactory.CreateTracer());
        }
        return recordSet.FirstOrDefault();
    }
    #endregion

    #region Get Execute Query
    /// <returns></returns>
    public dynamic? ExecuteQuery(string database, string entity, Conditions conditions)
    {
        IEnumerable<dynamic> recordSet;
        using (var connection = DbConnection)
        {
            recordSet = connection.Query(entity, new QueryField(conditions.Field, Conversions.GetOperation(conditions.Condition), conditions.Value), trace: TraceFactory.CreateTracer());
        }
        return recordSet;
    }
    #endregion

    #region Get By Page with Simple Conditions
    public dynamic? GetByPage(string database, string entity, Conditions conditions)
    {
        IEnumerable<dynamic> recordSet;
        using (var connection = DbConnection)
        {
            //Ref:
            //1. https://repodb.net/operation/batchquery
            //2. https://repodb.net/feature/targeted

            #region Order By
            var orderBy = new[]
            {
                new OrderField("Date", Order.Descending)
            };
            #endregion

            #region Where
            var where = new QueryField(conditions.Field, Conversions.GetOperation(conditions.Condition), conditions.Value);
            #endregion

            #region Execution
            recordSet = connection.BatchQuery
                 (
                 tableName: entity,
                 where: where,
                 trace: TraceFactory.CreateTracer(),
                 page: conditions.Page,
                 rowsPerBatch: conditions.RowsPerBatch,
                 orderBy: orderBy
                 );
            #endregion
        }

        return recordSet;
    }
    #endregion

    #region Get By Page with Complex Conditions - FilterParams
    public dynamic? BatchQuery(string database, string entity, FilterParams filters)
    {
        return GetByPage(database, entity, filters);
    }
    public dynamic? GetByPage(string database, string entity, FilterParams filters)
    {
        IEnumerable<object> recordSet;
        using (var connection = DbConnection)
        {
            //Ref:
            //1. https://repodb.net/operation/batchquery
            //2. https://repodb.net/feature/targeted

            #region Sorting
            IList<OrderField> orderBy = Utilities.GetOrderBy(filters.Sort);
            #endregion

            #region Targeted Fields
            IEnumerable<Field>? fields = filters.Fields is null ? null : Field.From(filters.Fields.ToArray());
            #endregion

            #region Where 
            QueryGroup where = Utilities.GetQueryGroup(filters.Filter);
            #endregion

            #region Execution Layer
            recordSet = connection.BatchQuery
                 (
                 tableName: entity,
                 fields: fields,
                 where: where,
                 trace: TraceFactory.CreateTracer(),
                 page: filters.Page,
                 rowsPerBatch: filters.RowsPerBatch,
                 orderBy: orderBy
                 );
            #endregion
        }

        return recordSet;
    }
    #endregion

    #region Exceptional Case - Execute Non Query    
    public dynamic? ExecuteNonQuery(string database, string sqlQuery)
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



