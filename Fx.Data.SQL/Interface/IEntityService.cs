using Fx.Data.SQL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parameters = System.Collections.Generic.Dictionary<string, string>;
using Record = System.Collections.Generic.IDictionary<string, object>;
using Records = System.Collections.Generic.List<System.Collections.Generic.IDictionary<string, object>>;

namespace Fx.Data.SQL.Interface;

// <summary>
/// Entity Service Interface to implement in RDBMS
/// </summary>
public interface IEntityService
{
    long Create(string database, string entity, Parameters jsonData);
    dynamic? GetSingle(string database, string entity, Conditions conditionColumns);
    dynamic? GetByPage(string database, string entity, Conditions conditionColumns);
    dynamic? GetByPage(string database, string entity, FilterParams filters);
}
/*
        Records GetAll(string tableName, Conditions where = null, Joins joins = null, bool hasDeleted = true, string schema = null);
        List<T> GetAll<T>(string tableName, Conditions where = null, Joins joins = null, bool hasDeleted = true, string schema = null) where T : new();
        Record GetSingle(string tableName, int recordId, Joins joins = null);
        Record GetSingle(string tableName, Conditions where);
        Record GetSingle(string tableName, Conditions where, Joins joins = null);
        Record GetById(string tableName, int entityId);
        Record Insert(string tableName, Parameters parms);
        Record Update(string tableName, int entityId, Parameters parms);
        void SoftDelete(string tableName, int entityId);
        void HardDelete(string tableName, int entityId);
        bool IsEntityValid(string entityName);
        bool IsUserActionAuthorized(string entityName, int userId, string method);
 */





