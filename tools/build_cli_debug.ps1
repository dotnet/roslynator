dotnet clean "$PSScriptRoot/../src/CommandLine.slnx" ` -c Debug

dotnet build "$PSScriptRoot/../src/CommandLine.slnx" -c Debug -v minimal /p:RoslynatorDotNetCli=true /m

Write-Host "DONE"
