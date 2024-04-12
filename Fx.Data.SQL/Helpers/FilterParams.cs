﻿using RepoDb.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fx.Data.SQL.Helpers;


public class FilterParams
{
    public List<string> Fields { get; set; }
    public List<Sort> Sort { get; set; }
    public List<Filter> Filter { get; set; }
    public int Page { get; set; }
    public int RowsPerBatch { get; set; }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<FilterParams>(myJsonResponse);
public class And
{
    public string Field { get; set; }
    public string Value { get; set; }
    public string Condition { get; set; }
}

public class Filter
{
    public List<And> And { get; set; }
    public List<Or> Or { get; set; }
}

public class Or
{
    public string Field { get; set; }
    public string Value { get; set; }
    public string Condition { get; set; }
}

public class Sort
{
    public string Field { get; set; }
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