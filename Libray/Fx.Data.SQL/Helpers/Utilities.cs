using RepoDb;
using RepoDb.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Data.SQL.Helpers;

public static class Utilities
{
    public static IList<OrderField> GetOrderBy(List<Sort> sorts)
    {
        IList<OrderField> orderBy = new List<OrderField>();
        if (sorts is null)
        {
            throw new ArgumentException("Sort Field is required. eg. \"Sort\":[\r\n        {\r\n            \"Field\": \"Id\",\r\n            \"OrderBy\":\"Descending\"\r\n        }\r\n    ]");
        }
        else
        {
            foreach (var sort in sorts)
            {
                orderBy.Add(new OrderField(sort.Field, sort.OrderBy));
            }
        }
        return orderBy;
    }

    public static QueryGroup GetQueryGroup(IList<Filter> filters)
    {
        QueryGroup where = null;
        if (filters is not null)
        {
            IList<QueryField> andQueryField = new List<QueryField>();
            IList<QueryField> orQueryField = new List<QueryField>();
            foreach (var condition in filters)
            {
                //AND
                if (condition.And is not null)
                {
                    foreach (var and in condition.And)
                    {
                        andQueryField.Add(new QueryField
                            (
                            fieldName: and.Field,
                            operation: Conversions.GetOperation(and.Condition),
                            value: and.Value
                            ));
                    }
                }
                //OR
                if (condition.Or is not null)
                {
                    foreach (var or in condition.Or)
                    {
                        orQueryField.Add(new QueryField
                            (
                            fieldName: or.Field,
                            operation: Conversions.GetOperation(or.Condition),
                            value: or.Value
                            ));
                    }
                }
            }

            QueryGroup orWhere = new QueryGroup(orQueryField, Conjunction.Or);
            QueryGroup andWhere = new QueryGroup(andQueryField, Conjunction.And);

            where = andQueryField.Count > 0 && orQueryField.Count > 0
                ? new QueryGroup(new[] { orWhere, andWhere })
                : andQueryField.Count > 0
                ? andWhere
                : orWhere;
        }
        return where;
    }
}

