using RepoDb.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Data.SQL.Helpers;

/// <summary>
/// Very much to convert all kind of data type from C# to RDBMS
/// </summary>
public class Conversions
{
    /// <summary>
    /// Used to convert the parameter value to the right data type.
    /// </summary>
    /// <param name="parameter">Dictionary object</param>
    /// <returns>returns the target data type.</returns>
    public static Dictionary<string, object> ParameterConversion(Dictionary<string, string> parameter)
    {
        var target = new Dictionary<string, object>();

        foreach (var item in parameter)
        {
            //Console.WriteLine($"Key: {item.Key} Value:{item.Value} Type: {item.GetType().Name}");
            target.Add(item.Key, GetType(item.Value));
        }
        return target;
    }

    /// <summary>
    /// Converts the dictionary Object as 'JSON' Data.
    /// </summary>
    /// <param name="dict">Dictionary Object</param>
    /// <returns>returns a JSON String</returns>
    public static string DictionaryToJson(Dictionary<string, string> dict)
    {
        var entries = dict.Select(d =>
            string.Format("\"{0}\": \"{1}\"", d.Key, string.Join(",", d.Value)));
        return "{" + string.Join(",", entries) + "}";
    }
    /// <summary>
    /// Get the Operation condition to be used which is equivalent to RepoDb 'Operation'.
    /// </summary>
    /// <param name="condition">the condition string as 'gt, gte, lt, lte, eq, neq, lk, nlk, bt, nbt, in, nin => all from 'RepoDb.Enumerations''.</param>
    /// <returns></returns>
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

    //TODO: Need to extend this method for all Data Types.
    /// <summary>
    /// Converts to various Types.
    /// Currently Supported types: int,float,long, boolean, datetime.
    /// Ref: 
    /// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types
    /// https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
    /// </summary>
    /// <param name="value">actual value to be converted</param>
    /// <returns>converted value return with right data type.</returns>  
    public static object GetType(string value)
    {
        object objValue = value;

        if (int.TryParse(value, out int intValue))
            objValue = intValue;
        else if (float.TryParse(value, out float floatValue))
            objValue = floatValue;
        else if (long.TryParse(value, out long longValue))
            objValue = longValue;
        else if (decimal.TryParse(value, out decimal decimalValue))
            objValue = decimalValue;
        else if (bool.TryParse(value, out bool boolValue))
            objValue = boolValue;
        else if (byte.TryParse(value, out byte byteValue))
            objValue = byteValue;
        else if(DateTime.TryParse(value, out DateTime dateTimeValue))        
            objValue = dateTimeValue;
        else
        {
            // Use Type.GetType() for non-primitive types
            Type customType = Type.GetType(value);
            if (customType != null)
                objValue = Activator.CreateInstance(customType);
        }

        return objValue;
    }

    /*
     public static object GetType(string value)
    {
        object objValue = value;
        do
        {
            if (int.TryParse(value, out int intValue))
            {
                objValue = intValue;
                break;
            }
            if (float.TryParse(value, out float floatValue))
            {
                objValue = floatValue;
                break;
            }
            if (long.TryParse(value, out long longValue))
            {
                objValue = longValue;
                break;
            }
            if (decimal.TryParse(value, out decimal decimalValue))
            {
                objValue = decimalValue;
                break;
            }
            else if (DateTime.TryParse(value, out DateTime dtValue))
            {
                objValue = dtValue;
                break;
            }
            else if (bool.TryParse(value, out bool boValue))
            {
                objValue = boValue;
                break;
            }
            else if (byte.TryParse(value, out byte byteValue))
            {
                objValue = byteValue;
                break;
            }
            else
            {
                objValue = value;
                break;
            }
        } while (false);
        return objValue;
        
    }
     */

}
