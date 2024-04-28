## Fx.Data.SQL
Supports

1. SQL Server
2. MySQL
3. SQLite
4. PostgreSQL

## How to use?

1. Install-Package Fx.Data.SQL

2. Code to Initialize SQL Server

### Initialization on SQL Server
```
IEntityService _entityService;
SqlConnection sqlServerConnection = new SqlConnection(connString);
_entityService = new SQLEntityService(sqlServerConnection, logger);
```

### Initialization on PostgreSQL
```
IEntityService _entityService;
NpgsqlConnection sqlServerConnection = new NpgsqlConnection(ConnectionString);
_entityService = new SQLEntityService(sqlServerConnection, logger);
```

### Initialization on SQLite
```
IEntityService _entityService;
SQLiteConnection sqlServerConnection = new SQLiteConnection(ConnectionString);
_entityService = new SQLEntityService(sqlServerConnection, logger);
```

### Initialization on MySQL
```
IEntityService _entityService;
MySqlConnection sqlServerConnection = new MySqlConnection(ConnectionString);
_entityService = new SQLEntityService(sqlServerConnection, logger);
```

### Execution, never changes

```
var userDetails = _entityService.GetById("User", "1");
string json = JsonSerializer.Serialize(userDetails);
Console.WriteLine($"Resulted Record {json}");

```

