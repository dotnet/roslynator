@echo off

dotnet build "..\src\Tools\ConfigFileGenerator\ConfigFileGenerator.csproj" -c Release

"..\src\Tools\ConfigFileGenerator\bin\Release\net461\Roslynator.ConfigFileGenerator.exe" "..\src"
