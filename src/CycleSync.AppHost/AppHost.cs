var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlserver");
var cycleSyncDatabase = sqlServer.AddDatabase("cyclesyncdb");

var api = builder.AddProject<Projects.CycleSync_Api>("api")
    .WithReference(cycleSyncDatabase)
    .WaitFor(cycleSyncDatabase)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

builder.AddViteApp("web", "../CycleSync.Web")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
