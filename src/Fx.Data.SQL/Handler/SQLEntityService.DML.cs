using Fx.Data.SQL.Helpers;
using RepoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parameters = System.Collections.Generic.Dictionary<string, string>;


namespace Fx.Data.SQL.Handler;

public partial class SQLEntityService
{

    #region DML Methods

    #region Create
    /// <summary>
    /// Generic Insertion for SQL Server
    /// </summary>
    /// <param name="entity">Table Name eg.'test', 'user'</param>
    /// <param name="jsonData">
    /// eg.{
    ///  "C1": "Field 1",
    ///  "C2": "Field 2",
    ///  "Age": "31"
    ///}
    ///</param>
    /// <returns>Record 'id' of the inserted Object.</returns>
    public long Create(string entity, Parameters jsonData)
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
    public dynamic? Update(string entity, Parameters jsonData)
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
    public dynamic? Update(string entity, IList<string> updateFields, Parameters jsonData)
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
    public dynamic? Delete(string entity, long deleteId)
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

    #endregion
}

