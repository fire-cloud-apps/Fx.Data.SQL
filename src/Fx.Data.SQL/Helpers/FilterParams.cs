using RepoDb.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fx.Data.SQL.Helpers;


/// <summary>
/// Filter Params
/// </summary>
public class FilterParams
{
    /// <summary>
    /// Fields to return or columns where operations to be conducted.
    /// </summary>
    public List<string> Fields { get; set; }
    public List<Sort> Sort { get; set; }
    public List<Filter> Filter { get; set; }
    public int Page { get; set; } = 0;
    public int RowsPerBatch { get; set; } = 10;
}

/// <summary>
/// "AND" Condition
/// </summary>
public class And : InputFields
{
    
}
/// <summary>
/// "OR" condition
/// </summary>
public class Or : InputFields
{
}
/// <summary>
/// Filter object with 'And' / 'Or'.
/// </summary>
public class Filter
{
    #region New Query Group
    public IList<InputFields> ConditionFields { get; set; }

    public string Conjunctions { get; set; } = "And";

    #endregion

    #region Old Query Group
    /// <summary>
    /// 'And' Condition as a List
    /// </summary>
    //public List<And> And { get; set; }
    /// <summary>
    /// 'OR' Condition as a list
    /// </summary>
    //public List<Or> Or { get; set; }
    #endregion
}


/// <summary>
/// Sort conditions
/// </summary>
public class Sort
{
    /// <summary>
    /// Sorting Field
    /// </summary>
    public string Field { get; set; }
    /// <summary>
    /// Order By 'Descending' or 'Ascending'
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Order OrderBy { get; set; }
}





/*
 
{
    "Fields":["Id","Date","DateTime"],
    "Sort":[
        {
            "Field": "Date",
            "OrderBy":"Desc"

        },
        {
            "Field": "Date",
            "OrderBy":"Desc"
        }
    ],
    "Filter":[
        {
            "And":[
                {
                    "Field":"Date",
                    "Value":"2024-10-04",
                    "Condition":"eq"
                }
            ],
            "Or":[
                {
                    "Field":"Bit",
                    "Value":"false",
                    "Condition":"eq"
                }
            ]
        }
    ],
    "Page":"0",
    "RowsPerBatch":"5"
}
 */