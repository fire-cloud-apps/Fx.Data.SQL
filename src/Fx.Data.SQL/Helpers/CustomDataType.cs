using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Data.SQL.Helpers;

/// <summary>
/// For more complex datatypes, we should use this method to identify a target data type form the sender.
/// </summary>
/// <remarks>
/// https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
/// </remarks>
public class CustomDataType
{
    /// <summary>
    /// Target Data Type as string. Eg. 'byte[]', Guid, Char[], XML or even string etc.
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// Data valye as string.
    /// </summary>
    public string Value { get; set; }
}

