using Fx.Data.SQL.Handler;
using Fx.Data.SQL.Helpers;
using Fx.Data.SQL.Interface;
using Microsoft.Data.Sqlite;
using Shouldly;
using System.Text.Json;
using Xunit.Abstractions;

namespace Fx.Data.SQL.xUnit
{
    //https://xunit.net/docs/capturing-output
    //to run full test in console: dotnet test --logger "console;verbosity=detailed"
    public class SQLServerEntityServiceTest
    {
        private readonly ITestOutputHelper output;
        IEntityService _entityService;
        string _connectionString;
        string _dbName = "DMS";
        string _tblName = "test";

        #region Global Contructor
        public SQLServerEntityServiceTest(ITestOutputHelper output)
        {
            this.output = output;
            #region SQL Server DB Settings
            _connectionString = "Server=localhost;Database={0};User Id=sa;Password=System@1984;TrustServerCertificate=True;";//DMS
            string connString = string.Format(_connectionString, _dbName);
            _entityService = new SQLEntityService(new SqliteConnection(connString), null);
            #endregion
        }
        #endregion

        #region RepoDB Interface Query
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

        [Fact]
        public void GetById_Test()
        {
            var userDetails = _entityService.GetById( "User", "1");
            string json = JsonSerializer.Serialize(userDetails);
            json.ShouldNotBeEmpty();
            output.WriteLine($"Resulted Record {json}");
        }

        [Fact]
        public void GetBySingle_Test()
        {
            var userDetails = _entityService.GetSingle("User", new Helpers.Conditions()
            {
                Condition = "gt",
                Field = "ExpiresIn",
                Value = "10"
            });
            string json = JsonSerializer.Serialize(userDetails);
            json.ShouldNotBeEmpty();
            output.WriteLine($"Resulted Record {json}");
        }

        //[Fact]
        public void ExecuteNonQuery_Test()
        {
            string query = "CREATE TABLE DMS.dbo.VarCharTable (\r\n\tId int IDENTITY(0,1) NOT NULL,\r\n\tVarCharTest varchar(150) NOT NULL,\r\n\tCONSTRAINT NewTable_PK PRIMARY KEY (Id)\r\n);\r\n";
            var userDetails = _entityService.ExecuteNonQuery(query);

            string json = JsonSerializer.Serialize(userDetails);
            json.ShouldNotBeEmpty();
            output.WriteLine($"Result: {json}");
        }
    }
}