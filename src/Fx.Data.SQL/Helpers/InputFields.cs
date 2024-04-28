using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Data.SQL.Helpers;

public class InputFields
{
    public string Field { get; set; }
    public object _value = string.Empty;
    public object Value
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
    
    /// <summary>
     /// gt, gte, lt, lte, eq, neq, lk, nlk, bt, nbt, in, nin => all from 'RepoDb.Enumerations'
     /// </summary>
    public string Condition { get; set; } = "eq";
}
