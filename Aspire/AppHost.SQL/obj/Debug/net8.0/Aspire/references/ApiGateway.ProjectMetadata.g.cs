namespace Projects;

[global::System.Diagnostics.DebuggerDisplay("Type = {GetType().Name,nq}, ProjectPath = {ProjectPath}")]
public class ApiGateway : global::Aspire.Hosting.IProjectMetadata
{
  public string ProjectPath => """C:\Works\Github\Fx.Data.SQL\Aspire\ApiGateway\ApiGateway.csproj""";
}
