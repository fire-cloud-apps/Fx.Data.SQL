var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Fx_Data_SQLAPIs>("fx-data-sqlapis");

builder.AddProject<Projects.Fx_Authentication_Service>("fx-authentication-service");

builder.Build().Run();
