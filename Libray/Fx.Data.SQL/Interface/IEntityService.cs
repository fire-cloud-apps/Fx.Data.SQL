using Fx.Data.SQL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parameters = System.Collections.Generic.Dictionary<string, string>;
using Record = System.Collections.Generic.IDictionary<string, object>;
using Records = System.Collections.Generic.List<System.Collections.Generic.IDictionary<string, object>>;

namespace Fx.Data.SQL.Interface;

// <summary>
/// Entity Service Interface to implement in RDBMS
/// </summary>
public interface IEntityService
{
    #region Get & Filter Methods
    dynamic? GetById(string database, string entity, string id);
    dynamic? GetSingle(string database, string entity, Conditions conditionColumns);
    dynamic? GetByPage(string database, string entity, Conditions conditionColumns);
    dynamic? GetByPage(string database, string entity, FilterParams filters);
    #endregion

    #region Update Methods
    long Create(string database, string entity, Parameters jsonData);
    /// <summary>
    /// This method is used to updates an existing row in the table.
    /// This accept Anonymous Type, so you can pass any fields with right db & table name.
    /// </summary>
    /// <param name="database">database name</param>
    /// <param name="entity">table name</param>
    /// <param name="jsonData">Fields and Value to be updated send as JSON</param>
    /// <exception cref="Other Errors: If the data mismatch happens during conversion, it may throw an error." />
    /// <remarks>Reference: https://repodb.net/operation/update</remarks>
    /// <returns>No of records updated</returns>
    dynamic? Update(string database, string entity, Parameters jsonData);
    /// <summary>
    /// This method is used to updates an existing row in the table.
    /// This accept Anonymous Type, so you can pass any fields with right db & table name.
    /// </summary>
    /// <param name="database">database name</param>
    /// <param name="entity">table name</param>
    /// <param name="updateFields">Fields to be updated as string array</param>
    /// <param name="jsonData">Fields and Value to be updated send as JSON</param>
    /// <exception cref="Other Errors: If the data mismatch happens during conversion, it may throw an error." />
    /// <remarks>Reference: https://repodb.net/operation/update</remarks>
    /// <returns>No of records updated</returns>
    dynamic? Update(string database, string entity, IList<string> updateFields, Parameters jsonData);
    dynamic? Delete(string database, string entity, long deleteId);
    #endregion

    #region Exceptional Case
    dynamic? ExecuteNonQuery(string database, string sqlQuery);
    #endregion

    #region RepoDB Operations
    dynamic? Average(string table, string fieldToAverage, List<Filter> filter);
    dynamic? Count(string database, string entity, List<Filter> filter);
    dynamic? BatchQuery(string database, string entity, FilterParams filters);
    dynamic? ExecuteQuery(string database, string entity, Conditions conditions);
    dynamic? Exists(string database, string entity, List<Filter> filters);
    #endregion
}






