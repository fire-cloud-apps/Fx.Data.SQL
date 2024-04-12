using RepoDb.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Data.SQL.Helpers;
public class Conversions
{
    public static Dictionary<string, object> ParameterConversion(Dictionary<string, string> parameter)
    {
        var target = new Dictionary<string, object>();

        foreach (var item in parameter)
        {
            //Console.WriteLine($"Key: {item.Key} Value:{item.Value} Type: {item.GetType().Name}");
            target.Add(item.Key, item.Value);
        }
        return target;
    }

    public static string DictionaryToJson(Dictionary<string, string> dict)
    {
        var entries = dict.Select(d =>
            string.Format("\"{0}\": \"{1}\"", d.Key, string.Join(",", d.Value)));
        return "{" + string.Join(",", entries) + "}";
    }

    public static Operation GetOperation(string condition)
    {
        Operation operation = Operation.Equal;
        //gt, gte, lt, lte, eq, neq, lk, nlk, bt, nbt, in, nin => all from 'RepoDb.Enumerations'
        switch (condition)
        {
            case "gt":
                operation = Operation.GreaterThan;
                break;
            case "gte":
                operation = Operation.GreaterThanOrEqual;
                break;
            case "lt":
                operation = Operation.LessThan;
                break;
            case "lte":
                operation = Operation.LessThanOrEqual;
                break;
            
            case "lk":
                operation = Operation.Like;
                break;
            case "nlk":
                operation = Operation.NotLike;
                break;
            case "bt":
                operation = Operation.Between;
                break;
            case "nbt":
                operation = Operation.NotLike;
                break;
            case "in":
                operation = Operation.In;
                break;
            case "nin":
                operation = Operation.NotIn;
                break;
            case "neq":
                operation = Operation.NotEqual;
                break;
            case "eq":
            default:
                operation = Operation.Equal;
                break;
        }
        return operation;
    }

    /// <summary>
    /// Converts to various Types.
    /// Currently Supported types: int,float,long, boolean, datetime
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static object GetType(string value)
    {
        object objValue = value;
        
        if (int.TryParse(value, out int intValue))
        {            
            objValue = intValue;
        }
        if (float.TryParse(value, out float floatValue))
        {
            objValue = floatValue;
        }
        if (long.TryParse(value, out long longValue))
        {
            objValue = longValue;
        }
        else if(DateTime.TryParse(value, out DateTime dtValue))
        {
            objValue = dtValue;
        }
        else if (bool.TryParse(value, out bool boValue))
        {
            objValue = boValue;
        }
        else
        {
            objValue = value;
        }

        return objValue;
    }    
}
