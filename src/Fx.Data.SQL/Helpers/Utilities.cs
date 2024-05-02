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
            IList<QueryGroup> newGroupList = new List<QueryGroup>(); 

            #region New Query Group Code
            foreach (var conditions in filters)
            {
                if (conditions is not null)
                {
                    IList<QueryField> newQueryField = new List<QueryField>();
                    foreach (var inputFields in conditions.ConditionFields)
                    {
                        var query = GetQueryField(inputFields);
                        newQueryField.Add(query);
                    }
                    Enum.TryParse(conditions.Conjunctions, out Conjunction conj);

                    var queryGroup = new QueryGroup(newQueryField.ToArray(), conj);
                    newGroupList.Add(queryGroup);
                }
            }
            where = new QueryGroup(newGroupList.ToList());
            #endregion
        }
        return where;
    }

    private static QueryField GetQueryField(InputFields input)
    {
        object val;
        if (input.Condition == "bt" || input.Condition == "nbt" || 
            input.Condition == "in" || input.Condition == "nin")
        {
            List<object> values = new List<object>();

            if (input.Value is not null)
            {
                values.Add(input.Value);
            }
            if (input.Value1 is not null)
            {
                values.Add(input.Value1);
            }
            if (input.Value2 is not null)
            {
                values.Add(input.Value2);
            }

            object[] extractedObj = values.ToArray();
            val = extractedObj;
        }
        else
        {
            val = input.Value;
        }
        return new QueryField
        (
            fieldName: input.Field,
            operation: Conversions.GetOperation(input.Condition),
            value: val
        );
    }

    #region Old Code Backup

    #region Old Group Code

    //IList <QueryField> andQueryField = new List<QueryField>();
    //IList<QueryField> orQueryField = new List<QueryField>();

    //foreach (var condition in filters)
    //{

    //    //AND
    //    if (condition.And is not null)
    //    {
    //        foreach (var and in condition.And)
    //        {
    //            ExtractQueryField(and, andQueryField);
    //        }
    //    }
    //    //OR
    //    if (condition.Or is not null)
    //    {
    //        foreach (var or in condition.Or)
    //        {
    //            ExtractQueryField(or, orQueryField);
    //        }
    //    }
    //}

    //QueryGroup orWhere = new QueryGroup(orQueryField, Conjunction.Or);
    //QueryGroup andWhere = new QueryGroup(andQueryField, Conjunction.And);

    //where = andQueryField.Count > 0 && orQueryField.Count > 0
    //    ? new QueryGroup(new[] { andWhere, orWhere })
    //    : andQueryField.Count > 0
    //    ? andWhere
    //    : orWhere;
    #endregion

    /*
    private static void ExtractQueryField(InputFields input, IList<QueryField> queryFields)
    {
        object val;
        if (input.Condition == "bt" || input.Condition == "nbt" || input.Condition == "in" || input.Condition == "nin")
        {
            List<object> values = new List<object>();

            if (input.Value is not null)
            {
                values.Add(input.Value);
            }
            if (input.Value1 is not null)
            {
                values.Add(input.Value1);
            }
            if (input.Value2 is not null)
            {
                values.Add(input.Value2);
            }

            object[] extractedObj = values.ToArray();
            val = extractedObj;
        }
        else
        {
            val = input.Value;
        }
        queryFields.Add(new QueryField
        (
            fieldName: input.Field,
            operation: Conversions.GetOperation(input.Condition),
            value: val
        ));
    }*/
    #endregion

}

