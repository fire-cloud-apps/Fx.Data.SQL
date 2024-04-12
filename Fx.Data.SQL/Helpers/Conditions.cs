using RepoDb.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Data.SQL.Helpers;
public class Conditions
{
    public string Field { get; set; }
    public object _value = string.Empty;
    public object Value
    {
        get
        {
            var convertedValue = Conversions.GetType(_value.ToString());
            return convertedValue;
        }
        set
        {
            _value = value;
        }
    }

    /// <summary>
    /// gt, gte, lt, lte, eq, neq, lk, nlk, bt, nbt, in, nin => all from 'RepoDb.Enumerations'
    /// </summary>
    public string Condition { get; set; }
}


public class JSONCondition
{
    public int Limit { get; set; }
    public int Page { get; set; }
    public IList<string> Fields { get; set; }
    public IList<QueryFilter> Filters { get; set; }

}

/// <summary>
/// Which is an alternative of QueryField
/// </summary>
public class QueryFilter
{
    public string Field { get; set; }
    public string Value { get; set; }
    public Operation Condition { get; set; }
    /// <summary>
    /// Optional lets have this, if required we can use it for some datatype.
    /// </summary>
    public string DataType { get; set; } = string.Empty;
}


