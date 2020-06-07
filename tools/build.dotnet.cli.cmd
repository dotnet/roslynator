@echo off

rd /S /Q "..\src\CommandLine\bin\Release\netcoreapp3.1"

del /Q "..\src\CommandLine\bin\Release\Roslynator.DotNet.Cli.*.nupkg"

dotnet restore --force "..\src\CommandLine.sln"

dotnet clean "..\src\CommandLine.sln"

dotnet pack "..\src\CommandLine\CommandLine.csproj" -c Release -v normal ^
 /p:RoslynatorDotNetCli=true,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591"

echo OK
pause
