using Fx.Data.SQL.Helpers;
using RepoDb.Enumerations;
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
    #region Get Methods

    #region Get By UniqueId
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <param name="table"> <inheritdoc /></param>
    /// <param name="field"> <inheritdoc /></param>
    /// <param name="value"> <inheritdoc /></param>
    /// <returns> <inheritdoc /></returns>
    public dynamic? GetByUniqueId(string table, string field, string value)
    {
        IEnumerable<dynamic> recordSet;
        using (var connection = DbConnection)
        {
            //Ref: https://repodb.net/class/parameter#queryfield
            recordSet = connection.Query(table, new QueryField(field, value), 
                trace: TraceFactory.CreateTracer());
        }
        return recordSet.FirstOrDefault();
    }
    #endregion

    #region Get By Id
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <param name="entity"> <inheritdoc /></param>
    /// <param name="id"> <inheritdoc /></param>
    /// <returns> <inheritdoc /></returns>
    public dynamic? GetById(string entity, string id)
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
    public dynamic? GetSingle(string entity, Conditions conditions)
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
    public dynamic? ExecuteQuery(string entity, Conditions conditions)
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
    public dynamic? GetByPage(string entity, Conditions conditions)
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
                new OrderField(conditions.Field, Order.Descending)
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
    public dynamic? BatchQuery(string entity, FilterParams filters)
    {
        return GetByPage(entity, filters);
    }
    public dynamic? GetByPage(string entity, FilterParams filters)
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

    #endregion
}

