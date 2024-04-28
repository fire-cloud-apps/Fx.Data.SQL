using RepoDb.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Data.SQL.Helpers;

/// <summary>
/// Conditions query.
/// </summary>
public class Conditions : InputFields
{    
    /// <summary>
    /// Set the on the condition, form which page we need to read. Page stats from 0, Default value is '0'.
    /// </summary>
    public int Page { get; set; } = 0;
    /// <summary>
    /// No of records to be retrieved from the database.
    /// </summary>
    public int RowsPerBatch { get; set; } = 10;
}

