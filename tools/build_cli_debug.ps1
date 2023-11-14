dotnet clean "$PSScriptRoot/../src/CommandLine.sln" ` -c Debug

dotnet build "$PSScriptRoot/../src/CommandLine.sln" -c Debug -v minimal /p:RoslynatorDotNetCli=true /m

Write-Host "DONE"
