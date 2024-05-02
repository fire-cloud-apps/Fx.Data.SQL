using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Data.SQL.Helpers;

public class InputFields
{
    public string Field { get; set; }
    public object? _value = string.Empty;
    public object? Value
    {
        get
        {
            //Currently supports primary items, we should extend this.
            var convertedValue = Conversions.GetType(_value.ToString());
            return convertedValue;
        }
        set
        {
            _value = value;
        }
    }
    private object? _value1 = string.Empty;
    /// <summary>
    /// Used for 'between' and 'In' Operators
    /// </summary>
    public object? Value1
    {
        get
        {
            //Currently supports primary items, we should extend this.
            var convertedValue = Conversions.GetType(_value1.ToString());
            return convertedValue;
        }
        set
        {
            _value1 = value;
        }

    }
    private object? _value2 = string.Empty;
    /// <summary>
    /// Used for 'between' and 'In' Operators
    /// </summary>
    public object? Value2
    {
        get
        {
            var convertedValue = Conversions.GetType(_value2.ToString());
            return convertedValue;
        }
        set
        {
            _value2 = value;
        }
    }


    /// <summary>
    /// gt, gte, lt, lte, eq, neq, lk, nlk, bt, nbt, in, nin => all from 'RepoDb.Enumerations'
    /// </summary>
    public string Condition { get; set; } = "eq";

    /// <summary>
    /// Condition Group Logic, based on 'And' or 'Or'. Default is And
    /// </summary>
    //public string GroupLogic { get; set; } = "And";
}
