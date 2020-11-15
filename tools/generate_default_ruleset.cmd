@echo off

dotnet build "..\src\Tools\RuleSetGenerator\RuleSetGenerator.csproj" -c Release

dotnet "..\src\Tools\RuleSetGenerator\bin\Release\netcoreapp3.1\Roslynator.RuleSetGenerator.dll" "..\src"

pause