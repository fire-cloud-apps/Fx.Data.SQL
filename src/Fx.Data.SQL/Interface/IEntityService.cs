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

/// <summary>
/// Entity Service Interface to implement in RDBMS
///</summary>
public interface IEntityService
{
    #region Get & Filter Methods
    /// <summary>
    /// Get the data by unique 'Id'. The field should be in the name of 'id' or 'Id'.
    /// </summary>
    /// <param name="table">Table name, which exists in the database.</param>
    /// <param name="id">unique id's value</param>
    /// <returns>returns the table data</returns>
    dynamic? GetById(string table, string id);

    /// <summary>
    /// Get the record by using filter condition
    /// </summary>
    /// <param name="table">On what table, do this query to be executed.</param>
    /// <param name="conditions">Filter condition to get the data from table</param>
    /// <returns>returns the table data</returns>
    dynamic? GetSingle(string table, Conditions conditions);

    /// <summary>
    /// Get the record by using filter condition
    /// </summary>
    /// <param name="table">On what table, do this query to be executed.</param>
    /// <param name="conditions">Filter condition to get the data from table</param>
    /// <returns>returns the table data</returns>
    dynamic? GetByPage(string table, Conditions conditions);

    /// <summary>
    /// Get the record by using filter condition
    /// </summary>
    /// <param name="table">On what table, do this query to be executed.</param>
    /// <param name="filters">Filter condition to get the data from table</param>
    /// <returns>returns the table data</returns>
    dynamic? GetByPage(string table, FilterParams filters);

    #endregion

    #region Update Methods
    /// <summary>
    /// Used to Insert data into the database
    /// </summary>
    /// <param name="table">On what table, do this query to be executed.</param>
    /// <param name="jsonData">Data as a JsonData</param>
    /// <returns>returns the inserted data.</returns>
    long Create(string table, Parameters jsonData);
    /// <summary>
    /// This method is used to updates an existing row in the table.
    /// This accept Anonymous Type, so you can pass any fields with right db & table name.
    /// </summary>
    /// <param name="table">On what table, do this query to be executed.</param>
    /// <param name="jsonData">Fields and Value to be updated send as JSON</param>
    /// <exception cref="Other Errors: If the data mismatch happens during conversion, it may throw an error." />
    /// <remarks>Reference: https://repodb.net/operation/update</remarks>
    /// <returns>No of records updated</returns>
    dynamic? Update(string entity, Parameters jsonData);
    /// <summary>
    /// This method is used to updates an existing row in the table.
    /// This accept Anonymous Type, so you can pass any fields with right db & table name.
    /// </summary>
    /// <param name="database">database name</param>
    /// <param name="table">table name</param>
    /// <param name="updateFields">Fields to be updated as string array</param>
    /// <param name="jsonData">Fields and Value to be updated send as JSON</param>
    /// <exception cref="Other Errors: If the data mismatch happens during conversion, it may throw an error." />
    /// <remarks>Reference: https://repodb.net/operation/update</remarks>
    /// <returns>No of records updated</returns>
    dynamic? Update(string table, IList<string> updateFields, Parameters jsonData);
    /// <summary>
    /// Delete the data based on the unique condition
    /// </summary>
    /// <param name="table">On what table, do this query to be executed.</param>
    /// <param name="deleteId">unique Id, on which the records to be deleted.</param>
    /// <returns>Delete the record id.</returns>
    dynamic? Delete(string table, long deleteId);
    #endregion

    #region Exceptional Case
    /// <summary>
    /// Used to execute a raw-SQL directly towards the database. It returns the number of rows affected during the execution. This method supports all types of RDMBS data providers.
    /// </summary>
    /// <param name="sqlQuery">The raw query statement</param>
    /// <returns>returns the table data</returns>
    dynamic? ExecuteNonQuery(string sqlQuery);
    /// <summary>
    /// Used to truncates a table from the database.
    /// </summary>
    /// <param name="table">On which table this to be executed.</param>
    /// <returns>return no of rows deleted.</returns>
    dynamic? Truncate(string table);
    #endregion

    #region RepoDB Operations
    /// <summary>
    /// Used to compute the average value of the target field.
    /// </summary>
    /// <param name="table">On what table, do this query to be executed.</param>
    /// <param name="fields">On what field, this should be executed.</param>
    /// <param name="filter">Conditional Filter as List.</param>
    /// <returns>returns Max value</returns>
    dynamic? Average(string table, string fields, List<Filter> filter);

    /// <summary>
    /// Used to count the number of rows from the table.
    /// </summary>
    /// <param name="table">On what table, do this query to be executed.</param>
    /// <param name="filter">Conditional Filter as List.</param>
    /// <returns>returns No of Records</returns>
    dynamic? Count(string table, List<Filter> filter);
    /// <summary>
    /// Used to query the rows from the database by batch.
    /// </summary>
    /// <param name="table">On what table, do this query to be executed.</param>
    /// <param name="filter">Conditional Filter as List.</param>
    /// <returns>returns a completed filtered data</returns>
    dynamic? BatchQuery(string table, FilterParams filter);

    /// <summary>
    /// Used to execute a raw-SQL directly towards the database.This method supports all types of RDMBS data providers.
    /// </summary>
    /// <param name="table">On what table, do this query to be executed.</param>
    /// <param name="conditions">Conditional Filters </param>
    /// <returns>returns a completed filtered data</returns>
    dynamic? ExecuteQuery(string table, Conditions conditions);
    /// <summary>
    /// Finds the Exists value based on the conditions or filters.
    /// </summary>
    /// <param name="table">On what table, do this query to be executed.</param>
    /// <param name="filters">Conditional Filter as List.</param>
    /// <returns>returns Bool value</returns>
    dynamic? Exists(string table, List<Filter> filters);
    /// <summary>
    /// Finds the Sum of value based on the conditions or filters.
    /// </summary>
    /// <param name="table">On what table, do this query to be executed.</param>
    /// <param name="fields">On what field, this should be executed.</param>
    /// <param name="filter">Conditional Filter as List.</param>
    /// <returns>returns Sum value</returns>
    dynamic? Sum(string table, string fields, List<Filter> filter);

    /// <summary>
    /// Finds the Min value based on the conditions or filters.
    /// </summary>
    /// <param name="table">On what table, do this query to be executed.</param>
    /// <param name="fields">On what field, this should be executed.</param>
    /// <param name="filter">Conditional Filter as List.</param>
    /// <returns>returns Min value</returns>
    dynamic? Min(string table, string fields, List<Filter> filter);
    /// <summary>
    /// Finds the Maximum value based on the conditions or filters.
    /// </summary>
    /// <param name="table">On what table, do this query to be executed.</param>
    /// <param name="fields">On what field, this should be executed.</param>
    /// <param name="filter">Conditional Filter as List.</param>
    /// <returns>returns Max value</returns>
    dynamic? Max(string table, string fields, List<Filter> filter);
    #endregion
}
