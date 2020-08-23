@echo off

dotnet build "..\src\Tools\RuleSetGenerator\RuleSetGenerator.csproj" -c Release

dotnet "..\src\Tools\RuleSetGenerator\bin\Release\netcoreapp3.0\Roslynator.RuleSetGenerator.dll" "..\src"

pause