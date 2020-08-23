@echo off

dotnet build "..\src\Tools\EditorConfigGenerator\EditorConfigGenerator.csproj" -c Release

dotnet "..\src\Tools\EditorConfigGenerator\bin\Release\netcoreapp3.0\Roslynator.EditorConfigGenerator.dll" "..\src"

pause