using Fx.Data.SQL.Helpers;
using Fx.Data.SQL.Interface;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fx.Data.SQL.xUnit.Handlers;

public partial class SQLEntityServiceTest
{
    #region Count
    [Fact]
    public void Execute_Count_Test()
    {
        var userDetails = _entityService.Count(_tblName, new List<Filter>()
            {
                new Filter()
                {
                    And = new List<And>()
                    {
                        new And()
                        {
                            Condition = "eq",
                            Field ="C1",
                            Value = "Field 3"
                        }
                    }
                }
            });
        string json = JsonSerializer.Serialize(userDetails);
        json.ShouldNotBeEmpty();
        output.WriteLine($"Execute_Count_Test Count: {json}");
    }
    #endregion

    #region Exists
    [Fact]
    public void Execute_Exists_Test()
    {
        var resultSet = _entityService.Exists(_tblName, new List<Filter>()
            {
                new Filter()
                {
                    And = new List<And>()
                    {
                        new And()
                        {
                            Condition = "eq",
                            Field ="C1",
                            Value = "Field 3"
                        }
                    }
                }
            });
        string json = JsonSerializer.Serialize(resultSet);
        json.ShouldNotBeEmpty();
        output.WriteLine($"Execute_Count_Test Count: {json}");
    }
    #endregion

    #region Average
    [Fact]
    public void Execute_Average_Test()
    {
        var userDetails = _entityService.Average("Orders", "Freight", new List<Filter>()
            {
                new Filter()
                {
                    And = new List<And>()
                    {
                        new And()
                        {
                            Condition = "eq",
                            Field ="EmployeeId",
                            Value = "4"
                        }
                    }
                }
            });
        string json = JsonSerializer.Serialize(userDetails);
        json.ShouldNotBeEmpty();
        output.WriteLine($"Execute_Average_Test Count: {json}");
    }
    #endregion
}
