using Fx.Data.SQL.Helpers;
using Microsoft.Extensions.Logging;
using RepoDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parameters = System.Collections.Generic.Dictionary<string, string>;

namespace Fx.Data.SQL.Handler;
public partial class SQLEntityService
{
    #region Execute Average    
    public dynamic Average(string table, string fieldToAverage, List<Filter> filter)
    {
        dynamic averageValue;
        using (var connection = DbConnection)
        {
            #region Where 
            QueryGroup where = Utilities.GetQueryGroup(filter);
            var field = Field.From(fieldToAverage).First();
            #endregion
            averageValue = connection.Average (
                tableName: table, 
                field: field,
                where: where,
                trace: TraceFactory.CreateTracer()
                );
        }
        return averageValue;
    }
    #endregion

    #region Execute Count
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="table"><inheritdoc/> </param>
    /// <param name="filter"> <inheritdoc/> </param>
    /// <returns> <inheritdoc/> </returns>
    public dynamic? Count(string table, List<Filter> filter)
    {
        long recordCount;
        using (var connection = DbConnection)
        {
            #region Where 
            QueryGroup where = Utilities.GetQueryGroup(filter);
            #endregion
            recordCount = connection.Count(table, where: where, trace: TraceFactory.CreateTracer());
        }
        return recordCount;
    }
    #endregion

    #region Execute Exists
    public dynamic? Exists(string entity, List<Filter> filter)
    {
        dynamic resultSet;
        using (var connection = DbConnection)
        {
            #region Where 
            QueryGroup where = Utilities.GetQueryGroup(filter);
            #endregion
            resultSet = connection.Exists(entity, where);
        }
        return resultSet;
    }
    #endregion

    #region Execute Sum
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="entity"><inheritdoc/> </param>
    /// <param name="fieldToSum"><inheritdoc/> </param>
    /// <param name="filter"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public dynamic? Sum(string entity, string fieldToSum, List<Filter> filter)
    {
        dynamic resultSet;
        using (var connection = DbConnection)
        {
            #region Where & Field
            var field = Field.From(fieldToSum).First();
            QueryGroup where = Utilities.GetQueryGroup(filter);
            #endregion
            resultSet = connection.Sum(entity, field, where);
        }
        return resultSet;
    }
    #endregion

    #region Execute Max
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="entity"><inheritdoc/> </param>
    /// <param name="fields"><inheritdoc/> </param>
    /// <param name="filter"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public dynamic? Max(string entity, string fields, List<Filter> filter)
    {
        dynamic resultSet;
        using (var connection = DbConnection)
        {
            #region Where & Field
            var field = Field.From(fields).First();
            QueryGroup where = Utilities.GetQueryGroup(filter);
            #endregion
            resultSet = connection.Max(entity, field, where);
        }
        return resultSet;
    }
    #endregion

    #region Execute Min
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="entity"><inheritdoc/> </param>
    /// <param name="fields"><inheritdoc/> </param>
    /// <param name="filter"> <inheritdoc/> </param>
    /// <returns> <inheritdoc/> </returns>
    public dynamic? Min(string entity, string fields, List<Filter> filter)
    {
        dynamic resultSet;
        using (var connection = DbConnection)
        {
            #region Where & Field
            var field = Field.From(fields).First();
            QueryGroup where = Utilities.GetQueryGroup(filter);
            #endregion
            resultSet = connection.Min(entity, field, where);
        }
        return resultSet;
    }
    #endregion

}
