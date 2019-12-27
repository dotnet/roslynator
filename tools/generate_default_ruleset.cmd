@echo off

dotnet build "..\src\Tools\RuleSetGenerator\RuleSetGenerator.csproj" -c Release

"..\src\Tools\RuleSetGenerator\bin\Release\net461\Roslynator.RuleSetGenerator.exe" "..\src"
