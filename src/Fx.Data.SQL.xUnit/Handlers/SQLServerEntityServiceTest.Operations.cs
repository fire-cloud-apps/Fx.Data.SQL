using Fx.Data.SQL.Handler;
using Fx.Data.SQL.Helpers;
using Fx.Data.SQL.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fx.Data.SQL.xUnit.Handlers;

public partial class SQLEntityServiceTest
{
    #region Count
    [Fact]
    //[Theory]
    //[MemberData(nameof(FilterData.Data))]
    //[InlineData(FilterData.Data)]
    public void Count_Test()
    {
        var userDetails = _entityService.Count(_tblName, null );
        string json = JsonSerializer.Serialize(userDetails);
        json.ShouldNotBeEmpty();
        output.WriteLine($"Execute_Count_Test Count: {json}");
    }

    #endregion

    #region Exists
    [Fact]
    public void Execute_Exists_Test()
    {
        var resultSet = _entityService.Exists("Employees", new List<Filter>()
            {
                new Filter()
                {
                    And = new List<And>()
                    {
                        new And()
                        {
                            Condition = "eq",
                            Field ="EmployeeId",
                            Value = "5"
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

    #region Sum
    [Fact]
    public void Execute_Sum_Test()
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
                            Value = "6"
                        }
                    }
                }
            });
        string json = JsonSerializer.Serialize(userDetails);
        json.ShouldNotBeEmpty();
        output.WriteLine($"Execute_Sum_Test Count: {json}");
    }
    #endregion

    #region Max
    [Fact]
    public void Execute_Max_Test()
    {
        var userDetails = _entityService.Max("Orders", "Freight", new List<Filter>()
            {
                new Filter()
                {
                    And = new List<And>()
                    {
                        new And()
                        {
                            Condition = "eq",
                            Field ="EmployeeId",
                            Value = "6"
                        }
                    }
                }
            });
        string json = JsonSerializer.Serialize(userDetails);
        json.ShouldNotBeEmpty();
        output.WriteLine($"Execute_Sum_Test Count: {json}");
    }
    #endregion

    #region Min
    [Fact]
    public void Execute_Min_Test()
    {
        var userDetails = _entityService.Min("Orders", "Freight", new List<Filter>()
            {
                new Filter()
                {
                    And = new List<And>()
                    {
                        new And()
                        {
                            Condition = "eq",
                            Field ="EmployeeId",
                            Value = "6"
                        }
                    }
                }
            });
        string json = JsonSerializer.Serialize(userDetails);
        json.ShouldNotBeEmpty();
        output.WriteLine($"Execute_Sum_Test Count: {json}");
    }
    #endregion

    #region Truncate
    [Fact]
    public void Execute_Truncate_Test()
    {
        string sampleConnection = "Server=localhost;Database=DMS;User Id=sa;Password=System@1984;TrustServerCertificate=True;";

        SqlConnection sqlServerConnection = new SqlConnection(sampleConnection);
        _entityService = new SQLEntityService(sqlServerConnection, null);

        var userDetails = _entityService.Truncate("TestNumerics");
        string json = JsonSerializer.Serialize(userDetails);
        json.ShouldNotBeEmpty();
        output.WriteLine($"Execute_Truncate_Test Count: {json}");
    }
    #endregion
}


public class FilterData
{
    public static IList<Filter> Data => new List<Filter>()
            {
                new Filter()
                {
                    And = new List<And>()
                    {
                        new And()
                        {
                            Condition = "eq",
                            Field ="Freight",
                            Value = "Field 3"
                        }
                    }
                }
            };
}