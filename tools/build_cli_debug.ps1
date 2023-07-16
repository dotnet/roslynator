dotnet clean "../src/CommandLine.sln" ` -c Debug

dotnet build "../src/CommandLine.sln" -c Debug -v minimal /p:RoslynatorDotNetCli=true /m

Write-Host "DONE"
