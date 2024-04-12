using Fx.Data.SQL.Helpers;
using Fx.Data.SQL.Interface;
using Microsoft.Data.SqlClient;
using RepoDb.Enumerations;
using RepoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Data.SQL.Handler;


using RepoDb;
using System.Collections.Immutable;
using Microsoft.Data.SqlClient;
using Fx.Data.SQL.Interface;
using Microsoft.Extensions.Logging;
using Fx.Data.SQL.Helpers;
using Parameters = System.Collections.Generic.Dictionary<string, string>;
using Record = System.Collections.Generic.IDictionary<string, object>;
using Records = System.Collections.Generic.List<System.Collections.Generic.IDictionary<string, object>>;
using Operation = RepoDb.Enumerations.Operation;
using RepoDb.Enumerations;



//Ref: SQL Server Data Type Reference, https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings

public class SQLServerEntityService : IEntityService
{
    #region Global Property
    public ILogger Logger { get; set; }
    public string ConnectionString { get; set; }
    #endregion

    #region Contructor
    public SQLServerEntityService(string connectionString, ILogger logger)
    {
        Logger = logger;
        ConnectionString = connectionString;
        GlobalConfiguration.Setup().UseSqlServer();
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

        #region
        //Ref: https://repodb.net/operation/insert via via Anonymous Type.        
        #endregion
        int id;
        string connString = string.Format(ConnectionString, database);
        using (var connection = new SqlConnection(connString))
        {
            id = connection.Insert<int>(entity, parameter);
        }
        return id;
    }
    #endregion

    public dynamic? GetSingle(string database, string entity, Conditions conditions)
    {
        IEnumerable<dynamic> recordSet;
        string connString = string.Format(ConnectionString, database);
        using (var connection = new SqlConnection(connString))
        {
            //Ref: https://repodb.net/reference/queryexpressions
            recordSet = connection.Query(entity, new QueryField(conditions.Field, Conversions.GetOperation(conditions.Condition), conditions.Value), trace: TraceFactory.CreateTracer());
        }
        return recordSet.FirstOrDefault();
    }

    public dynamic? GetByPage(string database, string entity, Conditions conditions)
    {
        IEnumerable<dynamic> recordSet;
        string connString = string.Format(ConnectionString, database);
        using (var connection = new SqlConnection(connString))
        {
            //Ref:
            //1. https://repodb.net/operation/batchquery
            //2. https://repodb.net/feature/targeted

            var orderBy = new[]
            {
                new OrderField("Date", Order.Descending)
            };
            var fields = Field.From("Id", "Date", "DateTime");
            var where = new QueryField(conditions.Field, Conversions.GetOperation(conditions.Condition), conditions.Value);

            recordSet = connection.BatchQuery
                 (
                 tableName: entity,
                 fields: fields,
                 where: where,
                 trace: TraceFactory.CreateTracer(),
                 page: 0,
                 rowsPerBatch: 5,
                 orderBy: orderBy
                 );
        }

        return recordSet;
    }

    public dynamic? GetByPage(string database, string entity, FilterParams filters)
    {
        IEnumerable<dynamic> recordSet;
        string connString = string.Format(ConnectionString, database);
        using (var connection = new SqlConnection(connString))
        {
            //Ref:
            //1. https://repodb.net/operation/batchquery
            //2. https://repodb.net/feature/targeted

            #region Sorting
            //var orderBy = new[]
            //{
            //    new OrderField("Date", Order.Descending)
            //};
            IList<OrderField> orderBy = new List<OrderField>();
            foreach(var sort in filters.Sort)
            {
                orderBy.Add(new OrderField(sort.Field, sort.OrderBy));
            }
            #endregion

            #region Targeted Fields
            //var fields = Field.From("Id", "Date", "DateTime"); 
            //Field.Parse(filters.Fields);  - does not works
            //Field.From("Id", "Date", "DateTime"); - Works
            //var fields = Field.Parse(filters.Fields.ToArray());-> does not works
            var fields = Field.From(filters.Fields.ToArray());
            #endregion

            #region Where 
            //var where = new QueryField(conditions.Field, Conversions.GetOperation(conditions.Condition), conditions.Value);
            IList<QueryField> andQueryField = new List<QueryField>();
            IList<QueryField> orQueryField = new List<QueryField>();
            foreach (var condition in filters.Filter)
            {
                //AND
                if (condition.And is not null)
                {
                    foreach (var and in condition.And)
                    {
                        andQueryField.Add(new QueryField(and.Field, Conversions.GetOperation(and.Condition), and.Value));
                    }
                }
                //OR
                if (condition.Or is not null)
                {
                    foreach (var or in condition.Or)
                    {
                        orQueryField.Add(new QueryField(or.Field, Conversions.GetOperation(or.Condition), or.Value));
                    }
                }
                
            }
            //Grouping
            var orWhere = new QueryGroup(orQueryField, Conjunction.Or);
            var andWhere = new QueryGroup(andQueryField, Conjunction.And);
            var where = new QueryGroup( new[] { orWhere, andWhere });
            #endregion

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
            /*
             Generated Query:
                SELECT [Id], [Date], [DateTime] FROM [TestDates] WHERE 
                (([Bit] = 'true') AND ([Date] = '2024-10-04')) 
                ORDER BY [Date] DESC, 
                [Id] DESC OFFSET 0 ROWS FETCH NEXT 5 ROWS ONLY ;
            But batchQuery Method returns '0'. Why?
             */
        }

        return recordSet;
    }



}



