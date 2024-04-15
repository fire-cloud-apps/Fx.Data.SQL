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
                field:  field,
                where: where,
                trace: TraceFactory.CreateTracer()
                );
        }
        return averageValue;
    }
    #endregion

    #region Execute Count
    public dynamic? Count(string database, string entity, List<Filter> filter)
    {
        long recordCount;
        using (var connection = DbConnection)
        {
            #region Where 
            QueryGroup where = Utilities.GetQueryGroup(filter);
            #endregion
            recordCount = connection.Count(entity, where: where, trace: TraceFactory.CreateTracer());
        }
        return recordCount;
    }
    #endregion

    #region Execute Exists
    public dynamic? Exists(string database, string entity, List<Filter> filter)
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
}
