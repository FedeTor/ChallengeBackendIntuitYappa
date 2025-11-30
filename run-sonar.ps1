param(
    [Parameter(Mandatory = $true)]
    [string]$Token
)

if (Test-Path "tests\Clientes.Tests\TestResults") {
    Remove-Item -Recurse -Force "tests\Clientes.Tests\TestResults"
}

dotnet sonarscanner begin `
  /k:"challenge-clientes" `
  /d:sonar.host.url="http://localhost:9000" `
  /d:sonar.token="$Token" `
  /d:sonar.cs.opencover.reportsPaths="tests/Clientes.Tests/TestResults/coverage.opencover.xml" `
  /d:sonar.coverage.exclusions="**/*Tests/**/*,**/obj/**/*,**/bin/**/*,**/*.Designer.cs,**/Program.cs" `
  /d:sonar.test.exclusions="**/*Tests/**/*"

dotnet build --no-incremental

dotnet test "tests/Clientes.Tests/Clientes.Tests.csproj" `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=opencover `
  /p:CoverletOutput=TestResults/ `
  /p:Exclude="[*.Tests]*" `
  /p:ExcludeByFile="**/Program.cs"

dotnet sonarscanner end /d:sonar.token="$Token"
