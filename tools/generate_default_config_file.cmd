@echo off

dotnet build "..\src\Tools\ConfigFileGenerator\ConfigFileGenerator.csproj" -c Release

dotnet "..\src\Tools\ConfigFileGenerator\bin\Release\netcoreapp3.0\Roslynator.ConfigFileGenerator.dll" "..\src"

pause