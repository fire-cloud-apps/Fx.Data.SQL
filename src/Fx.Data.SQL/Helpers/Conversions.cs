using MySqlX.XDevAPI.Common;
using RepoDb;
using RepoDb.Enumerations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
            object targetType = GetType(item.Value);
            target.Add(item.Key, targetType);
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
                operation = Operation.NotBetween;
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

    /// TODO: Need to extend this method for all Data Types.
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
        var ci = new CultureInfo("en-US");
        var formats = new[] { "yyyy-dd-MM HH:mm:ss", "dd-MM-yyyy HH:mm:ss", "M-d-yyyy", "dd-MM-yyyy", "MM-dd-yyyy", "M.d.yyyy", "dd.MM.yyyy", "MM.dd.yyyy" };
        object objValue = value;
        string? strValue = objValue.ToString();

        if(regexPattern.Count <= 0)
        {
            InvokePattern();
        }
        
        if (strValue is not null)
        {
            foreach(var regex in regexPattern)
            {
                if (Regex.IsMatch(value, regex.Value))
                {
                    switch (regex.Key)
                    {
                        case "int":
                            int intValue = int.Parse(strValue);
                            objValue = intValue;
                            break;
                        case "float":
                            float floatValue = float.Parse(strValue);
                            objValue = floatValue;
                            break;
                        case "decimal":
                            decimal decValue = decimal.Parse(strValue);
                            objValue = decValue;
                            break;
                        case "bool":
                            bool bValue = bool.Parse(strValue);
                            objValue = bValue;
                            break;
                        case "date":
                        case "dateTime":
                            DateTime dtValue = DateTime.ParseExact(value, formats, ci, DateTimeStyles.AssumeLocal);
                            objValue = dtValue;
                            break;
                        case "json":
                            objValue = GetCustomDataType(value);
                            break;
                        case "base65":
                            byte[] btValue = Encoding.ASCII.GetBytes(strValue);
                            objValue = btValue;
                            break;
                        default:
                            objValue = value as string;
                            break;
                    }
                    Console.WriteLine($"DataType Match for the value '{value}' is '{regex.Key}'");
                    break;
                }
            }
        }
        return objValue;
    }

    private static object GetCustomDataType(string value)
    {
        object objValue;
        CustomDataType? customData = JsonSerializer.Deserialize<CustomDataType>(value);
        if (customData is not null)
        {
            switch (customData.Type)
            {
                case "byte[]":
                    byte[] btVal = Encoding.ASCII.GetBytes(customData.Value);
                    objValue = btVal;
                    break;
                case "guid":
                    Guid guid = new Guid(value);
                    objValue = guid;
                    break;
                case "xml":
                default:
                    objValue = value; break;
            }
        }
        else
        {
            objValue = value;
        }

        return objValue;
    }

    static Dictionary<string, string> regexPattern = new Dictionary<string, string>();

    static string regexDateTime = @"^(3[01]|[12][0-9]|0?[1-9])(\/|-|.)(1[0-2]|0?[1-9])\2([0-9]{2})?[0-9]{2} \d{2}:\d{2}:\d{2}$";
    static string regexDate = @"^(3[01]|[12][0-9]|0?[1-9])(\/|-|.)(1[0-2]|0?[1-9])\2([0-9]{2})?[0-9]{2}$";

    static void InvokePattern()
    {
        regexPattern.Clear();
        regexPattern.Add("int", @"^\d+$");
        regexPattern.Add("float", @"^\d+(\.\d+)?$");
        regexPattern.Add("decimal", @"^\d+\.\d+$");
        regexPattern.Add("date", regexDate);
        regexPattern.Add("dateTime", regexDateTime);
        regexPattern.Add("bool", @"^(true|false)$");
        regexPattern.Add("json", @"^(?:\{.*\}|\[.*\])$");
        regexPattern.Add("base65", @"\bbase64\b");//Checks if the string contains base64 string value if yes, converts it as byte[].

    }

    //In future we can try to implement the below code to identify data type
    /*
     *  DataTable? dataTable;
        using (var reader = connection.ExecuteReader($"Select * from {entity};", CommandBehavior.SchemaOnly))
        {
            dataTable = reader.GetSchemaTable();
        }
        foreach(DataRow row in dataTable.Rows)
        {
            var name = row["ColumnName"];
            var size = row["ColumnSize"];
            var dataType = row["DataTypeName"];
            Console.WriteLine("ColumnName={0}", row.Field<string>("ColumnName"));
        }
     */

    public static Dictionary<string, dynamic> DataTypeConversion(Dictionary<string, string> parameter, IDbConnection dbConnection, string table)
    {
        DataTable? dataTable;
        Dictionary<string, dynamic> targetValue = new Dictionary<string, dynamic>();
        using (var reader = dbConnection.ExecuteReader($"Select * from {table};", CommandBehavior.SchemaOnly))
        {
            dataTable = reader.GetSchemaTable();
        }

        foreach (DataRow row in dataTable.Rows)
        {
            foreach (var item in parameter)
            {
                if (item.Key.ToLower() == row["ColumnName"].ToString().ToLower())
                {
                    Console.WriteLine($"Model Match Name : {item.Key} DB ColumnName: {row["ColumnName"]} DataType: {row["DataTypeName"]}");
                    var convertedValue = GetTypeFromDBType(row["DataTypeName"].ToString(), item.Value);
                    targetValue.Add(item.Key, convertedValue);
                    break;
                }
            }
            //var name = row["ColumnName"];
            //var size = row["ColumnSize"]; 
            //var dataType = row["DataTypeName"];
            //Console.WriteLine("ColumnName={0}", row.Field<string>("ColumnName"));
        }
        
        
        return targetValue;
    }

    public static dynamic GetTypeFromDBType(string sqlServerDataType,  string value)
    {
        dynamic returnValue = null;
        if (value is not null)
        {
            switch (sqlServerDataType.ToLower())
            {
                case "int":
                case "smallint":
                case "tinyint":
                    returnValue = int.Parse(value);
                    break;
                case "bigint":
                    returnValue = long.Parse(value);
                    break;
                case "decimal":
                case "money":
                    returnValue = decimal.Parse(value);
                    break;
                case "float":
                    returnValue = float.Parse(value);
                    break;
                case "real":
                    returnValue = double.Parse(value);
                    break;
                case "char":
                    returnValue = char.Parse(value);
                    break;
                case "nvarchar":
                case "varchar":
                case "text":
                    returnValue = value;
                    break;
                //return "string";
                case "datetime":
                case "smalldatetime":
                case "date":
                    returnValue =DateTime.Parse(value);
                    break;
                case "bit":
                    returnValue = bool.Parse(value);
                    break;
                case "image":
                case "varbinary":
                case "binary":
                    byte[] btVal = Encoding.ASCII.GetBytes(value);
                    returnValue = btVal;
                    break;
                default:
                    // Handle unsupported data types or provide a more generic option
                    // Consider logging or throwing an exception for unknown types
                    returnValue = value;
                    break;
                    //return "object";
            }
        }
        return returnValue;
    }

}
